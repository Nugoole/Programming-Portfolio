using ALT.DS.UC.Messenger;
using ALT.DSCamera.Tool;
using Cognex.VisionPro;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using System;
using System.Linq;
using System.Windows;

namespace ALT.DS.UC.ViewModel
{
    public enum RegionSetType
    {
        Height = 0,
        Tilt,
        CrossSection
    }

    public class VMRegionNameSet : ViewModelBase
    {
        public string NewToolName { get; set; }

        public RegionSetType RegionType { get; set; }
        public RelayCommand<Window> OnRegionSetComplete { get; set; }
        public RelayCommand<Window> OnRegionSetCancel { get; set; }
        public RoutedEventHandler OnCancelClicked { get; set; }

        public VMRegionNameSet()
        {
            OnRegionSetComplete = new RelayCommand<Window>(OnRegionSetCompleteAction);
            OnRegionSetCancel = new RelayCommand<Window>(OnRegionSetCancelAction);
            OnCancelClicked = new RoutedEventHandler(OnCancelClickedAction);
        }

        public void OnCancelClickedAction(object sender, RoutedEventArgs e)
        {
            (sender as Window).Close();
        }

        private void OnRegionSetCancelAction(Window obj)
        {
            obj.Close();
        }

        private void OnRegionSetCompleteAction(Window window)
        {
            if (RegionType == RegionSetType.Height)
            {
                foreach (ICogGraphicInteractive item in MdHeightCalcTool.Instance.HeightCalcRegions)
                {
                    if (item.TipText.Split(new string[] { "Bolt_" }, StringSplitOptions.RemoveEmptyEntries).Last().Equals(NewToolName))
                    {
                        var exception = new ExceptionMessenger()
                        {
                            Exception = new Exception("해당 이름이 이미 존재합니다.")
                        };

                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(exception);

                        return;
                    }
                }
            }
            else
            {
                foreach (var RegionCollection in MdTiltCalcTool.Instance.TiltCalcRegions)
                {
                    foreach (ICogGraphicInteractive item in RegionCollection)
                    {
                        if (item.TipText.Split(new string[] { "Tilt_" }, StringSplitOptions.RemoveEmptyEntries).Last().Split('_').First().Equals(NewToolName))
                        {
                            var exception = new ExceptionMessenger()
                            {
                                Exception = new Exception("해당 이름이 이미 존재합니다.")
                            };

                            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(exception);

                            return;
                        }
                    }
                }
            }

            
            var messenger = new RegionNameSetCompleteMessenger()
            {
                RegionType = RegionType,
                RegionName = NewToolName
            };

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(messenger);


            NewToolName = string.Empty;

            window.Close();
        }
    }
}
