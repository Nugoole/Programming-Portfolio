using ALT.DSCamera.Interface;

using Cognex.VisionPro;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ALT.DSCamera.Camera3D
{
    public partial class Md3DFrameGrabber : I3DCamParams, INotifyPropertyChanged
    {
        #region PropertyChangedConfig
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private bool isDSMax;
        private static bool isInitialized = false;

        /// <summary>
        /// 객체입니다.
        /// </summary>
        public static Md3DFrameGrabber Instance
        {
            get
            {
                return instance;
            }
        }



        public static bool IsInitialized => isInitialized;
        /// <summary>
        /// 연결 가능한 FrameGrabbers들입니다.
        /// </summary>
        public static List<string> FrameGrabbers
        {
            get
            {
                if (frameGrabbers is null)
                    frameGrabbers = new List<string>();

                return frameGrabbers;
            }
        }

        /// <summary>
        /// 현재 Online모드인지 나타냅니다.
        /// </summary>
        public bool IsOnline { get; set; }
        /// <summary>
        /// 현재 취득한 이미지의 인덱스입니다.
        /// </summary>
        public int FrameNumber => allFrameCount;

        /// <summary>
        /// <see cref="UseEncoder"/>이 true인 경우, 카메라 취득을 시작하는 Count를 가져오거나 세팅합니다.
        /// </summary>
        public int StartEncoderCount
        {
            get
            {
                if (IsDSMax)
                {
                    return int.Parse(GetDSMaxInterfaceProperty(DSMAX_EncoderCountLB));
                }
                else
                {
                    //TODO : DSMAX 아닐때 작성하기
                    //return Value When Cam is not DSMAX
                    return 0;
                }
            }
            set
            {
                if (IsDSMax)
                {
                    SetDSMaxInterfaceProperty(DSMAX_EncoderCountLB, value.ToString());
                }
                else
                {
                    //TODO : DSMAX 아닐때 작성하기
                }
            }
        }

        /// <summary>
        /// <see cref="UseEncoder"/>이 true인 경우, 카메라 취득을 종료하는 Count를 가져오거나 세팅합니다.
        /// </summary>
        public int EndEncoderCount
        {
            get
            {
                if (IsDSMax)
                {
                    return int.Parse(GetDSMaxInterfaceProperty(DSMAX_EncoderCountUB));
                }
                else
                {
                    //TODO : DSMAX 아닐때 작성하기
                    //return Value When Cam is not DSMAX
                    return 0;
                }
            }
            set
            {
                if (IsDSMax)
                {
                    SetDSMaxInterfaceProperty(DSMAX_EncoderCountUB, value.ToString());
                }
                else
                {
                    //TODO : DSMAX 아닐때 작성하기
                }
            }

        }

        /// <summary>
        /// 현재 연결된 3D 카메라가 DSMAX인지 나타냅니다.
        /// </summary>
        public bool IsDSMax
        {
            get
            {
                if (Monitor.TryEnter(cam3D, 100))
                {
                    isDSMax = cam3D.FrameGrabber.Name.Contains("DSMAX");
                    Monitor.Exit(cam3D);
                    return isDSMax;

                }
                else
                {
                    return isDSMax;
                }
            }
        }

        /// <summary>
        /// 현재 카메라가 연결되어있는지 나타냅니다.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            private set
            {
                isConnected = value;
                RaisePropertyChanged();
            }
        }

        #region Properties To SaveLoad

        /// <summary>
        /// 카메라의 노출값입니다.
        /// </summary>
        public double Exposure
        {
            get
            {
                if (cam3D != null)
                    return cam3D.OwnedExposureParams.Exposure;

                return 0;
            }
            set
            {
                RaisePropertyChanged();
                cam3D.OwnedExposureParams.Exposure = exposure;
            }
        }

        public void StartLive()
        {
            cam3D.OwnedTriggerParams.TriggerEnabled = false;

            cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.FreeRun;

            cam3D.OwnedTriggerParams.TriggerEnabled = true;
        }

        public void StopLive()
        {
            cam3D.OwnedTriggerParams.TriggerEnabled = false;

            cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Manual;
        }

        /// <summary>
        /// DS시리즈 카메라의 Sensitivity입니다.
        /// </summary>
        public double Sensitivity
        {
            get
            {
                if (cam3D != null && !IsDSMax)
                    return cam3D.OwnedProfileCameraParams.DetectionSensitivity / (double)cam3D.OwnedProfileCameraParams.DetectionSensitivityMax;

                return 0;
            }
            set
            {
                if (cam3D != null && !IsDSMax)
                {
                    cam3D.OwnedProfileCameraParams.DetectionSensitivity = (int)(value * cam3D.OwnedProfileCameraParams.DetectionSensitivityMax);
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 레이저모드를 변경합니다. <see cref="LaserModes"/>
        /// </summary>
        public string LaserMode
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                    {
                        if (DSGetFeature(DSMAX_LaserMode).Equals(CogAcqLaserModeConstants.Strobed.ToString()))
                            return CogAcqLaserModeConstants.Strobed.ToString();
                        else
                        {
                            if (GetDSMaxRemoteDeviceProperty(DSMAX_LaserPower).Equals(0.ToString()))
                                return CogAcqLaserModeConstants.Off.ToString();
                            else
                                return CogAcqLaserModeConstants.On.ToString();
                        }
                    }
                    else
                        return cam3D.OwnedProfileCameraParams.LaserMode.ToString();
                }

                return string.Empty;
            }
            set
            {
                if (cam3D != null)
                {
                    if (Enum.TryParse(value, out CogAcqLaserModeConstants result))
                    {
                        if (!IsDSMax)
                            cam3D.OwnedProfileCameraParams.LaserMode = result;
                        else
                        {
                            if (result == CogAcqLaserModeConstants.Strobed)
                                DSSetFeature(DSMAX_LaserMode, CogAcqLaserModeConstants.Strobed.ToString());
                            else if (result == CogAcqLaserModeConstants.On)
                            {
                                DSSetFeature(DSMAX_LaserMode, "Manual");
                                SetDSMaxRemoteDeviceProperty(DSMAX_LaserPower, 128.ToString());
                            }
                            else
                            {
                                DSSetFeature(DSMAX_LaserMode, "Manual");
                                SetDSMaxRemoteDeviceProperty(DSMAX_LaserPower, 0.ToString());
                            }

                        }

                    }

                    RaisePropertyChanged();
                }
            }
        }



        /// <summary>
        /// Scan할 길이를 가져오거나 세팅합니다.
        /// </summary>
        public int ScanLength
        {
            get
            {
                if (cam3D != null)
                {
                    cam3D.OwnedROIParams.GetROIXYWidthHeight(out _, out _, out _, out int Length);
                    return Length;
                }

                return 0;
            }
            set
            {
                if (cam3D != null)
                {
                    cam3D.OwnedROIParams.GetROIXYWidthHeight(out int x, out int y, out int w, out int h);

                    try
                    {
                        cam3D.OwnedROIParams.SetROIXYWidthHeight(x, y, w, value);
                        RaisePropertyChanged();
                    }
                    catch (Exception)
                    {
                        value = h;
                        cam3D.OwnedROIParams.SetROIXYWidthHeight(x, y, w, h);
                        RaisePropertyChanged();
                    }
                }
            }
        }


        /// <summary>
        /// 레이저의 세기를 가져오거나 설정합니다. 설정값의 범위는 0~255입니다.
        /// </summary>
        public int LaserPower
        {
            get
            {
                if (cam3D != null)
                    if (IsDSMax)
                        return int.Parse(DSMaxCustomProperties[DSMAX_LaserPower]);

                return 0;
            }
            set
            {
                if (cam3D != null)
                    if (IsDSMax)
                        SetDefaultPropertyFromDSMax(DSMAX_LaserPower, value.ToString());

                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Intensity 모드에서 Z축(세로축)의 ROI 시작지점을 나타냅니다.
        /// </summary>
        public double ZDetectionBase
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return double.Parse(DSMaxCustomProperties[DSMAX_ZDetectionBase]);
                    else
                        return cam3D.OwnedProfileCameraParams.ZDetectionBase;
                }
                return 0;
            }
            set
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        SetDefaultPropertyFromDSMax(DSMAX_ZDetectionBase, value.ToString());
                    else
                    {
                        if (ZDetectionHeightMax > value)
                            cam3D.OwnedProfileCameraParams.ZDetectionBase = value;
                    }


                    RaisePropertyChanged();
                }

            }
        }

        /// <summary>
        /// Intensity 모드에서 Z축(세로축)의 ROI 길이를 나타냅니다.
        /// </summary>
        public double ZDetectionHeight
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return double.Parse(DSMaxCustomProperties[DSMAX_ZDetectionHeight]);
                    else
                        return cam3D.OwnedProfileCameraParams.ZDetectionHeight;
                }

                return 0;
            }
            set
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        SetDefaultPropertyFromDSMax(DSMAX_ZDetectionHeight, value.ToString());
                    else
                    {
                        if (ZDetectionHeightMax - ZDetectionBase > value)
                            cam3D.OwnedProfileCameraParams.ZDetectionHeight = value;
                    }

                    RaisePropertyChanged();
                }
            }
        }


        /// <summary>
        /// 카메라의 취득 방향을 가져오거나 설정합니다. <see cref="AcquisitionDirections"/>
        /// </summary>
        public string AcquireDirection
        {
            get
            {
                if (cam3D != null && !IsDSMax)
                    return cam3D.OwnedLineScanParams.ProfileCameraAcquireDirection.ToString();

                return string.Empty;
            }
            set
            {
                if (cam3D != null && !IsDSMax)
                {
                    if (Enum.TryParse(value, out CogProfileCameraDirectionConstants result))
                        cam3D.OwnedLineScanParams.ProfileCameraAcquireDirection = result;

                    RaisePropertyChanged();
                }
            }
        }


        /// <summary>
        /// Encoder의 방향을 설정합니다. <see cref="AcquisitionDirections"/>
        /// </summary>
        public string EncoderDirection
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return DSMaxCustomProperties[DSMAX_EncoderDirection];
                    else
                        return cam3D.OwnedLineScanParams.ProfileCameraPositiveEncoderDirection.ToString();
                }

                return string.Empty;
            }
            set
            {
                if (cam3D != null)
                {

                    if (Enum.TryParse(value, out CogProfileCameraDirectionConstants result))
                    {
                        if (IsDSMax)
                            SetDefaultPropertyFromDSMax(DSMAX_EncoderDirection, result.ToString());
                        else
                            cam3D.OwnedLineScanParams.ProfileCameraPositiveEncoderDirection = result;
                    }

                    RaisePropertyChanged();
                }
            }
        }


        /// <summary>
        /// Encoder를 사용하는지를 나타냅니다.
        /// </summary>
        public bool UseEncoder
        {
            get
            {
                if (cam3D != null)
                {
                    if (!IsDSMax)
                        return cam3D.OwnedLineScanParams.TriggerFromEncoder;
                    else
                        return DSMaxCustomProperties[DSMAX_MotionInput].Equals(DSMAXMotionInputConstants.QuadratureEncoder.ToString());
                }
                else
                    return false;
            }
            set
            {
                if (cam3D != null)
                {
                    if (value)
                    {
                        if (IsDSMax)
                        {
                            cam3D.OwnedTriggerParams.TriggerEnabled = false;

                            cam3D.Flush();

                            cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Auto;
                            SetDefaultPropertyFromDSMax(DSMAX_MotionInput, DSMAXMotionInputConstants.QuadratureEncoder.ToString());

                            if (!useHardwareTrigger)
                                SetDSMaxInterfaceProperty(DSMAX_TriggerSource, DSMAXTriggerSourceConstants.Encoder.ToString());
                        }
                        else
                        {
                            cam3D.OwnedTriggerParams.TriggerEnabled = false;

                            cam3D.Flush();

                            cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Auto;
                            cam3D.OwnedLineScanParams.ResetCounterOnHardwareTrigger = true;
                            cam3D.OwnedLineScanParams.TriggerFromEncoder = value;
                        }
                    }
                    else
                    {
                        if (IsDSMax)
                        {
                            cam3D.OwnedTriggerParams.TriggerEnabled = false;

                            cam3D.Flush();

                            cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Manual;
                            SetDefaultPropertyFromDSMax(DSMAX_MotionInput, DSMAXMotionInputConstants.TimeBased.ToString());
                            SetDSMaxInterfaceProperty(DSMAX_TriggerSource, DSMAXTriggerSourceConstants.External.ToString());
                        }
                        else
                        {
                            cam3D.OwnedTriggerParams.TriggerEnabled = false;

                            cam3D.Flush();

                            cam3D.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Manual;
                            cam3D.OwnedLineScanParams.ResetCounterOnHardwareTrigger = false;
                            cam3D.OwnedLineScanParams.TriggerFromEncoder = value;
                        }
                    }

                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// HDR 모드의 사용여부를 나타냅니다.
        /// </summary>
        public bool HighDynamicRange
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                    {
                        return GetDSMaxRemoteDeviceProperty(DSMAX_HighDynamicRangeMode).Equals("SDA");
                    }
                    else
                    {
                        return cam3D.OwnedProfileCameraParams.HighDynamicRange;
                    }
                }

                return false;
            }
            set
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                    {
                        if (value)
                            SetDSMaxRemoteDeviceProperty(DSMAX_HighDynamicRangeMode, "SDA");
                        else
                            SetDSMaxRemoteDeviceProperty(DSMAX_HighDynamicRangeMode, "Linear");
                    }
                    else
                    {
                        cam3D.OwnedProfileCameraParams.HighDynamicRange = value;
                        RaisePropertyChanged();
                    }
                }
            }
        }


        /// <summary>
        /// XScale
        /// </summary>
        public double XScale
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return double.Parse(DSMaxCustomProperties["X Scale"]);
                    else
                        return cam3D.OwnedRangeImageParams.XScale;
                }

                return 0;
            }
            set
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        SetDefaultPropertyFromDSMax(DSMAX_XScale, value.ToString());
                    else
                    {
                        if (value > cam3D.OwnedRangeImageParams.XScaleMin && value < cam3D.OwnedRangeImageParams.XScaleMax)
                        {
                            cam3D.OwnedRangeImageParams.XScale = value;
                            var width = Math.Abs(FirstPixelLocation) * 2 / value;

                            cam3D.OwnedROIParams.GetROIXYWidthHeight(out int x, out int y, out int w, out int h);
                            try
                            {
                                cam3D.OwnedROIParams.SetROIXYWidthHeight(x, y, (int)width, h);
                            }
                            catch (Exception)
                            {
                                value = w;
                                cam3D.OwnedROIParams.SetROIXYWidthHeight(x, y, (int)width, h);
                            }
                        }
                    }

                    RaisePropertyChanged();
                }
            }
        }


        /// <summary>
        /// MotionSpeed
        /// </summary>
        public double MotionSpeed
        {
            get
            {
                if (cam3D != null)
                {
                    if (!IsDSMax)
                        return cam3D.OwnedLineScanParams.ExpectedMotionSpeed;
                    else
                        return double.Parse(DSMaxCustomProperties[DSMAX_TimeBasedLineRateHz]);
                }


                return 0;
            }
            set
            {
                if (value > 0 && value <= 25000)
                {
                    if (cam3D != null)
                    {
                        if (!IsDSMax)
                            cam3D.OwnedLineScanParams.ExpectedMotionSpeed = value;
                        else
                            SetDefaultPropertyFromDSMax(DSMAX_TimeBasedLineRateHz, value.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Encoder의 분해능을 가져오거나 설정합니다.
        /// </summary>
        public int EncoderRes
        {
            get
            {
                if (cam3D != null && !IsDSMax)
                {
                    return (int)Math.Pow(2, (double)cam3D.OwnedLineScanParams.EncoderResolution);
                }

                return 0;
            }
            set
            {
                if (cam3D != null && !IsDSMax)
                {
                    cam3D.OwnedLineScanParams.EncoderResolution = ((CogEncoderResolutionConstants)Math.Log(value, 2));
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 라인당 몇 Step을 설정할 것인지 나타냅니다.
        /// </summary>
        public double StepsPerLine
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return double.Parse(DSMaxCustomProperties[DSMAX_StepsPerLine]);
                    else
                    {
                        cam3D.OwnedLineScanParams.GetStepsPerLine(out int stepsPerLine, out int step16thsPerLine);
                        return stepsPerLine + step16thsPerLine / 16;
                    }
                }

                return 0;
            }
            set
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        SetDefaultPropertyFromDSMax(DSMAX_StepsPerLine, value.ToString());
                    else
                        cam3D.OwnedLineScanParams.SetStepsPerLine((int)value, (int)value / 16);

                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Encoder의 한 Cycle당 실제 진행 길이를 나타냅니다.
        /// </summary>
        public double DistancePerCycle
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return double.Parse(DSMaxCustomProperties[DSMAX_DistancePerCycle]);
                    else
                        return cam3D.OwnedLineScanParams.DistancePerCycle;
                }

                return 0;
            }
            set
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        SetDefaultPropertyFromDSMax(DSMAX_DistancePerCycle, value.ToString());
                    else
                        cam3D.OwnedLineScanParams.DistancePerCycle = value;


                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 카메라의 X 축 중심으로 취득 시작 X OffSet 지점을 설정합니다. 음수 값을 지원합니다.
        /// </summary>
        public double FirstPixelLocation
        {
            get
            {
                if (cam3D != null && !IsDSMax)
                    return cam3D.OwnedRangeImageParams.FirstPixelLocation;

                return 0;
            }
            set
            {
                if (cam3D != null && !IsDSMax)
                {
                    if (value > -cam3D.OwnedRangeImageParams.FirstPixelLocationRange && value < cam3D.OwnedRangeImageParams.FirstPixelLocationRange)
                    {
                        cam3D.OwnedRangeImageParams.FirstPixelLocation = value;
                        RaisePropertyChanged();
                    }
                }
            }
        }

        /// <summary>
        /// 카메라의 모드를 설정합니다. <see cref="CogAcqCameraModeConstants"/>
        /// </summary>
        public string CameraMode
        {
            get
            {
                if (cam3D != null)
                {
                    if (!IsDSMax)
                        return cam3D.OwnedProfileCameraParams.CameraMode.ToString();
                    else
                        return DSMaxCustomProperties[nameof(CameraMode)];
                }

                return string.Empty;
            }
            set
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        SetDefaultPropertyFromDSMax(nameof(CameraMode), value.ToString());
                    else
                        if (Enum.TryParse(value, out CogAcqCameraModeConstants result))
                        cam3D.OwnedProfileCameraParams.CameraMode = result;
                }
            }
        }

        /// <summary>
        /// 트리거를 HardwareTrigger로 설정하거나 가져옵니다.
        /// </summary>
        public bool UseHardwareTrigger
        {
            get => useHardwareTrigger; set
            {
                useHardwareTrigger = value;
                cam3D.OwnedTriggerParams.TriggerEnabled = false;

                cam3D.OwnedTriggerParams.TriggerModel = value ? CogAcqTriggerModelConstants.Auto : CogAcqTriggerModelConstants.Manual;
            }
        }
        #endregion


        #region ReadOnlyProperties

        /// <summary>
        /// 카메라의 이름입니다.
        /// </summary>
        public string CamName
        {
            get
            {
                if (cam3D != null)
                    return cam3D.FrameGrabber.Name;

                return string.Empty;
            }
        }

        /// <summary>
        /// 최대 라인주파수를 가져옵니다.
        /// </summary>
        public double MaxLineFreq
        {
            get
            {
                if (cam3D != null && !IsDSMax)
                    return cam3D.OwnedLineScanParams.MaximumLineFrequency;

                return 0;
            }
        }

        /// <summary>
        /// DSMAX의 Custom Property를 가져옵니다.
        /// </summary>
        public Dictionary<string, string> DSMaxCustomProperties
        {
            get
            {
                if (IsDSMax)
                {
                    var propertyList = cam3D.OwnedCustomPropertiesParams.CustomPropsAsString.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    propertyList.ForEach(x => x.Replace(@"Write\t", ""));

                    var dictionary = new Dictionary<string, string>();

                    propertyList.ForEach(x =>
                    {
                        var key = x.Split('\t')[1];
                        var value = x.Split('\t')[2];

                        dictionary.Add(key, value);
                    });

                    return dictionary;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 현재 Line주파수를 가져옵니다.
        /// </summary>
        public double LineFreq
        {
            get
            {
                if (cam3D != null && !IsDSMax)
                    return cam3D.OwnedLineScanParams.LineFrequency;

                return 0;
            }
        }

        /// <summary>
        /// 마지막으로 취득 된 이미지입니다.
        /// </summary>
        public ICogImage Image { get; private set; }

        /// <summary>
        /// 현재 EncoderCount입니다.
        /// </summary>
        public int CurrentEncoderCount
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return int.Parse(GetDSMaxInterfaceProperty("CM_Encoder_Count"));
                    else
                        return cam3D.OwnedLineScanParams.CurrentEncoderCount;
                }


                return -1;
            }
        }

        /// <summary>
        /// ZDetectionHeight의 최대값입니다.
        /// </summary>
        public double ZDetectionHeightMax
        {
            get
            {
                if (cam3D != null && !IsDSMax)
                    return cam3D.OwnedProfileCameraParams.ZDetectionHeightMax;

                return 0;
            }
        }
        /// <summary>
        /// 트리거를 활성화시킵니다. 현재 값이 False이면 이미지 취득이 불가능합니다.
        /// </summary>
        public bool TriggerEnable
        {
            get
            {
                return cam3D.OwnedTriggerParams.TriggerEnabled;
            }
            set
            {
                cam3D.OwnedTriggerParams.TriggerEnabled = value;
            }
        }

        /// <summary>
        /// 설정된 YScale입니다.
        /// </summary>
        public double YScale
        {
            get
            {
                if (cam3D != null)
                {
                    if (IsDSMax)
                        return double.Parse(cam3D.FrameGrabber.OwnedGenTLAccess.GetFeature(DSMAX_YScale));
                    else
                        return cam3D.OwnedRangeImageParams.YScale;
                }


                return 0;
            }
        }

        public IEnumerable<string> LaserModes => Enum.GetNames(typeof(CogAcqLaserModeConstants));
        public IEnumerable<string> EncoderResolutions => Enum.GetNames(typeof(CogEncoderResolutionConstants));
        public IEnumerable<string> AcquisitionDirections => Enum.GetNames(typeof(CogProfileCameraDirectionConstants));

        

        #endregion

    }
}
