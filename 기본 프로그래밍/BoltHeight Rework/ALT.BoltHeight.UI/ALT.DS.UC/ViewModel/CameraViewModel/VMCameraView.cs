using ALT.DS.UC.Messenger;
using ALT.DSCamera;
using ALT.DSCamera.Camera3D;
using ALT.DSCamera.Interface;

using Cognex.VisionPro;
using Cognex.VisionPro.Implementation.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace ALT.DS.UC.ViewModel
{
    public partial class VMCameraView : GalaSoft.MvvmLight.ViewModelBase
    {

        private readonly Thread connectionCheckThread;
        private readonly Thread encoderCountReadThread;
        private bool threadStopper;
        private bool isLive;
        private bool continuousEnable;
        private bool liveEnable;
        private bool onlineEnable;
        private bool canLiveAcquisition;
        private ICogImage displayImage;
        private ICogAcqFifo fifo;

        public ICogAcqFifo Fifo
        {
            get => fifo; set
            {
                fifo = value;
                RaisePropertyChanged();
            }
        }
        public ICogImage DisplayImage
        {
            get => displayImage; set
            {
                displayImage = value;
                RaisePropertyChanged();
            }
        }
        public IEnumerable<string> LaserModes => Enum.GetNames(typeof(CogAcqLaserModeConstants));
        public IEnumerable<string> EncoderResolutions => Enum.GetNames(typeof(CogEncoderResolutionConstants));
        public IEnumerable<string> AcquisitionDirections => Enum.GetNames(typeof(CogProfileCameraDirectionConstants));
        public bool IsDSMax { get; private set; }
        public bool IsConnected => Md3DFrameGrabber.Instance.IsConnected;
        public I3DCamParams Params { get; set; }
        public int StartEncoderCount { get; set; }
        public int EndEncoderCount { get; set; }


        public int EncoderCount
        {
            get
            {
                if (Md3DFrameGrabber.Instance.UseEncoder)
                    return Md3DFrameGrabber.Instance.CurrentEncoderCount;
                else
                    return 0;
            }
        }


        public bool IsLive
        {
            get => isLive; set
            {
                isLive = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Live 버튼 활성화 플래그
        /// </summary>
        public bool LiveEnable
        {
            get => liveEnable; set
            {
                liveEnable = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Continuous 버튼 활성화 플래그
        /// </summary>
        public bool ContinuousEnable
        {
            get => continuousEnable; set
            {
                continuousEnable = value;
                RaisePropertyChanged();
            }
        }
        public bool IsOnline
        {
            get => Md3DFrameGrabber.Instance.IsOnline; set
            {
                Md3DFrameGrabber.Instance.IsOnline = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 온라인 버튼 활성화 플래그
        /// </summary>
        public bool OnlineEnable
        {
            get => onlineEnable; set
            {
                onlineEnable = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// StartLive 시 Acquisition 제어를 할지 말지 선택하는 플래그
        /// </summary>
        public bool CanLiveAcquisition
        {
            get => canLiveAcquisition; set
            {
                canLiveAcquisition = value;
                RaisePropertyChanged();
            }
        }
        public bool UseEncoderEnable { get => Md3DFrameGrabber.Instance.IsConnected; }

        public VMCameraView()
        {
            MessengerInstance = GalaSoft.MvvmLight.Messaging.Messenger.Default;

            Params = _3DParamContainer.Instance;

            InitializeRelayCommands();

            Md3DFrameGrabber.Initialize(true);
            Md3DFrameGrabber.Instance.FifoAcquisitionDone += Instance_FifoAcquisitionDone;
            Md3DFrameGrabber.Instance.OnImageGrabbed += DSCamera_OnImageGrabbed;
            Md3DFrameGrabber.Instance.On3DRangeImageGrabbed += DSCamera_On3DRangeImageGrabbed;
            Md3DFrameGrabber.Instance.OnExceptionOccured += Instance_OnExceptionOccured;

            connectionCheckThread = new Thread(ConnectionCheckAction);
            encoderCountReadThread = new Thread(EncoderCountReadAction);

            MessengerInstance.Register<AcquiredImageMessenger>(this, OnImageAcquired);
        }

        private void Instance_OnExceptionOccured(object sender, Exception e)
        {
            this.MessageBox(e.Message);
        }

        public override void Cleanup()
        {
            base.Cleanup();

            Md3DFrameGrabber.Instance.DisconnectCamera();
            threadStopper = true;
            if (connectionCheckThread.IsAlive)
                connectionCheckThread.Join();
        }

        private void Instance_FifoAcquisitionDone(object sender, ICogAcqFifo e)
        {
            Fifo = e;

            if (Fifo != null)
            {
                ContinuousEnable = true;
                LiveEnable = true;
                OnlineEnable = true;
                IsDSMax = Md3DFrameGrabber.Instance.IsDSMax;

                if(!connectionCheckThread.IsAlive)
                    connectionCheckThread.Start();

                if(!encoderCountReadThread.IsAlive)
                    encoderCountReadThread.Start();

                try
                {
                    _3DParamContainer.Instance.ApplyParams(Md3DFrameGrabber.Instance as I3DCamParams);
                }
                catch (Exception ex)
                {
                    MessengerInstance.Send(new ExceptionMessenger()
                    {
                        Exception = ex
                    });
                }
                
                

                RaisePropertyChanged(nameof(IsConnected));
                var messenger = new OnLoadingMessenger() { isLoading = false };

                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(messenger);

            }
        }

        private void DSCamera_On3DRangeImageGrabbed(object sender, ICogVisionData e)
        {
            MessengerInstance.Send(new _3DGraphicVisionDataAcquiredMessenger()
            {
                visionData = e
            });
        }

        private void DSCamera_OnImageGrabbed(object sender, ICogImage e)
        {
            MessengerInstance.Send(new AcquiredImageMessenger()
            {
                Image = e,
                Is2DPreview = e.GetType() != typeof(CogImage16Range),
                Purpose = ImagePurposeConstants.ToolRun,
                IsOnline = IsOnline
            });
        }

        

        private void OnImageAcquired(AcquiredImageMessenger obj)
        {
            if (obj.Is2DPreview)
                DisplayImage = obj.Image;
        }

        
    }

    [ValueConversion(typeof(string), typeof(double))]
    public class LengthToMiliMeterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Equals(string.Empty))
                return 0;

            if (!int.TryParse(value.ToString(), out int result))
                throw new InvalidOperationException("TargetType must be int");
            else
                return result * Md3DFrameGrabber.Instance.YScale;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
