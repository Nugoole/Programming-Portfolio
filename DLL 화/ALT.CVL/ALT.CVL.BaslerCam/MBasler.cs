

using ALT.CVL.Common.CamBaseModel;
using ALT.CVL.Common.Model;
using ALT.CVL.Common.Enum;

using Basler.Pylon;

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ALT.CVL.BaslerCam
{
    /// <summary>
    /// Basler DLL로 연결 가능한 바슬러 카메라 클래스
    /// </summary>
    internal sealed class MBasler : CamBase
    {
        private dynamic bitmapSource;
        private Int32Rect ROI;
        private Camera camera;
        private PixelDataConverter converter;
        private System.Windows.Media.PixelFormat camPixelFormat;
        private readonly string serialNumber;
        
        internal MBasler(string id)
        {
            serialNumber = id;
        }

        internal CamInfo CamInfoSetter { get=>camInfo; set=>camInfo = value; }


        /// <summary>
        /// 카메라를 연결합니다.
        /// </summary>
        /// <param name="camId">
        /// <see cref="MBasler.AvailableCameras"/>에서 가져온 카메라 정보
        /// </param>
        public override void Connect()
        {
            //재연결 시 오류 안나도록 하기, 하트비트 지정
            Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "3000");

            camera = new Camera(serialNumber);

            camera.CameraOpened += Configuration.AcquireContinuous;
            camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            camera.Open();

            //Initialize Interfaces
            InitCamInterfaces();

            foreach (var parameter in camera.Parameters)
            {
                if (parameter.IsReadable && parameter.IsWritable)
                {
                    //if (ParameterToUse.Params.Contains(parameter.Name))
                    //{
                    Type type;
                    var paramType = parameter.GetType().Name;

                    if (paramType.Contains("Enum"))
                        type = typeof(System.Enum);
                    else if (paramType.Contains("Bool"))
                        type = typeof(bool);
                    else
                        type = typeof(string);



                    Parameter param = new Parameter(parameter.Name, parameter.ToString(), !parameter.IsWritable, type, OnValueGet, Param_OnValueSet, type == typeof(System.Enum) ? (parameter as IEnumParameter).GetAllValues() : null);


                    //if(!Parameters.ParameterNames.Contains(param.Name))
                    try
                    {
                        ((CamParameters)camParameters).Add(param.Name, param);
                    }
                    catch (Exception)
                    {


                    }

                }
            }


            var width = camera.Parameters[PLCamera.Width].GetValue();
            var height = camera.Parameters[PLCamera.Height].GetValue();

            camPixelFormat = camera.Parameters[PLCamera.PixelFormat].GetValue().Contains("Mono") ? PixelFormats.Gray8 : PixelFormats.Bgr24;





            ROI = new Int32Rect(0, 0, (int)width, (int)height);
            converter = new PixelDataConverter();
            RaiseOnConnected();
        }

        public override void Disconnect()
        {
            if (camera.IsConnected)
            {
                if (camera.StreamGrabber.IsGrabbing)
                    camera.StreamGrabber.Stop();

                camera.Close();
            }
        }

        private void InitCamInterfaces()
        {
            format.IsMono = camera.Parameters[PLCamera.PixelFormat].ToString().Contains("Mono");

            //IsConnected 업데이트가 안됨 해결해야함.
            status = new BaslerCamStatus(camera) { IsLive = false };
        }

        public override void Grab()
        {
            //22.01.06 lys
            if(!camera.StreamGrabber.IsGrabbing)
                camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
        }
        public override void StartLive()
        {
            status.IsLive = true;
            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
        }
        public override void StopLive()
        {
            if (camera.StreamGrabber.IsGrabbing)
            {
                camera.StreamGrabber.Stop();
                status.IsLive = false;
            }
        }


        private void StreamGrabber_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;


                var width = grabResult.Width;
                var height = grabResult.Height;


                if (OutputImageFormat == OutputImageFormat.BitmapSource)
                {
                    bitmapSource = new WriteableBitmap((int)width, (int)height, 96, 96, camPixelFormat, null);

                    converter.OutputPixelFormat = bitmapSource.Format == PixelFormats.Gray8 ? PixelType.Mono8 : PixelType.BGR8packed;

                    bitmapSource.Lock();

                    converter.Convert(bitmapSource.BackBuffer, converter.GetBufferSizeForConversion(grabResult), grabResult);
                }
                else
                {
                    bitmapSource = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                    watch.Start();
                    var bmpData = bitmapSource.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                    converter.OutputPixelFormat = PixelType.BGRA8packed;

                    converter.Convert(bmpData.Scan0, converter.GetBufferSizeForConversion(grabResult), grabResult);

                    bitmapSource.UnlockBits(bmpData);
                    watch.Stop();
                    Console.WriteLine(watch.ElapsedMilliseconds);
                }

                if (OutputImageFormat == OutputImageFormat.BitmapSource)
                {
                    bitmapSource.AddDirtyRect(ROI);
                    bitmapSource.Unlock();
                }


                if (grabResult.GrabSucceeded)
                {
                    RaiseOnImageGrabbed(bitmapSource);
                }



            }
            catch (Exception)
            {

            }
        }

        private void Param_OnValueSet(object sender, string value)
        {
            Parameter parameter = sender as Parameter;


            camera.Parameters.Where(x => x.Name.Equals(parameter.Name))?.FirstOrDefault()?.ParseAndSetValue(value);

            if (parameter.Name.Equals(PLCamera.PixelFormat.ToString().Split('/').Last()))
            {
                var pixelFormat = value.Contains("Mono") ? PixelFormats.Gray8 : PixelFormats.Bgr24;

                bitmapSource = new WriteableBitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, bitmapSource.DpiX, bitmapSource.DpiY, pixelFormat, null);
            }

            if (parameter.Name.Equals(PLCamera.Width.ToString().Split('/').Last()))
            {
                bitmapSource = new WriteableBitmap(int.Parse(value), bitmapSource.PixelHeight, bitmapSource.DpiX, bitmapSource.DpiY, bitmapSource.Format, null);
                ROI.Width = int.Parse(value);
            }

            if (parameter.Name.Equals(PLCamera.Height.ToString().Split('/').Last()))
            {
                bitmapSource = new WriteableBitmap(bitmapSource.PixelWidth, int.Parse(value), bitmapSource.DpiX, bitmapSource.DpiY, bitmapSource.Format, null);
                ROI.Height = int.Parse(value);
            }

        }

        private string OnValueGet(string name)
        {
            if (camera.Parameters[name] is IBooleanParameter param)
            {
                return param.GetValue().ToString();
            }
            else
                return camera.Parameters[name].ToString();
        }


        public override void WhiteBalance()
        {
            if (format.IsMono)
                return;


            camera.Parameters[PLCamera.AutoFunctionAOISelector].SetValue(PLCamera.AutoFunctionAOISelector.AOI2);
            camera.Parameters[PLCamera.AutoFunctionAOIUsageWhiteBalance].SetValue(true);
            camera.Parameters[PLCamera.BalanceWhiteAuto].SetValue(PLCamera.BalanceWhiteAuto.Once);
            Grab();
        }
    }
}
