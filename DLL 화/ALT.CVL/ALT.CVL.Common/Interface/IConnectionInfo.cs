using System.ComponentModel;

using ALT.CVL.Common.Enum;

namespace ALT.CVL.Common.Interface
{
    public interface IConnectionInfo
    {
        int BaudRate { get; set; }
        string COMPort { get; set; }
        string GateWay { get; set; }
        string IP { get; set; }
        bool IsChangable { get; }
        ControllerModel Model { get; set; }
        ConnectionProtocol Protocol { get; set; }
        string SubnetMask { get; set; }
        int TCPPort { get; set; }
    }
}