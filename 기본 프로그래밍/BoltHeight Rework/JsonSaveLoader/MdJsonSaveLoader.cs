using Newtonsoft.Json;

using System;
using System.IO;
using System.Reflection;

namespace JsonSaveLoader
{
    public class MdJsonSaveLoader
    {
        public static string defaultConfigName = "Config3D.Json";
        public static void Save(object _object, string filePath, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = defaultConfigName;

            string sJson = JsonConvert.SerializeObject(_object, Formatting.Indented);

            if (string.IsNullOrEmpty(filePath))
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + fileName, sJson);
            else
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                File.WriteAllText(filePath + $@"\{fileName}", sJson);
            }

        }
        public static T Load<T>(string FilePath) where T : class
        {
            string path;
            T data;
            if (string.IsNullOrEmpty(FilePath))
            {
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + defaultConfigName;
            }
            else
            {
                path = FilePath;
            }
            if (File.Exists(path))
            {
                string sJson = File.ReadAllText(path);
                try
                {
                    data = JsonConvert.DeserializeObject<T>(sJson);
                    return data;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
