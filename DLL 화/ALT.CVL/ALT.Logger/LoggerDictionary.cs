using ALT.CVL.Log.Enum;
using ALT.CVL.Log.Interface;
using ALT.CVL.Log.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ALT.CVL.Log
{
    /// <summary>
    /// Logger들을 관리하는 Dictionary 객체입니다.
    /// </summary>
    public class LoggerDictionary
    {
        private static readonly Lazy<LoggerDictionary> instance = new Lazy<LoggerDictionary>(() => new LoggerDictionary());
        private Dictionary<string, ILogger> commonLoggerDictionary = new Dictionary<string, ILogger>();


        /// <summary>
        /// LoggerDictionary에 접근하기 위한 Singleton 객체입니다.
        /// </summary>
        public static LoggerDictionary Instance => instance.Value;

        private LoggerDictionary(bool initMainLogs = false)
        {
            if(initMainLogs)
            {
                foreach (var item in System.Enum.GetNames(typeof(MainLogs)))
                {
                    CreateOrGetLogger(item);
                } 
            }

            //var path = Assembly.GetExecutingAssembly().Location;
            //System.Windows.MessageBox.Show(path);

            //using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ALT.CVL.Logger.log4net.dll"))
            //{
            //    var block = new byte[stream.Length];
                
            //    stream.Read(block, 0, block.Length);
            //    using(FileStream fs = new FileStream($"{AppDomain.CurrentDomain.BaseDirectory}\\log4net.dll",FileMode.OpenOrCreate))
            //    {
            //        fs.Write(block, 0, block.Length);
            //    }
            //}
        }


        /// <summary>
        /// LoggerDictionary에서 loggerKey에 대한 Logger를 만들거나 가져옵니다.
        /// </summary>
        /// <param name="loggerKey">
        /// 가져올 Logger의 key
        /// </param>
        /// <returns>
        /// 해당 LoggerKey에 맞는 Logger를 반환합니다.
        /// </returns>
        public ILogger CreateOrGetLogger(string loggerKey, int capacity = 100)
        {
            if (!commonLoggerDictionary.Keys.Contains(loggerKey))
            {
                var newLogger = new Logger(loggerKey, capacity);

                commonLoggerDictionary.Add(loggerKey, newLogger);
            }

            

            return commonLoggerDictionary[loggerKey];
        }

        public ILogger GetMainLogger(MainLogs mainLog)
        {
            if (commonLoggerDictionary.Keys.Contains(mainLog.ToString()))
                return commonLoggerDictionary[mainLog.ToString()];

            return null;
        }
        
    }
}
