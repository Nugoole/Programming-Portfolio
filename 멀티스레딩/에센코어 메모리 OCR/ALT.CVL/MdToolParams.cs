using Cognex.VisionPro.Implementation;
using System.Collections.ObjectModel;

namespace ALT.CVL
{
    public class MdToolParams
    {
        #region Constructor
        public MdToolParams()
        {
            OCToolBase = new ObservableCollection<CogToolBase>();
        }
        #endregion

        #region Variables

        #endregion

        #region Properties
        public ObservableCollection<CogToolBase> OCToolBase { get; set; }
        #endregion

        #region Functions

        #endregion
    }
}
