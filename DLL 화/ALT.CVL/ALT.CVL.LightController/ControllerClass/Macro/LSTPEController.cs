using ALT.CVL.Common.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.ControllerClass
{
    internal class LSTPEController : LightControllerBase
    {
        internal static bool CheckSerialPort(string portname)
        {
            int idx = ALT_LSTPE_Create();
            bool result = ALT_LSTPE_Uart_Check(idx, portname);
            ALT_LSTPE_Close(idx);

            return result;
        }

        private const string DLLName = "ALT_LSTPE.dll";



        #region P/Invoke Controller DLL
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern int ALT_LSTPE_Create();
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern void ALT_LSTPE_Close(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_Lan_Start(int index, bool useTCPProtocol, string ipString, int port, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_Uart_Check(int index, string portName);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_Uart_Start(int index, string portName, int baudRate, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_Setup(int index, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern ulong ALT_LSTPE_GetLastErrorCode(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_IsTcpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_IsUdpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_IsUartConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_Protocol_Load(int index, ref long time, byte[] dataMem, ref int dataSize, ref bool sendMode);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_SendIPAddress(int index, string ipAddress, string netMask, string gateWay, int portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_ReadIPAddress(int index, out string ipAddress, out string netMask, out string gateWay, out int portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_SendMACAddress(int index, string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_ReadMACAddress(int index, out string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_ChannelBrightSend(int index, byte pageIndex, byte channelIndex, ushort ChannelBright);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_AllChannelBrightSend(int index, byte pageIndex, ushort[] ChannelBright);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_AllChannelBrightRead(int index, byte pageIndex, out ushort[] ChannelBright);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_TriggerModeSend(int index, bool triggerMode);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_SaveSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_LoadSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSTPE_PageIndexSend(int index, byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_LSTPE_PageIndexRead(int index, ref byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_LSTPE_ConfigModeRead(int index, out byte channelNum);
        #endregion

        
        public override int? Segment { get => null; }
        public override int Channel { get => 0; }
        public override int Page { get => 0; set { } }
        public override TriggerEdge? TriggerEdge { get => null; set { } }
        public override IEnumerable<ILightControllerParameter<int>> Values { get => null; }

        internal LSTPEController() : base()
        {

        }
        public override void ChangeAllValue(int value)
        {
            throw new NotImplementedException();
        }

        public override void ChangeValue(int channel, int value)
        {
            throw new NotImplementedException();
        }

        public override bool Connect(ConnectionProtocol protocol, string serialPortName, int baudRate = 9600, string ip = "192.168.10.10", int port = 1000)
        {
            throw new NotImplementedException();
        }

        protected override ulong GetLastErrorCode()
        {
            throw new NotImplementedException();
        }
    }
}
