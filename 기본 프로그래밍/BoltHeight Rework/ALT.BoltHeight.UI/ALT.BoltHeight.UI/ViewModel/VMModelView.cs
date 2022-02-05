using ALT.BoltHeight.Messenger;
using ALT.DSCamera.Tool;
using ALT.BoltHeight.Views;
using GalaSoft.MvvmLight.Command;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ALT.BoltHeight.UI.ViewModel
{
    public class VMModelView : GalaSoft.MvvmLight.ViewModelBase
    {
        private Grid modelViewGrid;
        private readonly MdDynamicUC mdDynamicUC;
        public RelayCommand OnModelBtnClicked { get; set; }
        public Grid ModelViewGrid
        {
            get => modelViewGrid;
            set
            {
                modelViewGrid = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<MdHeightCalcTool> HeightCalcTools { get; set; }
        public ObservableCollection<MdTiltCalcTool> TiltCalcTools { get; set; }

        public RelayCommand OnHeightCalcToolAdd { get; set; }
        public RelayCommand OnTiltCalcToolAdd { get; set; }
        public VMModelView()
        {
            OnModelBtnClicked = new RelayCommand(OnModelBtnClickedAction);
            OnHeightCalcToolAdd = new RelayCommand(OnHeightCalcToolAddAction);
            OnTiltCalcToolAdd = new RelayCommand(OnTiltCalcToolAddAction);
            mdDynamicUC = MdDynamicUC.GetDynamicCs();
            ModelViewGrid = mdDynamicUC.GridDraw();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<GridMessenger>(this, UCRefresh);
        }

        private void UCRefresh(GridMessenger refreshGrid)
        {
            ModelViewGrid = refreshGrid.subUC;
        }

        private void OnModelBtnClickedAction()
        {

        }
        private void OnTiltCalcToolAddAction()
        {

        }

        private void OnHeightCalcToolAddAction()
        {

        }
    }
}
