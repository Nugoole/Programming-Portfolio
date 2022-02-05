using ALT.CVL.Log.Enum;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ALT.CVL.Log.Model
{
    internal class Logger : ALT.CVL.Log.Interface.ILogger, INotifyPropertyChanged
    {
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region New Class
        private const string dateFormat = "MM-dd";
        private const string timeFormat = "HH:mm:ss";
        private RollingFileAppender appender;
        private ILoggerRepository repository;
        private Hierarchy hierarchy;
        private log4net.ILog logger;
        private LimitedObservableCollection<Interface.ILog> logs;
        private string defaultFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}";
        private string defaultFileName = "test";
        private bool isReversed;

        public event PropertyChangedEventHandler PropertyChanged;



        public string FilePath
        {
            get => appender.File;
            set
            {
                defaultFilePath = value;
                ChangeAppenders(CreateAppender());
                RaisePropertyChanged();
            }
        }

        public string DatePattern
        {
            get => appender.DatePattern;
            set
            {
                appender.DatePattern = value;
                ChangeAppenders(appender);
                RaisePropertyChanged();
            }
        }

        public RollingFileAppender.RollingMode RollingMode
        {
            get => appender.RollingStyle;
            set
            {
                appender.RollingStyle = value;
                ChangeAppenders(appender);
                RaisePropertyChanged();
            }
        }

        public Array ArrRollingMode
        {
            get => System.Enum.GetValues(typeof(RollingFileAppender.RollingMode));
        }
        public IReadOnlyCollection<Interface.ILog> Logs { get => logs; }
        public int Capacity { get => logs.Capacity; set => logs.Capacity = value; }
        public bool IsReversed
        {
            get => isReversed; set
            {
                if (isReversed != value)
                {
                    logs = new LimitedObservableCollection<Interface.ILog>(logs.Capacity, logs.Reverse());
                }

                isReversed = value;
            }
        }
        internal Logger(string loggerName = null, int capacity = 100)
        {
            if (!string.IsNullOrEmpty(loggerName))
                defaultFileName = loggerName;



            InitLogManager(defaultFilePath + defaultFileName);

            logs = new LimitedObservableCollection<Interface.ILog>(capacity);
        }

        private void InitLogManager(string repositoryName)
        {
            appender = CreateAppender();
            repository = LogManager.CreateRepository(repositoryName);
            repository.Configured = true;
            hierarchy = repository as Hierarchy;

            // 로그 출력 설정 All 이면 모든 설정이 되고 Info 이면 최하 레벨 Info 위가 설정됩니다.
            hierarchy.Root.Level = log4net.Core.Level.All;
            logger = new LogImpl(hierarchy.GetLogger(repositoryName));
            ChangeAppenders(appender);
        }

        private RollingFileAppender CreateAppender()
        {
            var appender = new RollingFileAppender
            {
                Name = "DefaultAppender",
                File = $"{defaultFilePath}\\[{DateTime.Today:yyyy-MM-dd}]{defaultFileName}.log",

                //Init Layout
                Layout = new PatternLayout("%d [%t] %-5p - %m%n"),

                //change to log file deletion enabled mode
                LockingModel = new FileAppender.MinimalLock()
            };
            return appender;
        }

        private void ChangeAppenders(RollingFileAppender appender)
        {
            if (hierarchy.Root.Appenders.Count > 0)
            {
                hierarchy.Root.Appenders[0].Close();
                hierarchy.Root.RemoveAllAppenders();
            }

            hierarchy.Root.AddAppender(appender);
            appender.ActivateOptions();
        }

        public void Write(LogLevel logLevel, string logMessage)
        {

            // 로그 레벨 순위 입니다.
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    logger.Fatal(logMessage);
                    break;
                case LogLevel.Error:
                    logger.Error(logMessage);
                    break;
                case LogLevel.Warn:
                    logger.Warn(logMessage);
                    break;
                case LogLevel.Info:
                    logger.Info(logMessage);
                    break;
                case LogLevel.Debug:
                    logger.Debug(logMessage);
                    break;
                default:
                    break;
            }

            DateTime timeNow = DateTime.Now;

            if(isReversed)
                logs.Insert(0, new Log(timeNow.Date.ToString(dateFormat), timeNow.ToString(timeFormat), logLevel.ToString(), logMessage));
            else
                logs.Add(new Log(timeNow.Date.ToString(dateFormat), timeNow.ToString(timeFormat), logLevel.ToString(), logMessage));
        }

        public Task<bool> WriteAsync(LogLevel logLevel, string logMessage)
        {
            return Task.Run(() =>
            {
                try
                {
                    Write(logLevel, logMessage);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public void ClearCurrentLogs()
        {
            logs.Clear();
        }


        #endregion
    }
}
