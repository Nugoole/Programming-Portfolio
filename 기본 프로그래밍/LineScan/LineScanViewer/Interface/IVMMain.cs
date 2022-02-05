using LineScanViewer.Model;

using System.Windows.Controls;
using System.Windows.Input;

namespace LineScanViewer.Interface
{
    public interface IVMMain
    {
        UserControl UCMainView { get; set; }
        ICommand CmdLoadCameraView { get; set; }
        ICommand CmdLoadModelView { get; set; }
        ICommand CmdLoadConfigView { get; set; }
        ICommand CmdLoadStorageView { get; set; }
        ICommand CmdOnClosing { get; set; }
        ICommand CmdOnConnect { get; set; }
        ICommand CmdOnDisConnect { get; set; }
        ICommand CmdOnOpenCamera { get; set; }


        string MenuCategory { get; set; }
    }
}
