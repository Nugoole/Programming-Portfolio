using ALT.CVL.Common.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.ControllerClass
{
    internal class TDIVIDERController : LightControllerBase
    {
        internal static bool CheckSerialPort(string portname)
        {
            int idx = ALT_TDIV_Create();
            bool result = ALT_TDIV_Uart_Check(idx, portname);
            ALT_TDIV_Close(idx);

            return result;
        }

        private const string DLLName = "ALT_TDIV.dll";

        #region P/Invoke Controller DLL
        [DllImport(DLLName)]
        private static extern int ALT_TDIV_Create();
        [DllImport(DLLName)]
        private static extern void ALT_TDIV_Close(int index);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_Lan_Start(int index, bool useTCPProtocol, string ipString, int port, byte channelNum);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_Uart_Check(int index, string portName);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_Uart_Start(int index, string portName, int baudRate, byte channelNum);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_Setup(int index, byte channelNum);
        [DllImport(DLLName)]
        private static extern ulong ALT_TDIV_GetLastErrorCode(int index);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_IsTcpConnect(int index);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_IsUdpConnect(int index);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_IsUartConnect(int index);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_Protocol_Load(int index, ref long time, byte[] dataMem, ref int dataSize, ref bool sendMode);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_SendIPAddress(int index, string ipAddress, string netMask, string gateWay, int portNum);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_ReadIPAddress(int index, out string ipAddress, out string netMask, out string gateWay, out int portNum);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_SendMACAddress(int index, string macAddress);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_ReadMACAddress(int index, out string macAddress);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_ChannelBrightSend(int index, byte pageIndex, byte channelIndex, ushort ChannelBright);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_AllChannelBrightSend(int index, byte pageIndex, ushort[] ChannelBright);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_AllChannelBrightRead(int index, byte pageIndex, out ushort[] ChannelBright);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_TriggerModeSend(int index, bool triggerMode);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_SaveSend(int index);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_LoadSend(int index);
        [DllImport(DLLName)]
        private static extern bool ALT_TDIV_PageIndexSend(int index, byte pageIndex);
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_TDIV_PageIndexRead(int index, ref byte pageIndex);
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_TDIV_ConfigModeRead(int index, out byte channelNum);
        public override int? Segment { get => null; }
        public override int Channel { get => 0; }
        public override int Page { get => 0; set { } }
        public override TriggerEdge? TriggerEdge { get => null; set { } }
        public override IEnumerable<ILightControllerParameter<int>> Values { get => null; }
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
        #endregion

        internal TDIVIDERController() : base()
        {

        }

    }
}
