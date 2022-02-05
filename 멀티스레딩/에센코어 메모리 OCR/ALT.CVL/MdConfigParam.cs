using ALT.CVL.Interfaces;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace ALT.CVL
{
    public class MdConfigParam : ObservableObject, IConfigParam
    {
        #region Constructor
        public MdConfigParam(string folderPath, string fileName)
        {
            if (!File.Exists(folderPath + @"\" + $"{fileName}.json"))
            {
                RecipePath = AppDomain.CurrentDomain.BaseDirectory;
                NGImgPath = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < CamCount; i++)
                {

                    OCCameraData.Add(new MdCameraData($"CamName{i + 1}"));

                }
            }
            OCWfhDisplay = new MdDisplay().GetOCWfhDisplay(CamCount);
        }
        #endregion

        #region Variables
        private string recipePath;
        private string ngImgPath;
        #endregion

        #region Properties
        public string RecipePath { get => recipePath; set { Set(ref recipePath, value); } }
        public string NGImgPath { get => ngImgPath; set { Set(ref ngImgPath, value); } }
        public int CamCount { get; private set; } = 7;
        public ObservableCollection<MdCameraData> OCCameraData { get; set; } = new ObservableCollection<MdCameraData>();
        public ObservableCollection<WindowsFormsHost> OCWfhDisplay { get; }
        #endregion
    }
}
