using System;
using System.Collections.ObjectModel;

namespace ALT.CVL
{
    public class MdTotalParamData : ITotalParam
    {
        #region Singleton Constructor
        private static readonly Lazy<MdTotalParamData> instance = new Lazy<MdTotalParamData>(() => new MdTotalParamData());
        private MdTotalParamData()
        {
            OCTotalParamInfo = new ObservableCollection<MdTotalParamInfo>(); 
            for (int i = 0; i < MdConfigData.Getinstance().ConfigParam.CamCount; i++)
            {
                OCTotalParamInfo.Add(new MdTotalParamInfo());
            }
        }
        public static MdTotalParamData Getinstance() => instance.Value;
        #endregion

        #region Variables

        #endregion

        #region Properties
        public ObservableCollection<MdTotalParamInfo> OCTotalParamInfo { get; set; }
        #endregion

        #region Functions
        #endregion
    }
}
