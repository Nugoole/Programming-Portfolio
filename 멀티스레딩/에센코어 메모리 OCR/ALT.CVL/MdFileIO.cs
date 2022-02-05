using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ALT.CVL
{
    public class MdFileIO
    {
        #region Singleton Constructor
        private static readonly Lazy<MdFileIO> instance = new Lazy<MdFileIO>(() => new MdFileIO());

        private MdFileIO()
        {

        }
        public static MdFileIO Getinstance() => instance.Value;
        #endregion

        #region Variables

        #endregion

        #region Properties

        #endregion

        #region Functions
     
        public void AddFolder(string folderPath, string folderName)
        {
            var modelPath = $@"{folderPath}\{folderName}";
            CreateDirectory(modelPath);
        }
        public void DeleteFolder(string folderPath, string folderName)
        {
            var modelPath = $@"{folderPath}\{folderName}";
            DeleteDirectory(modelPath);
        }
        public string DlgFolderPathSet()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.IsFolderPicker = true;
                var result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok) { return dialog.FileName; }
                else return null;
            }
        }
        public void DirectoryOpen(string folderPath)
        {
            if (Directory.Exists(folderPath))
                Process.Start(folderPath);
        }
        public void JsonFileSave(object _object, string folderPath, string fileName)
        {
            string sJson = JsonConvert.SerializeObject(_object, Formatting.Indented);

            if (string.IsNullOrEmpty(folderPath))
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + $"{fileName}.json", sJson);
            else
            {
                CreateDirectory(folderPath);
                File.WriteAllText(folderPath + $@"\{fileName}.json", sJson);
            }
        }
        public static T JsonFileLoad<T>(string folderPath, string fileName) where T : class
        {
            T data;
            if (File.Exists(folderPath + @"\" + $"{fileName}.json"))
            {
                string sJson = File.ReadAllText(folderPath + @"\" + fileName + ".json");
                data = JsonConvert.DeserializeObject<T>(sJson);
                return data;
            }
            else
            {
                return null;
            }
        }
        public void SaveTools(IEnumerable<CogToolBase> ocToolBase, string folderPath)
        {
            string toolPath = $@"{folderPath}\Tools";
            CreateDirectory(toolPath);
            foreach (var tool in ocToolBase)
            {
                CogSerializer.SaveObjectToFile(tool, $@"{toolPath}\{tool.Name}.vpp");
            }
        }
        public void ToolFileListSet(MdToolParams toolInfo, string folderPath)
        {
            string toolPath = $@"{folderPath}\Tools";
            DirectoryInfo info = new DirectoryInfo(toolPath);
            if (info.Exists)
            {
                toolInfo.OCToolBase.Clear();
                foreach (var toolFile in info.GetFiles())
                {
                    toolInfo.OCToolBase.Add(CogSerializer.LoadObjectFromFile(toolFile.FullName) as CogToolBase);
                }
            }
        }
        public T LoadTool<T>(IEnumerable<CogToolBase> ocToolBase, CogToolBase t) where T : class
        {
            T toolData = null;
            var tmpBase = ToolNameCheck(ocToolBase, t.Name);
            if (tmpBase.GetType() == t.GetType())
            {
                toolData = tmpBase as T;
            }
            return toolData;
        }

        private CogToolBase ToolNameCheck(IEnumerable<CogToolBase> ocToolBase, string toolName)
        {
            CogToolBase toolBase = null;
            foreach (var data in ocToolBase)
            {
                if (data.Name == toolName)
                {
                    toolBase = data;
                }
            }
            return toolBase;
        }

        private void CreateDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }
        private void DeleteDirectory(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }
        public DirectoryInfo GetDirectoryInfo(string folderPath)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            return info;
        }
        #endregion
    }
}
