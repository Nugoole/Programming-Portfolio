using ALT.CVL;
using ALT.CVL.Interfaces;
using ALT.TIS.EssenCore.OCRMemory.Messenger;
using ALT.Log.Interface;
using ALT.Log.Model;
using ALT.TIS.EssenCore.OCRMemory.ViewModels;
using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ALT.TIS.EssenCore.OCRMemory
{
    public class VMCamera : ViewModelBase
    {
        #region Constructor

        public VMCamera(int index)
        {
            cameraIndex = index;
            InitCommand();
            InitVMCamera();

            MessengerInstance.Register<ResultMessenger>(this, OnResultOut);
            RaisePropertyChanged(nameof(CamName));
        }

        private void OnResultOut(ResultMessenger obj)
        {
            if (!cameraIndex.Equals(obj.Index))
                return;

            if (obj.PassFail)
                OKCount++;
            else
                NGCount++;
        }

        private void InitVMCamera()
        {
            AcqFifo = MdFrameGrabbers.Getinstance().OCAcqFifo[cameraIndex];
            if (AcqFifo != null)
                CameraLogger.Write(Log.Enum.LogLevel.Info, $"Cam{cameraIndex} : Connect");
            AcqFifo.Complete += AcqFifo_Complete;
            ConfigData = MdConfigData.Getinstance().ConfigParam;
            isContinueMode = false;
        }

        private void InitCommand()
        {
            CmdShowCameraView = new RelayCommand(ShowCameraView);
            CmdOneShot = new RelayCommand(OneShot);
            CmdContinueShot = new RelayCommand(ContinueShot);
        }

        #endregion

        #region Variables

        private UserControl cameraViewItem;
        private bool isContinueMode;
        private int trgCount;
        private int nGCount;
        private int oKCount;
        private int totalCount;
        private string camName;
        private readonly ICogAcqInfo info = new CogAcqInfo();
        private readonly int cameraIndex;
        private ILogger CameraLogger => LoggerDictionary.Instance.GetLogger(Log.Enum.LogUsage.System);
        #endregion

        #region Properties
        public ICogAcqFifo AcqFifo { get; private set; }
        public IConfigParam ConfigData { get; set; }
        public RelayCommand CmdShowCameraView { get; private set; }
        public RelayCommand CmdOneShot { get; set; }
        public RelayCommand CmdContinueShot { get; set; }
        public UserControl CameraViewItem { get => cameraViewItem; private set => Set(ref cameraViewItem, value); }
        public bool IsContinueMode { get => isContinueMode; set => Set(ref isContinueMode, value); }
        public int NGCount { get => nGCount; set => Set(ref nGCount, value); }
        public int OKCount { get => oKCount; set => Set(ref oKCount, value); }
        public int TotalCount { get => totalCount; set => Set(ref totalCount, value); }
        public string CamName { get => AcqFifo.FrameGrabber.Name; }
        #endregion

        #region Functions

        private void ShowCameraView()
        {

        }

        private void OneShot()
        {
            AcqFifo.StartAcquire();
            CameraLogger.Write(Log.Enum.LogLevel.Info, $"Cam{cameraIndex} : One Shot");
        }

        private void ContinueShot()
        {
            AcqFifo.StartAcquire();
            CameraLogger.Write(Log.Enum.LogLevel.Info, $"Cam{cameraIndex} : Continue");
        }

        private void AcqFifo_Complete(object sender, CogCompleteEventArgs e)
        {
            var currentImage = AcqFifo.CompleteAcquireEx(info);
            MessengerInstance.Send<ICogImage, VMTool>(currentImage);
            if (isContinueMode)
            {
                Task.Delay(500).Wait();
                AcqFifo.StartAcquire();
            }
            trgCount++;
            if (trgCount > 5)
            {
                GC.Collect();
                trgCount = 0;
            }
        }

        #endregion

    }
}
