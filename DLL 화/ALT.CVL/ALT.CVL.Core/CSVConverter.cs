using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ALT.CVL.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CSVConverter
    {
        private string filePath;

        public string FilePath
        {
            get => filePath; set
            {
                filePath = InitFile(value);
            }
        }



        public CSVConverter(string filePath)
        {
            filePath = InitFile(filePath);

            FilePath = filePath;
        }

        private static string InitFile(string filePath)
        {
            //IsDirectory
            if (Directory.Exists(filePath))
                filePath += "csv.csv";

            //IsFileName without Extension
            if (!filePath.Contains('.'))
                filePath += ".csv";

            if (!File.Exists(filePath))
                using (File.CreateText(filePath)) { }

            return filePath;
        }

        public void Prepend(string csvFormattedString)
        {
            InitFile(FilePath);

            string[] strBuffer = File.ReadAllLines(FilePath);

            string[] strToSave = new string[] { csvFormattedString };

            strToSave.Concat(strBuffer);

            File.WriteAllLines(FilePath, strToSave);
        }


        public void Append(string csvFormmattedString)
        {
            InitFile(FilePath);

            using (StreamWriter sw = File.AppendText(FilePath))
            {
                sw.WriteLine(csvFormmattedString);
            }
        }

        public void Append<T>(T classObject, string[] propertiesToSave = null, bool prependPropertyNames = false) where T : struct
        {
            StringBuilder csvStringBuilder = new StringBuilder();

            if (propertiesToSave == null)
            {
                var propertyNames = typeof(T).GetProperties().Select(x => x.Name);
                if (prependPropertyNames)
                {
                    foreach (var propertyName in propertyNames)
                    {
                        csvStringBuilder.Append(propertyName);
                        csvStringBuilder.Append(',');
                    }

                    csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
                    csvStringBuilder.Append(Environment.NewLine);

                    InitFile(FilePath);
                    if (!File.ReadAllText(FilePath).StartsWith(csvStringBuilder.ToString()))
                        Prepend(csvStringBuilder.ToString());

                    csvStringBuilder.Clear();
                }


                foreach (var propertyName in propertyNames)
                {
                    var value = typeof(T).GetProperty(propertyName).GetValue(classObject).ToString();
                    csvStringBuilder.Append(value);
                    csvStringBuilder.Append(',');
                }

                csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
            }
            else
            {
                if (prependPropertyNames)
                {
                    foreach (var propertyName in propertiesToSave)
                    {
                        csvStringBuilder.Append(propertyName);
                        csvStringBuilder.Append(',');
                    }

                    csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
                    csvStringBuilder.Append(Environment.NewLine);

                    InitFile(FilePath);
                    if (!File.ReadAllText(FilePath).StartsWith(csvStringBuilder.ToString()))
                        Prepend(csvStringBuilder.ToString());

                    csvStringBuilder.Clear();
                }


                foreach (var propertyName in propertiesToSave)
                {
                    var value = typeof(T).GetProperty(propertyName).GetValue(classObject).ToString();
                    csvStringBuilder.Append(value);
                    csvStringBuilder.Append(',');
                }

                csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
            }


            Append(csvStringBuilder.ToString());
        }




        /// <summary>
        /// CSV형식의 문자열을 CSV 파일로 만들어주는 함수입니다.
        /// </summary>
        /// <param name="csvFormattedString">
        /// CSV형식으로 구성된 문자열
        /// </param>
        /// <param name="fileName">
        /// 저장될 파일이름. Null일 시 CSV1.csv와 같이 저장됩니다.
        /// </param>
        public static void StringToCSV(string csvFormattedString, string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, csvFormattedString);
            }
        }

        public static void ClassToCSV<T>(T[] args, string fileName, string[] propertiesToSave = null) where T : class
        {
            StringBuilder csvStringBuilder = new StringBuilder();

            if (propertiesToSave == null)
            {
                var propertyNames = typeof(T).GetProperties().Select(x => x.Name);

                foreach (var propertyName in propertyNames)
                {
                    csvStringBuilder.Append(propertyName);
                    csvStringBuilder.Append(',');
                }

                csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
                csvStringBuilder.Append(Environment.NewLine);

                foreach (var arg in args)
                {
                    foreach (var propertyName in propertyNames)
                    {
                        var value = typeof(T).GetProperty(propertyName).GetValue(arg).ToString();
                        csvStringBuilder.Append(value);
                        csvStringBuilder.Append(',');
                    }

                    csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
                    csvStringBuilder.Append(Environment.NewLine);
                }
            }
            else
            {
                foreach (var propertyName in propertiesToSave)
                {
                    csvStringBuilder.Append(propertyName);
                    csvStringBuilder.Append(',');
                }

                csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
                csvStringBuilder.Append(Environment.NewLine);

                foreach (var arg in args)
                {
                    foreach (var propertyName in propertiesToSave)
                    {
                        var value = typeof(T).GetProperty(propertyName).GetValue(arg).ToString();
                        csvStringBuilder.Append(value);
                        csvStringBuilder.Append(',');
                    }

                    csvStringBuilder.Remove(csvStringBuilder.Length - 1, 1);
                    csvStringBuilder.Append(Environment.NewLine);
                }
            }


            StringToCSV(csvStringBuilder.ToString(), fileName);

        }
    }
}
