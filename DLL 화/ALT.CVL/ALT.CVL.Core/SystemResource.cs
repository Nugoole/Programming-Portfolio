using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ALT.CVL.Core
{
    /// <summary>
    /// 시스템의 리소스에 대한 값을 불러오는 클래스 입니다.
    /// </summary>
    public class SystemResource : INotifyPropertyChanged
    {
        public event Action OnDateTimer;
            
        private RealTimeTimer timer;
        private PerformanceCounter modifiedMemory;
        private PerformanceCounter cpuCounter;
        private DriveInfo currentDriveInfo;
        private float cpuUsage;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


        /// <summary>
        /// 현재 시간을 반환합니다.
        /// </summary>
        public string CurrentTime => DateTime.Now.ToString("G");
        /// <summary>
        /// 현재 메모리 사용량을 %로 나타냅니다.
        /// </summary>
        public double MemoryUsage => MemoryInUseMB / TotalMemorySizeMB * 100;
        /// <summary>
        /// 현재 하드디스크 저장공간의 사용량을 %로 나타냅니다.
        /// </summary>
        public double HDDUsage => (TotalHDDSpace - AvailableHDDSpace) / TotalHDDSpace * 100;

        /// <summary>
        /// 선택된 드라이브 이름입니다.
        /// </summary>
        public string DriveName
        {
            get
            {
                return currentDriveInfo.Name.Split('\\').First();
            }

            set
            {
                currentDriveInfo = DriveInfo.GetDrives().Where(x => x.Name.Split('\\').First().Equals(value)).First();
            }
        }

        /// <summary>
        /// 현재 하드디스크의 전체 용량입니다.
        /// </summary>
        public float TotalHDDSpace => currentDriveInfo.TotalSize;

        /// <summary>
        /// 현재 하드디스크의 사용가능 용량입니다.
        /// </summary>
        public float AvailableHDDSpace => currentDriveInfo.AvailableFreeSpace;

        /// <summary>
        /// 현재 CPU사용량입니다.
        /// </summary>
        public float CPUUsage
        {
            get
            {
                return cpuUsage;
            }
            private set
            {
                cpuUsage = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 메모리 사용량을 MB단위로 나타냅니다.
        /// </summary>
        public float MemoryInUseMB
        {
            get
            {
                PERFORMANCE_INFORMATION pi = new PERFORMANCE_INFORMATION();
                GetPerformanceInfo(pi, pi.cb);
                var page = GetInt64fromPointer(ref pi.PageSize);
                var total = GetInt64fromPointer(ref pi.PhysicalTotal);
                var available = GetInt64fromPointer(ref pi.PhysicalAvailable);

                var useNmodified = (total - available) * page;

                return (useNmodified - modifiedMemory.RawValue) / 1024 / 1024;
            }
        }

        private long GetInt64fromPointer(ref IntPtr intPtr)
        {
            return intPtr.ToInt64();
        }

        /// <summary>
        /// 컴퓨터의 전체 메모리 크기를 MB로 반환합니다.
        /// </summary>
        public float TotalMemorySizeMB
        {
            get
            {
                GetPhysicallyInstalledSystemMemory(out ulong totalMemory);
                return totalMemory / 1024;
            }
        }



        public SystemResource()
        {
            modifiedMemory = new PerformanceCounter("Memory", "Modified Page List Bytes", true);
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            currentDriveInfo = DriveInfo.GetDrives()[0];

            timer = new RealTimeTimer();
            
            timer.RealTimeSecondTick += Timer_Elapsed;
            Dispatcher.CurrentDispatcher.ShutdownStarted += CurrentDispatcher_ShutdownStarted;

            timer.Start();


        }

        private void CurrentDispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
            }
        }

        private void Timer_Elapsed(object sender, DateTime e)
        {
            CPUUsage = cpuCounter.NextValue();


            RaisePropertyChanged(nameof(CurrentTime));            
            RaisePropertyChanged(nameof(HDDUsage));
            RaisePropertyChanged(nameof(MemoryUsage));
            OnDateTimer?.Invoke();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out ulong MemoryInKilobytes);
        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool GetPerformanceInfo([In, Out] PERFORMANCE_INFORMATION pPerformanceInformation, int cb);



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class PERFORMANCE_INFORMATION
        {
            public int cb;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonpaged;
            public IntPtr PageSize;
            public int HandleCount;
            public int ProcessCount;
            public int ThreadCount;

            public PERFORMANCE_INFORMATION()
            {
                cb = Marshal.SizeOf(typeof(PERFORMANCE_INFORMATION));
            }
        }
    }
}
