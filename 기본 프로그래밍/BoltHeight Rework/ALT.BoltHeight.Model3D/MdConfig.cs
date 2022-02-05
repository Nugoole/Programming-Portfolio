using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ALT.DSCamera
{
    public class MdConfig
    {
        private static MdConfig mdConFig;
        private const string defaultFileName = @"\Config.json";
        private readonly string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string[] DefaultPath;


        public List<string> CamName { get; set; }        
        private MdConfig()
        {
            CamName = new List<string>();
            DefaultPath = new string[]{ StartupPath + @"\Img", StartupPath, StartupPath + @"\Model" };
        }
        public string[] DefaultPathIni()
        {
            if (File.Exists(StartupPath + defaultFileName))
            {
                string LoadpathArr = File.ReadAllText(StartupPath + defaultFileName);
                var pathArr = JsonConvert.DeserializeObject<MdConfig>(LoadpathArr);
                DefaultPath[0] = pathArr.DefaultPath[0];
                DefaultPath[1] = pathArr.DefaultPath[1];
                DefaultPath[2] = pathArr.DefaultPath[2];
                return DefaultPath;
            }
            else
            {

                return DefaultPath;
            }

        }
        public static MdConfig GetMdConfig()
        {
            if (mdConFig == null)
                mdConFig = new MdConfig();
            return mdConFig;
        }
        public static string FolderPathSet()
        {
            using (FolderBrowserDialog FolderDialog = new FolderBrowserDialog())
            {
                if (FolderDialog.ShowDialog() == DialogResult.OK)
                {
                    return FolderDialog.SelectedPath;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
