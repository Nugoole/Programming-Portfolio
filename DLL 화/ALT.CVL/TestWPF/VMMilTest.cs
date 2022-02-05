using ALT.CVL;
using ALT.CVL.Common.Interface;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Matrox.MatroxImagingLibrary;
using Matrox.MatroxImagingLibrary.WPF;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace TestWPF
{
    public class VMMilTest : ViewModelBase
    {
        private Image imageControl;
        private dynamic test;
        private MILWPFDisplay display;
        private MIL_ID displayID;
        private ILightController controller;

        public Image ImageControl
        {
            get => imageControl;
            set
            {
                imageControl = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OnConnect { get; set; }
        public RelayCommand OnDisconnect { get; set; }
        public RelayCommand OnRun { get; set; }
        public RelayCommand OnTrain { get; set; }
        public RelayCommand OnFind { get; set; }
        public RelayCommand OnFindLine { get; set; }
        public MIL_ID DisplayID { get => displayID; set => Set(ref displayID, value); }



        public ILightController ControllerTemp { get => controller; set => Set(ref controller, value); }


        public VMMilTest()
        {
            ImageControl = new Image();
            if(!IsInDesignMode)
                //test = new milTest();

            OnRun = new RelayCommand(OnRunClicked);
            OnTrain = new RelayCommand(OnTrainClicked);
            OnFind = new RelayCommand(OnFindClicked);
            OnFindLine = new RelayCommand(OnFindLineClicked);
            OnConnect = new RelayCommand(OnConnectClicked);
            OnDisconnect = new RelayCommand(OnDisconnectClicked);
            //DisplayID = test.DisplayId;

            
        }

        private void OnDisconnectClicked()
        {
            ControllerTemp?.Disconnect();
            controller = null;
            RaisePropertyChanged(nameof(ControllerTemp));
        }

        private void OnConnectClicked()
        {
            ControllerTemp = LightControllerConnector.Connect(ALT.CVL.Common.Enum.ControllerModel.ERS, ALT.CVL.Common.Enum.ConnectionProtocol.TCP);
            ControllerTemp.UpdateValueImmediately = true;
        }

        private void OnFindLineClicked()
        {
            test.FindEdge();
        }

        private void OnFindClicked()
        {
            test.Find();
        }

        private void OnTrainClicked()
        {
            test.Train();
        }

        private void OnRunClicked()
        {
            test.Grab();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            controller.Disconnect();
        }
    }
}
