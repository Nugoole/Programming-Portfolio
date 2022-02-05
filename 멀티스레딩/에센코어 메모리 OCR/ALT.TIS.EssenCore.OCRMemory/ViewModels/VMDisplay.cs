using ALT.CVL;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Forms.Integration;

namespace ALT.TIS.EssenCore.OCRMemory
{
    public class VMDisplay : ViewModelBase
    {

        #region Constructor

        public VMDisplay()
        {
            OCWfhDisplay = MdConfigData.Getinstance().ConfigParam.OCWfhDisplay;
        }

        #endregion

        #region Variables

        private ObservableCollection<WindowsFormsHost> oCWfhDisplay;

        #endregion

        #region Properties

        public ObservableCollection<WindowsFormsHost> OCWfhDisplay { get => oCWfhDisplay; private set => Set(ref oCWfhDisplay, value); }

        #endregion

        #region Functions

        #endregion

    }
}
