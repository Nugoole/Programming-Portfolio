using System.Runtime.CompilerServices;
using System.Windows;

namespace ALT.BoltHeight.UI
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}
