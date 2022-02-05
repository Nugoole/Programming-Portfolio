using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ALT.TIS.EssenCore.OCRMemory
{
    public static class SystemResource
    {

        private static PerformanceCounter modifiedMemory;
        private static PerformanceCounter cpuCounter;
        private static DriveInfo currentDriveInfo;
        public static string DriveName
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

        public static float TotalHDDSpace => currentDriveInfo.TotalSize;

        public static float AvailableHDDSpace => currentDriveInfo.AvailableFreeSpace;
        public static float CPUUsage
        {
            get
            {

                return (float)((ulong)cpuCounter.NextValue());
            }
        }
        public static float MemoryInUseMB
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

        private static long GetInt64fromPointer(ref IntPtr intPtr)
        {
            return intPtr.ToInt64();
        }

        public static float TotalMemorySizeMB
        {
            get
            {
                GetPhysicallyInstalledSystemMemory(out ulong totalMemory);
                return totalMemory / 1024;
            }
        }

        static SystemResource()
        {
            modifiedMemory = new PerformanceCounter("Memory", "Modified Page List Bytes", true);
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            currentDriveInfo = DriveInfo.GetDrives()[0];
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
