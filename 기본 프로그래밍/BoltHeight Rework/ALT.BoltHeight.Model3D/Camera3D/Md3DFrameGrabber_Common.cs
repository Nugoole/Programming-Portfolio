using ALT.DSCamera.Interface;

using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ALT.DSCamera.Camera3D
{
    
    enum DSMAXTriggerSourceConstants
    {
        External = 0,
        Encoder
    }

    enum DSMAXSpeedModeConstants
    {
        Low = 0,
        MediumLow,
        Medium,
        High
    }

    enum DSMAXMotionInputConstants
    {
        Manual = 0,
        TimeBased,
        SingleChannel,
        QuadratureEncoder
    }

    public partial class Md3DFrameGrabber
    {
        
        #region Field
        private CogFrameGrabbers grabbers;
        private CogCopyRegionTool splitter;
        private CogRectangle splitRegion;

        private readonly List<string> calibrationExceptionParams = new List<string>() { nameof(EncoderDirection), nameof(DistancePerCycle) };

        private static Md3DFrameGrabber instance;
        private static List<string> frameGrabbers;

        private ICogAcqFifo cam3D;
        private double exposure;
        private int allFrameCount;
        private bool isConnected;
        private bool flushed;
        private bool useHardwareTrigger;
        private double motionSpeedBuffer;


        private const string DSMAX_EncoderReset = "IF_CM_Encoder_Count_Reset";
        private const string DSMAX_TriggerSource = "CM_Trigger_Source";
        private const string DSMAX_TimeBasedLineRateHz = "TimeBasedLineRateHz";
        private const string DSMAX_LaserPower = "LaserPower";
        private const string DSMAX_SpeedMode = "SpeedMode";
        private const string DSMAX_YScale = "Y Scale";
        private const string DSMAX_ComputeYScale = "ComputeYScale";
        private const string DSMAX_IgnoreEncoderOverrun = "IgnoreEncoderOverrun";
        private const string DSMAX_LaserMode = "LaserMode";
        private const string DSMAX_ZDetectionBase = "DetectionStart";
        private const string DSMAX_ZDetectionHeight = "DetectionHeight";
        private const string DSMAX_EncoderDirection = "MotionDirection";
        private const string DSMAX_MotionInput = "MotionInput";
        private const string DSMAX_XScale = "X Scale";
        private const string DSMAX_StepsPerLine = "StepsPerLine";
        private const string DSMAX_DistancePerCycle = "DistancePerCycle";
        private const string DSMAX_EncoderCountUB = "CM_Encoder_Count_UB";
        private const string DSMAX_EncoderCountLB = "CM_Encoder_Count_LB";
        private const string DSMAX_HighDynamicRangeMode = "PC_CS_HDR_Mode";



        public event EventHandler<ICogAcqFifo> FifoAcquisitionDone;
        /// <summary>
        /// For Create 3D Rendering Image
        /// </summary>
        public event EventHandler<ICogVisionData> On3DRangeImageGrabbed;
        public event EventHandler<ICogImage> OnImageGrabbed;
        public event EventHandler<Exception> OnExceptionOccured;
        #endregion



        /// <summary>
        /// DS시리즈 카메라를 가져옵니다. 시리얼 넘버는 <see cref="ALT.DSCamera.Camera3D.Md3DFrameGrabber.FrameGrabbers"/>에서 가져옵니다.
        /// </summary>
        /// <param name="serialNumber">
        /// 시리얼 넘버입니다.
        /// </param>
        /// <returns>
        /// 연결 성공한 카메라를 반환합니다.
        /// </returns>
        public ICogAcqFifo GetAcqFifo(string serialNumber = "")
        {
            if (cam3D is null)
            {
                foreach (ICogFrameGrabber grabber in grabbers)
                {
                    if (grabber.SerialNumber.Contains(serialNumber))
                    {
                        new Thread(() =>
                        {
                            cam3D = grabber.CreateAcqFifo(grabber.AvailableVideoFormats[0], CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);

                            FifoAcquisitionDone?.Invoke(this, cam3D);
                        }).Start();
                        break;
                    }
                }
            }



            return cam3D;
        }
        private void Cam3D_Flushed(object sender, EventArgs e)
        {
            if (!flushed)
                flushed = true;
        }
        private void RaiseAllPropertyChanged()
        {
            GetType().GetProperties().ToList().ForEach(x => RaisePropertyChanged(x.Name));
        }

        /// <summary>
        /// 카메라가 이미지 취득을 성공했을 때
        /// </summary>
        /// <param name="sender">카메라</param>
        /// <param name="e"></param>
        private void AcqFifo_Complete(object sender, CogCompleteEventArgs e)
        {
            try
            {
                cam3D.GetFifoState(out _, out int numReady, out _);

                if (numReady > 0)
                {
                    var originalImage = cam3D.CompleteAcquireEx(new CogAcqInfo());

                    if (originalImage is CogImage16Range imageWithGrey && CameraMode.Equals("RangeWithGrey"))
                    {
                        splitRegion.SetXYWidthHeight(0, 0, imageWithGrey.Width / 2, imageWithGrey.Height);
                        splitter.InputImage = imageWithGrey;
                        splitter.Region = splitRegion;
                        splitter.Region.SelectedSpaceName = "#";



                        splitter.Run();

                        Image = splitter.OutputImage;

                        On3DRangeImageGrabbed?.Invoke(this, originalImage as ICogVisionData);
                    }
                    else
                        Image = originalImage;


                    OnImageGrabbed?.Invoke(this, Image);

                    if (allFrameCount++ % 4 == 0)
                    {
                        GC.Collect();
                    }



                    if (IsOnline)
                    {
                        cam3D.OwnedTriggerParams.TriggerEnabled = false;
                        while (cam3D.OwnedTriggerParams.TriggerEnabled) { };
                        StartAcquire();
                    }
                    else
                    {
                        if ((Image is CogImage16Range) && (UseHardwareTrigger || UseEncoder))
                            cam3D.OwnedTriggerParams.TriggerEnabled = false;
                    }




                    RaisePropertyChanged(nameof(Image));

                }
            }
            catch (Exception ex)
            {
                if (cam3D.OwnedTriggerParams.TriggerEnabled)
                    cam3D.OwnedTriggerParams.TriggerEnabled = false;

                if (!CameraMode.Contains("Intensity"))
                    OnExceptionOccured?.Invoke(this, ex);
            }
        }
        private void ParameterSet()
        {
            //General Camera Parameter
            cam3D.OwnedExposureParams.Exposure = 0.1;
            cam3D.OwnedTriggerParams.TriggerEnabled = false;
            cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Manual;


            //3DCamera Parameter
            if (!IsDSMax)
            {
                cam3D.OwnedRangeImageParams.XScale = 0.3;
                cam3D.OwnedProfileCameraParams.DetectionSensitivity = cam3D.OwnedProfileCameraParams.DetectionSensitivityMax / 2;
                cam3D.OwnedProfileCameraParams.HighDynamicRange = true;
                cam3D.OwnedProfileCameraParams.ZDetectionEnable = true;
                cam3D.OwnedProfileCameraParams.ZDetectionBase = 45;
                cam3D.OwnedProfileCameraParams.ZDetectionHeight = 25;
                cam3D.OwnedLineScanParams.ExpectedMotionSpeed = 50;
                cam3D.OwnedRangeImageParams.AutoCorrectPixelRowOrder = true;
                cam3D.OwnedLineScanParams.IgnoreTooFastEncoder = true;
                FirstPixelLocation = -47;
                HighDynamicRange = true;
                UseEncoder = false;
                UseHardwareTrigger = false;
                ScanLength = 400;
                StepsPerLine = 120;
                DistancePerCycle = 0.01;


                //Calibration File Access
                //cam3D.OwnedFieldCalibrationParams.FieldCalibrationFile = @"C:\Users\PPC-122ET\Desktop\Calibration 0605\DSFieldCalibration06-05-20_1557.car";
                //cam3D.OwnedFieldCalibrationParams.UseFieldCalibration = true;
            }
            //DSMAX 인 경우 초기화
            else
            {
                CameraMode = "RangeWithGrey";
                XScale = 0.015;
                StepsPerLine = 7;
                DistancePerCycle = 0.004;
                ZDetectionBase = 1;
                ZDetectionHeight = 5;

                SetDefaultPropertyFromDSMax(DSMAX_TimeBasedLineRateHz, 12000.ToString());
                SetDefaultPropertyFromDSMax(DSMAX_LaserPower, 100.ToString());
                SetDefaultPropertyFromDSMax(DSMAX_SpeedMode, DSMAXSpeedModeConstants.Medium.ToString());
                cam3D.FrameGrabber.OwnedGenTLAccess.SetDoubleFeature(DSMAX_YScale, 0.015);
                cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature(DSMAX_ComputeYScale, "False");
                cam3D.FrameGrabber.OwnedGenTLAccess.SetFeature(DSMAX_IgnoreEncoderOverrun, "True");
                SetDSMaxInterfaceProperty(DSMAX_EncoderCountUB, 100000.ToString());
                SetDSMaxInterfaceProperty(DSMAX_EncoderCountLB, 0.ToString());
                cam3D.OwnedCustomPropertiesParams.CustomPropsAsString = DSMaxParamAsStringBuffer;
            }
        }
        private Md3DFrameGrabber(bool initCamera)
        {
            Task.Run(() =>
            {
                if (initCamera)
                {
                    grabbers = new CogFrameGrabbers();

                    foreach (ICogFrameGrabber item in grabbers)
                    {
                        if ((item.Name.Contains("Cognex") && item.Name.Contains("DS")) || item.Name.Contains("DSMAX"))
                            FrameGrabbers.Add($@"{item.Name}:{item.SerialNumber}");
                    }
                    RaisePropertyChanged(nameof(FrameGrabbers));
                }
            });


            splitter = new CogCopyRegionTool();
            splitRegion = new CogRectangle();



            FifoAcquisitionDone += Md3DFrameGrabber_FifoAcquisitionDone;
        }

        private void Md3DFrameGrabber_FifoAcquisitionDone(object sender, ICogAcqFifo e)
        {
            if (cam3D != null)
            {
                isConnected = true;
                RaisePropertyChanged(nameof(IsConnected));
                ParameterSet();

                cam3D.Complete += AcqFifo_Complete;
                cam3D.Complete += SaveImage;
                cam3D.Flushed += Cam3D_Flushed;
                RaiseAllPropertyChanged();
            }
        }

        /// <summary>
        /// Md3DFrameGrabber가 초기화 되지 않은 경우 초기화 합니다.
        /// </summary>
        /// <param name="initCamera">
        /// 카메라를 초기화하려면 true로 설정합니다.
        /// </param>
        public static void Initialize(bool initCamera)
        {
            if (instance is null)
            {
                instance = new Md3DFrameGrabber(initCamera);
                isInitialized = true;
            }
        }


        /// <summary>
        /// 현재 카메라와 연결되어있는지 체크합니다.
        /// </summary>
        /// <returns>
        /// 현재 카메라의 연결상태입니다. 연결되었다면 <c>true</c>, 연결되어있지 않다면 <c>false</c>를 반환합니다.
        /// </returns>
        public bool ConnectionCheckFunc()
        {
            if (Monitor.TryEnter(cam3D, 10))
            {
                var changed = false;
                if (!IsDSMax)
                {
                    if (cam3D == null || cam3D.FrameGrabber.GetStatus(false) != CogFrameGrabberStatusConstants.Active)
                    {
                        if (isConnected)
                        {
                            IsConnected = false;
                            changed = true;
                        }
                    }
                    else
                    {
                        if (!isConnected)
                        {
                            IsConnected = true;
                            changed = true;
                        }
                    }
                }
                Monitor.Exit(cam3D);
                return changed;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// 카메라로 1장의 이미지 취득명령을 보냅니다.
        /// </summary>
        public void StartAcquire()
        {
            cam3D.OwnedTriggerParams.TriggerEnabled = false;

            if (UseEncoder || UseHardwareTrigger)
            {

                //트리거 소스 결정
                if (useHardwareTrigger)
                {
                    if (IsDSMax)
                        SetDSMaxInterfaceProperty(DSMAX_TriggerSource, DSMAXTriggerSourceConstants.External.ToString());
                }
                else
                {
                    if(IsDSMax)
                        SetDSMaxInterfaceProperty(DSMAX_TriggerSource, DSMAXTriggerSourceConstants.Encoder.ToString());
                }
                    
                

                cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Auto;
                cam3D.OwnedTriggerParams.TriggerEnabled = true;
            }
            else
            {
                cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Manual;
                cam3D.OwnedTriggerParams.TriggerEnabled = true;
                cam3D.StartAcquire();
            }
        }


        /// <summary>
        /// 현재 연결된 카메라를 연결해제합니다.
        /// </summary>
        public void DisconnectCamera()
        {
            if (cam3D != null)
            {
                if (IsConnected)
                {
                    cam3D.Complete -= AcqFifo_Complete;
                    cam3D.Complete -= SaveImage;
                    cam3D.Flushed -= Cam3D_Flushed;

                    

                    cam3D.FrameGrabber.Disconnect(true);
                }

            }
        }

        /// <summary>
        /// Encoder모드를 사용중인 경우, <see cref="CurrentEncoderCount"/> 값을 0으로 초기화합니다.
        /// </summary>
        public void ResetEncoderCount()
        {
            if (IsDSMax)
                cam3D.FrameGrabber.OwnedGenTLAccess.ExecuteCommand(DSMAX_EncoderReset);
            else
                cam3D.OwnedLineScanParams.ResetCounter();
        }


        /// <summary>
        /// 현재 카메라를 연결해제한 후 다시 연결합니다.
        /// </summary>
        public void ReconnectCurrentCam()
        {
            if (IsConnected)
            {
                //string camSN = cam3D.FrameGrabber.SerialNumber;
                DisconnectCamera();
                //다른 쓰레드에서 cam3D 사용중이라 에러남 참고

                Thread.Sleep(5000);
                //GetAcqFifo(camSN);

                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
        }

        /// <summary>
        /// 카메라를 2D Intensity 이미지 촬영모드 또는 3D Range 이미지 촬영모드로 바꿉니다.
        /// </summary>
        /// <param name="changeTo2D">
        /// <c>true</c>를 보내면 2D로, <c>false</c>를 보내면 3D로 바꿉니다.
        /// </param>
        public void Toggle2D3DCamera(bool changeTo2D)
        {
            if (IsConnected)
            {
                lock (cam3D)
                {
                    flushed = false;
                    cam3D.OwnedTriggerParams.TriggerEnabled = false;
                    cam3D.Flush();

                    while (!flushed) { };

                    if (changeTo2D)
                    {
                        cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Manual;

                        if (!IsDSMax)
                        {
                            cam3D.OwnedProfileCameraParams.CameraMode = CogAcqCameraModeConstants.IntensityWithGraphics;
                        }
                        else
                        {
                            CameraMode = CogAcqCameraModeConstants.Intensity.ToString();
                            motionSpeedBuffer = MotionSpeed;
                            MotionSpeed = 1;
                        }
                    }
                    else
                    {
                        cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Auto;

                        if (!IsDSMax)
                        {
                            CameraMode = CogAcqCameraModeConstants.RangeWithGrey.ToString();
                        }
                        else
                        {
                            CameraMode = CogAcqCameraModeConstants.RangeWithGrey.ToString();
                            MotionSpeed = motionSpeedBuffer;
                            motionSpeedBuffer = 0;
                        }
                    }

                    if (IsDSMax)
                        cam3D.OwnedCustomPropertiesParams.CustomPropsAsString = DSMaxParamAsStringBuffer;

                    if (changeTo2D)
                        cam3D.OwnedTriggerParams.TriggerEnabled = true;
                }
            }
        }

        /// <summary>
        /// 카메라 파라미터 정보를 적용합니다.
        /// </summary>
        /// <param name="paramObject">
        /// 카메라 파라미터 정보를 담고있는 객체입니다.
        /// </param>
        public void ApplyParams(I3DCamParams paramObject)
        {
            if (Monitor.TryEnter(cam3D, 300))
            {
                cam3D.OwnedTriggerParams.TriggerEnabled = false;

                flushed = false;
                cam3D.Flush();

                while (!flushed) { };



                foreach (var property in paramObject.GetType().GetProperties())
                {
                    if (!IsDSMax)
                        if (cam3D.OwnedFieldCalibrationParams.UseFieldCalibration)
                        {
                            if (calibrationExceptionParams.Contains(property.Name))
                                continue;
                        }

                    try
                    {
                        GetType().GetProperty(property.Name)?.SetValue(this, property.GetValue(paramObject));
                    }
                    catch (Exception ex)
                    {
                        OnExceptionOccured?.Invoke(this, new Exception(ex.Message + "Property Name : " + property.Name));
                    }
                }

                cam3D.OwnedCustomPropertiesParams.CustomPropsAsString = DSMaxParamAsStringBuffer;
            }
        }
    }
}
