using ALT.DS.UC.Messenger;
using ALT.DSCamera;
using ALT.DSCamera.Camera3D;
using ALT.DSCamera.Interface;
using GalaSoft.MvvmLight.Command;

using System.Threading;

namespace ALT.DS.UC.ViewModel
{
    public partial class VMCameraView : GalaSoft.MvvmLight.ViewModelBase
    {
        public RelayCommand AcquireImage { get; set; }
        public RelayCommand ApplyChangedParams { get; set; }
        public RelayCommand OnEncoderCountReset { get; set; }
        public RelayCommand<bool> OnLiveBtnClicked { get; set; }
        public RelayCommand<bool> OnContinuousBtnClicked { get; set; }
        public RelayCommand<bool> OnOnlineBtnClicked { get; set; }
        public RelayCommand OnTriggerBtnClicked { get; set; }
        public RelayCommand<int> OnSetStartEncoderCount { get; set; }
        public RelayCommand<int> OnSetEndEncoderCount { get; set; }
        public RelayCommand OnReconnectClicked { get; set; }


        private void InitializeRelayCommands()
        {
            AcquireImage = new RelayCommand(AcquireImageAction);
            ApplyChangedParams = new RelayCommand(ApplyChangedParamsAction);
            OnLiveBtnClicked = new RelayCommand<bool>(OnLiveBtnClickedAction);
            OnContinuousBtnClicked = new RelayCommand<bool>(OnContinuousBtnClickedAction);
            OnOnlineBtnClicked = new RelayCommand<bool>(OnOnlineBtnClickedAction);

            OnTriggerBtnClicked = new RelayCommand(OnTriggerBtnClickedAction);
            OnEncoderCountReset = new RelayCommand(OnEncoderCountResetAction);
            OnSetStartEncoderCount = new RelayCommand<int>(OnSetStartEncoderCountAction);
            OnSetEndEncoderCount = new RelayCommand<int>(OnSetEndEncoderCountAction);
            OnReconnectClicked = new RelayCommand(OnReconnectClickedAction);
        }

        private void OnReconnectClickedAction()
        {
            Md3DFrameGrabber.Instance.ReconnectCurrentCam();
        }

        private void OnSetEndEncoderCountAction(int endEncoderCount)
        {
            EndEncoderCount = endEncoderCount;
            Md3DFrameGrabber.Instance.EndEncoderCount = endEncoderCount;
            RaisePropertyChanged(nameof(EndEncoderCount));
        }

        private void OnSetStartEncoderCountAction(int startEncoderCount)
        {
            StartEncoderCount = startEncoderCount;
            Md3DFrameGrabber.Instance.StartEncoderCount = startEncoderCount;
            RaisePropertyChanged(nameof(StartEncoderCount));
        }

        private void OnEncoderCountResetAction()
        {
            Md3DFrameGrabber.Instance.ResetEncoderCount();
        }

        private void EncoderCountReadAction()
        {
            while (!threadStopper)
                if (Params.UseEncoder)
                {
                    RaisePropertyChanged(nameof(EncoderCount));
                    Thread.Sleep(5);
                }
        }

        private void OnTriggerBtnClickedAction()
        {
            Md3DFrameGrabber.Instance.Toggle2D3DCamera(true);

            Md3DFrameGrabber.Instance.StartAcquire();
        }





        private void ConnectionCheckAction()
        {
            while (!threadStopper)
            {
                if (Md3DFrameGrabber.Instance.ConnectionCheckFunc())
                    RaisePropertyChanged(nameof(IsConnected));
            }
        }



        private void OnOnlineBtnClickedAction(bool isChecked)
        {
            Md3DFrameGrabber.Instance.Toggle2D3DCamera(!isChecked);

            IsOnline = isChecked;
            Md3DFrameGrabber.Instance.IsOnline = isChecked;
            ContinuousEnable = !isChecked;
            LiveEnable = !isChecked;

            if (isChecked)
                Md3DFrameGrabber.Instance.StartAcquire();
        }

        private void OnContinuousBtnClickedAction(bool isChecked)
        {
            if (isChecked)
            {
                Md3DFrameGrabber.Instance.Toggle2D3DCamera(isChecked);

                CanLiveAcquisition = true;
                IsLive = isChecked;
                OnlineEnable = !isChecked;
                LiveEnable = !isChecked;
            }
            else
            {
                CanLiveAcquisition = false;
                IsLive = isChecked;

                Md3DFrameGrabber.Instance.Toggle2D3DCamera(isChecked);

                OnlineEnable = !isChecked;
                LiveEnable = !isChecked;
            }

            RaisePropertyChanged(nameof(UseEncoderEnable));
        }

        private void OnLiveBtnClickedAction(bool isChecked)
        {
            if (isChecked)
            {
                Md3DFrameGrabber.Instance.Toggle2D3DCamera(isChecked);
                Md3DFrameGrabber.Instance.StartLive();
                
                IsLive = isChecked;
                ContinuousEnable = !isChecked;
                OnlineEnable = !isChecked;

                RaisePropertyChanged(nameof(UseEncoderEnable));
            }
            else
            {
                Md3DFrameGrabber.Instance.StopLive();
                IsLive = isChecked;
                ContinuousEnable = !isChecked;
                OnlineEnable = !isChecked;

                Md3DFrameGrabber.Instance.Toggle2D3DCamera(isChecked);
            }
        }



        private void ApplyChangedParamsAction()
        {
            var messenger = new OnLoadingMessenger() { isLoading = true };
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(messenger);

            var triggerEnable = false;

            if (Md3DFrameGrabber.Instance.TriggerEnable)
            {
                triggerEnable = true;
                Md3DFrameGrabber.Instance.TriggerEnable = false;
            }


            try
            {
                Md3DFrameGrabber.Instance.ApplyParams(_3DParamContainer.Instance as I3DCamParams);

                _3DParamContainer.Instance.ApplyParams(Md3DFrameGrabber.Instance as I3DCamParams);
            }
            catch (System.Exception ex)
            {
                MessengerInstance.Send(new ExceptionMessenger()
                {
                    Exception = ex
                });
            }
            

            if (triggerEnable)
                Md3DFrameGrabber.Instance.TriggerEnable = true;
            messenger.isLoading = false;
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(messenger);
        }

        private void AcquireImageAction()
        {
            Md3DFrameGrabber.Instance.StartAcquire();
        }
    }
}
