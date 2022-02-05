using GalaSoft.MvvmLight;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ALT.TIS.EssenCore.OCRMemory
{
    public class VMEnvironment : ViewModelBase
    {
        #region Variables

        private const int updateInterval = 1000;
        private readonly Task updateTimer;
        private double cpuUsage;
        private double ramUsage;
        private double hddUsage;
        private string currentDateTime;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationToken token;

        #endregion

        #region Properties
        //public IEnumerable<EthernetInfo> NetInfos => netInfos;
        public double CPUUsage { get => cpuUsage; set => Set(ref cpuUsage, value); }
        public double MemoryUsage { get => ramUsage; set => Set(ref ramUsage, value); }
        public double HDDUsage { get => hddUsage; set => Set(ref hddUsage, value); }
        public string CurrentDateTime { get => currentDateTime; set => Set(ref currentDateTime, value); }
        public string CurrentVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        #endregion

        public VMEnvironment()
        {
            token = tokenSource.Token;
            updateTimer = new Task(() =>
            {

                while (!token.IsCancellationRequested)
                {
                    CurrentDateTime = DateTime.Now.ToString("G");

                    CPUUsage = SystemResource.CPUUsage;
                    MemoryUsage = (SystemResource.MemoryInUseMB / SystemResource.TotalMemorySizeMB) * 100;

                    HDDUsage = (SystemResource.TotalHDDSpace - SystemResource.AvailableHDDSpace) / SystemResource.TotalHDDSpace * 100;

                    Task.Delay(updateInterval).Wait();
                }
            }, token);

            updateTimer.Start();
        }

        ~VMEnvironment()
        {
            token.ThrowIfCancellationRequested();
        }
    }
}
