using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace ALT.CVL
{
    public class MdCameraData : ObservableObject
    {

        #region Constructor
        public MdCameraData(string camName)
        {
            CamName = camName;
            IPList = new ObservableCollection<string>();
            SelectedItem = string.Empty;
        }
        #endregion

        #region Variables
        private string selectedItem;
        #endregion

        #region Properties
        public string CamName { get; private set; }
        public ObservableCollection<string> IPList { get; set; }
        public string SelectedItem { get => selectedItem; set { Set(ref selectedItem, value); } }
        #endregion
    }
}
