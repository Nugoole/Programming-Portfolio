



using ALT.CVL.BaslerCam;
using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Extension;
using ALT.CVL.Common.Interface;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ALT.CVL.Controls
{
    /// <summary>
    /// UCCamera View에 대한 ViewModel입니다.
    /// </summary>
    public class VMCamera : ViewModelBase
    {
        private ICam myCam;
        private IAltFrameGrabber camNameToConnect;
        private CameraType camType;

        //public static VMCamera NewVMCamera => new VMCamera();

        public ImageSource Image { get; set; }
        public ObservableCollection<ICamParameter> Parameters { get; set; }


        public RelayCommand GrabOnce { get; set; }
        public RelayCommand WhiteBalance { get; set; }
        public RelayCommand StartLive { get; set; }
        public RelayCommand StopLive { get; set; }
        public CameraType CamType
        {
            get => camType; set
            {
                camType = value;
                RaisePropertyChanged(nameof(AvailableCameras));
            }
        }
        public IEnumerable<IAltFrameGrabber> AvailableCameras
        {
            get
            {
                switch (CamType)
                {
                    case CameraType.Basler:
                        return BaslerFrameGrabbers.Instance;
                    
                }

                return null;
            }
        }
        public IAltFrameGrabber CamNameToConnect
        {
            get => camNameToConnect; set
            {
                if (myCam != null)
                {
                    myCam.Disconnect();
                    myCam = null;
                }

                Set(ref camNameToConnect, value);
                OnConnectCam(value);
            }
        }
        public double? FPS => myCam?.FPS;
        public bool? IsMono => myCam?.Format?.IsMono;
        public bool? IsLive => myCam?.Status?.IsLive;

        public VMCamera()
        {
            GrabOnce = new RelayCommand(GrabOnceAction);
            StartLive = new RelayCommand(StartLiveAction);
            StopLive = new RelayCommand(StopLiveAction);
            WhiteBalance = new RelayCommand(WhiteBalanceAction);
            CamType = CameraType.Basler;
            DispatcherHelper.Initialize();
            DispatcherHelper.UIDispatcher.ShutdownStarted += UIDispatcher_ShutdownStarted;
        }

        private void UIDispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (myCam != null)
                myCam.Disconnect();
        }

        private void OnConnectCam(IAltFrameGrabber obj)
        {
            if (myCam != null)
                myCam.Disconnect();

            myCam = obj.Create();
            myCam.OutputImageFormat = OutputImageFormat.BitmapSource;
            myCam.OnImageGrabbed += ImageGrabber_OnImageGrabbed;

            Parameters = new ObservableCollection<ICamParameter>(myCam.Parameters);
            RaisePropertyChanged(nameof(Image));
            RaisePropertyChanged(nameof(Parameters));
            RaisePropertyChanged(nameof(IsMono));
        }

        //private void OnCamConnected(MsgCamConnect obj)
        //{
        //    myCam = obj.Cam;
        //    myCam.ImageGrabber.OnImageGrabbed += ImageGrabber_OnImageGrabbed;

        //    Parameters = new ObservableCollection<ICamParameter>(obj.Cam.Parameters);
        //    RaisePropertyChanged(nameof(Image));
        //    RaisePropertyChanged(nameof(Parameters));
        //    RaisePropertyChanged(nameof(IsMono));
        //}

        public VMCamera(ICam cam)
        {
            myCam = cam;
            
            myCam.OnImageGrabbed += ImageGrabber_OnImageGrabbed;
        }

        private void WhiteBalanceAction()
        {
            myCam.DoWhiteBalance();
        }

        private void StopLiveAction()
        {
            myCam.StopLive();
            RaisePropertyChanged(nameof(IsLive));
        }

        private void StartLiveAction()
        {
            myCam.StartLive();
            RaisePropertyChanged(nameof(IsLive));
        }

        private void GrabOnceAction()
        {
            myCam.Grab();
        }

        private void ImageGrabber_OnImageGrabbed(object sender, dynamic e)
        {
            if (myCam.OutputImageFormat == OutputImageFormat.BitmapSource)
            {
                e.Freeze();

                var imageSource = e;//BitmapToImageSourceConverter(e);




                Image = imageSource as BitmapSource;



                RaisePropertyChanged(nameof(Image));
            }
            //RaisePropertyChanged(nameof(FPS));
        }

        private ImageSource BitmapToImageSourceConverter(Bitmap e)
        {
            BitmapImage image = new BitmapImage();
            Stream bitmapStream = new MemoryStream();
            e.Save(bitmapStream, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmapStream.Seek(0, SeekOrigin.Begin);
            image.BeginInit();
            image.StreamSource = bitmapStream;
            image.EndInit();

            return image;
        }
    }

    class MultiBooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {

                if (item is bool boolean)
                {
                    if (!boolean)
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
