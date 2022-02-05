using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace ALT.CVL
{
    public class MdTotalParamInfo : ObservableObject
    {
        #region Constructor
        public MdTotalParamInfo()
        {
            CameraParamInfo = new MdCameraParams();
            ToolParamInfo = new MdToolParams();
        }
        #endregion

        #region Variables

        #endregion

        #region Properties
        public MdCameraParams CameraParamInfo { get; set; }
        [JsonIgnore]
        public MdToolParams ToolParamInfo { get; set; }
        #endregion

        #region Functions

        #endregion
    }
}
