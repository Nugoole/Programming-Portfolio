using ALT.CVL.Interfaces;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms.Integration;

namespace ALT.CVL
{
    public class MdConfigData : ObservableObject
    {
        #region Singleton Constructor
        private static readonly Lazy<MdConfigData> instance = new Lazy<MdConfigData>(() => new MdConfigData());
        private MdConfigData()
        {
            InitializeData();
        }
        public static MdConfigData Getinstance() => instance.Value;
        #endregion

        #region Variables
        public readonly string configFilePath = AppDomain.CurrentDomain.BaseDirectory;
        public readonly string configFileName = "Config";
        #endregion

        #region Properties
        public IConfigParam ConfigParam { get; set; }
        #endregion

        #region Functions
        private void InitializeData()
        {   
            try
            {
                ConfigParam = new MdConfigParam(configFilePath, configFileName);
                var data = MdFileIO.JsonFileLoad<MdConfigParam>(configFilePath, configFileName);
                if (data != null)
                    ConfigParam = data;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
        #endregion
    }
}
