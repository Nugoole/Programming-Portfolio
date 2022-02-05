
using ALT.CVL.Log.Interface;

namespace ALT.CVL.Log.Model
{
    internal class Log : ILog
    {
        public string LogDate { get; }
        public string LogTime { get; }
        public string LogLevel { get; }
        public string LogMessage { get; }

        public Log(string logDate, string logTime, string logLevel, string logMessage)
        {
            LogDate = logDate;
            LogTime = logTime;
            LogLevel = logLevel;
            LogMessage = logMessage;
        }
    }
}
