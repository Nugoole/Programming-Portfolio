using ALT.CVL.Common.Enum;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.ControllerClass
{
    internal class ESTPController : LightControllerBase
    {
        internal static bool CheckSerialPort(string portname)
        {
            int idx = ALT_ESTP_Create();
            bool result = ALT_ESTP_Uart_Check(idx, portname);
            ALT_ESTP_Close(idx);

            return result;
        }

        private const string DLLName = "ALT_ESTP.dll";

        #region P/Invoke Controller DLL
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern int ALT_ESTP_Create();
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern void ALT_ESTP_Close(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_Lan_Start(int index, bool useTCPProtocol, string ipString, int port, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_Uart_Check(int index, string portName);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_Uart_Start(int index, string portName, int baudRate, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_Setup(int index, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern ulong ALT_ESTP_GetLastErrorCode(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_IsTcpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_IsUdpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_IsUartConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_Protocol_Load(int index, ref long time, byte[] dataMem, ref int dataSize, ref bool sendMode);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_SendIPAddress(int index, byte[] ipAddress, byte[] netMask, byte[] gateWay, int portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_ReadIPAddress(int index, byte[] ipAddress, byte[] netMask, byte[] gateWay, int[] portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_SendMACAddress(int index, string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_ReadMACAddress(int index, out string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_StrobeValueSend(int index, byte pageIndex, byte channelIndex, ushort StrobeValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_AllStrobeValueSend(int index, byte pageIndex, ushort[] StrobeValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_AllStrobeValueRead(int index, byte pageIndex, out ushort[] StrobeValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_TriggerModeSend(int index, bool triggerMode);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_SaveSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_LoadSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ESTP_PageIndexSend(int index, byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_ESTP_PageIndexRead(int index, ref byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_ESTP_ConfigModeRead(int index, out byte channelNum, out bool triggerMode, out bool encoderMode);
        #endregion


        private ObservableCollection<ILightControllerParameter<int>> values = new ObservableCollection<ILightControllerParameter<int>>();

        #region 연결 정보 속성
        //public override string IP
        //{
        //    get
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] buffer1 = new byte[4];
        //        byte[] buffer2 = new byte[4];
        //        int[] buffer3 = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, ip, buffer1, buffer2, buffer3);

        //        return $"{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ESTP_SendIPAddress(myControllerIndex, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), netMask, gateWay, port[0]);
        //        RaisePropertyChanged();
        //    }
        //}
        //public override string SubnetMask
        //{
        //    get
        //    {
        //        byte[] buffer1 = new byte[4];
        //        byte[] subnetMask = new byte[4];
        //        byte[] buffer2 = new byte[4];
        //        int[] buffer3 = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, buffer1, subnetMask, buffer2, buffer3);

        //        return $"{subnetMask[0]}.{subnetMask[1]}.{subnetMask[2]}.{subnetMask[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ESTP_SendIPAddress(myControllerIndex, ip, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), gateWay, port[0]);
        //        RaisePropertyChanged();
        //    }
        //}
        //public override string GateWay
        //{
        //    get
        //    {
        //        byte[] buffer1 = new byte[4];
        //        byte[] buffer2 = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] buffer3 = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, buffer1, buffer2, gateWay, buffer3);

        //        return $"{gateWay[0]}.{gateWay[1]}.{gateWay[2]}.{gateWay[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ESTP_SendIPAddress(myControllerIndex, ip, netMask, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), port[0]);
        //        RaisePropertyChanged();
        //    }
        //}
        //public override int TCPPort
        //{
        //    get
        //    {
        //        byte[] buffer1 = new byte[4];
        //        byte[] buffer2 = new byte[4];
        //        byte[] buffer3 = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, buffer1, buffer2, buffer3, port);

        //        return port[0];
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ESTP_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ESTP_SendIPAddress(myControllerIndex, ip, netMask, gateWay, value);
        //        RaisePropertyChanged();
        //    }
        //}
        #endregion
        public override IEnumerable<ILightControllerParameter<int>> Values => values;
        public override TriggerEdge? TriggerEdge
        {
            get
            {
                if(myControllerIndex >= 0)
                {
                    ALT_ESTP_ConfigModeRead(myControllerIndex, out _, out bool triggerMode, out _);

                    CheckError();

                    if (triggerMode)
                        return Common.Enum.TriggerEdge.RisingEdge;
                    else
                        return Common.Enum.TriggerEdge.FallingEdge;
                }

                return null;
            }
            set
            {
                if(myControllerIndex >= 0)
                {
                    ALT_ESTP_TriggerModeSend(myControllerIndex, value == Common.Enum.TriggerEdge.RisingEdge);
                    CheckError();
                }
            }
        }
        public override int? Segment => null;
        public override int Channel
        {
            get
            {
                if (myControllerIndex >= 0)
                {
                    ALT_ESTP_ConfigModeRead(myControllerIndex, out byte channelNum, out _, out _);
                    CheckError();
                    return channelNum;
                }

                return -1;
            }
        }
        public override int Page
        {
            get
            {
                byte pageIndex = Convert.ToByte(0);

                if (myControllerIndex >= 0)
                    ALT_ESTP_PageIndexRead(myControllerIndex, ref pageIndex);

                CheckError();

                return Convert.ToInt32(pageIndex);
            }
            set
            {
                if (myControllerIndex >= 0)
                {
                    ALT_ESTP_PageIndexSend(myControllerIndex, Convert.ToByte(value));
                    CheckError();
                }

                RaisePropertyChanged();
                foreach (var item in Values)
                {
                    bool temp = item.UpdateImmediately;

                    item.UpdateImmediately = true;
                    item.Refresh();
                    item.UpdateImmediately = temp;
                }

                RaisePageSetCompleted(Page);
            }
        }

        internal ESTPController() : base()
        {

        }
        public override void ChangeAllValue(int value)
        {
            ushort[] values = new ushort[Channel];

            for (int i = 0; i < Channel; i++)
            {
                values[i] = (ushort)value;
            }


            ALT_ESTP_AllStrobeValueSend(myControllerIndex, Convert.ToByte(Page), values);
            CheckError();
        }

        public override void ChangeValue(int channel, int value)
        {
            ALT_ESTP_StrobeValueSend(myControllerIndex, Convert.ToByte(Page), Convert.ToByte(channel), (ushort)value);
            CheckError();
        }

        public override bool Connect(ConnectionProtocol protocol, string serialPortName, int baudRate = 9600, string ip = "192.168.10.10", int port = 1000)
        {
            myControllerIndex = ALT_ESTP_Create();
            //CheckError();


            switch (protocol)
            {
                case ConnectionProtocol.TCP:
                    if (!ALT_ESTP_Lan_Start(myControllerIndex, true, ip, port, Convert.ToByte(Channel)))
                        return false;
                    break;
                case ConnectionProtocol.UDP:
                    if (!ALT_ESTP_Lan_Start(myControllerIndex, false, ip, port, Convert.ToByte(Channel)))
                        return false;
                    break;
                case ConnectionProtocol.RS232:
                    if (!ALT_ESTP_Uart_Start(myControllerIndex, serialPortName, baudRate, Convert.ToByte(Channel)))
                        return false;
                    break;
            }


            if (!ALT_ESTP_Setup(myControllerIndex, Convert.ToByte(Channel)))
            {
                ALT_ESTP_Close(myControllerIndex);
                return false;
            }

            //채널 개수 읽어오기
            var channel = Channel;


            //채널 개수에 맞는 만큼 Values에 채널 파라미터 추가
            for (int i = 0; i < channel; i++)
            {
                values.Add(
                    new LightControllerParameter<int>(
                        //Name
                        "Channel" + (i + 1), i,
                        //Getter
                        (index) =>
                        {
                            var channelIndex = index;
                            ushort[] vals = new ushort[Channel];

                            ALT_ESTP_AllStrobeValueRead(myControllerIndex, Convert.ToByte(Page), out vals);


                            return vals[channelIndex];
                        },
                        //Setter
                        (index, setValue) =>
                        {
                            var channelIndex = index;
                            ALT_ESTP_StrobeValueSend(myControllerIndex, Convert.ToByte(Page), Convert.ToByte(channelIndex), (ushort)setValue);
                        }
                        )
                    );
            }


            
            RaisePropertyChanged(nameof(Page));
            RaisePropertyChanged(nameof(Channel));
            RaisePropertyChanged(nameof(Values));

            UpdateValueImmediately = false;

            return true;
        }

        public override void Disconnect()
        {
            base.Disconnect();

            ALT_ESTP_Close(myControllerIndex);
        }

        protected override ulong GetLastErrorCode()
        {
            if (myControllerIndex >= 0)
                return ALT_ESTP_GetLastErrorCode(myControllerIndex);

            return 0;
        }
    }
}
