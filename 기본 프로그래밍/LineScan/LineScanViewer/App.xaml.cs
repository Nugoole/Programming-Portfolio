using System;
using System.Windows;

namespace LineScanViewer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + AppDomain.CurrentDomain.BaseDirectory + @"MIL_Dlls");
            //AppDomain.CurrentDomain.AppendPrivatePath(AppDomain.CurrentDomain.BaseDirectory + @"MIL_Dlls\");
        }
    }
}
