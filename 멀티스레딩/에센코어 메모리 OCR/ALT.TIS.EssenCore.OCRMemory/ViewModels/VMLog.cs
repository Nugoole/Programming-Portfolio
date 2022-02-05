using ALT.Log.Enum;
using ALT.Log.Interface;
using ALT.Log.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ALT.TIS.EssenCore.OCRMemory
{
    public class VMLog : ViewModelBase
    {
        public ObservableCollection<ILog> FileIOLogs => LoggerDictionary.Instance.GetLogger(LogUsage.FileIO).Logs;
        public ObservableCollection<ILog> SystemLogs => LoggerDictionary.Instance.GetLogger(LogUsage.System).Logs;
        public ObservableCollection<ILog> ToolLogs => LoggerDictionary.Instance.GetLogger(LogUsage.Tool).Logs;
        public ObservableCollection<ILog> ComIOLogs => LoggerDictionary.Instance.GetLogger(LogUsage.ComIO).Logs;

        public ObservableCollection<ILog> CapLogs => LoggerDictionary.Instance.GetLogger(Camera.Cap).Logs;
        public ObservableCollection<ILog> FinishLogs => LoggerDictionary.Instance.GetLogger(Camera.Finish).Logs;
        public ObservableCollection<ILog> Level1Logs => LoggerDictionary.Instance.GetLogger(Camera.Level1).Logs;
        public ObservableCollection<ILog> Level2Logs => LoggerDictionary.Instance.GetLogger(Camera.Level2).Logs;
        public ObservableCollection<ILog> Level3Logs => LoggerDictionary.Instance.GetLogger(Camera.Level3).Logs;
        public ObservableCollection<ILog> Level4Logs => LoggerDictionary.Instance.GetLogger(Camera.Level4).Logs;
        public ObservableCollection<ILog> Level5Logs => LoggerDictionary.Instance.GetLogger(Camera.Level5).Logs;

        public ICommand CmdClearFileIOLogs { get; set; }
        public ICommand CmdClearToolLogs { get; set; }
        public ICommand CmdClearComIOLogs { get; set; }
        public ICommand CmdClearSystemLogs { get; set; }





        public ICommand TestCommand { get; set; }

        private ILogger comIOLogger = LoggerDictionary.Instance.GetLogger(LogUsage.ComIO);
        private ILogger systemLogger = LoggerDictionary.Instance.GetLogger(LogUsage.System);
        private ILogger toolLogger = LoggerDictionary.Instance.GetLogger(LogUsage.Tool);
        private ILogger fileIOLogger = LoggerDictionary.Instance.GetLogger(LogUsage.FileIO);
        private ILogger CapLogger = LoggerDictionary.Instance.GetLogger(Camera.Cap);
        private ILogger FinishLogger = LoggerDictionary.Instance.GetLogger(Camera.Finish);
        private ILogger Level1Logger = LoggerDictionary.Instance.GetLogger(Camera.Level1);
        private ILogger Level2Logger = LoggerDictionary.Instance.GetLogger(Camera.Level2);
        private ILogger Level3Logger = LoggerDictionary.Instance.GetLogger(Camera.Level3);
        private ILogger Level4Logger = LoggerDictionary.Instance.GetLogger(Camera.Level4);
        private ILogger Level5Logger = LoggerDictionary.Instance.GetLogger(Camera.Level5);
        public VMLog()
        {
            CmdClearComIOLogs = new RelayCommand(() => LoggerDictionary.Instance.GetLogger(LogUsage.ComIO).Logs.Clear());
            CmdClearFileIOLogs = new RelayCommand(() => LoggerDictionary.Instance.GetLogger(LogUsage.FileIO).Logs.Clear());
            CmdClearSystemLogs = new RelayCommand(() => LoggerDictionary.Instance.GetLogger(LogUsage.System).Logs.Clear());
            CmdClearToolLogs = new RelayCommand(() => LoggerDictionary.Instance.GetLogger(LogUsage.Tool).Logs.Clear());

            //RunRandomLogger(comIOLogger);
            //RunRandomLogger(systemLogger);
            //RunRandomLogger(toolLogger);
            //RunRandomLogger(fileIOLogger);
            //RunRandomLogger(CapLogger);
            //RunRandomLogger(FinishLogger);
            //RunRandomLogger(Level1Logger);
            //RunRandomLogger(Level2Logger);
            //RunRandomLogger(Level3Logger);
            //RunRandomLogger(Level4Logger);
            //RunRandomLogger(Level5Logger);


        }

        private void RunRandomLogger(ILogger logger)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    logger.Write(LogLevel.Info, DateTime.Now.ToString("mm:ss"));

                    Random random = new Random(DateTime.Now.Millisecond);
                    int delay = random.Next(10) * 100;

                    Task.Delay(delay).Wait();
                }
            });
        }
    }
}
