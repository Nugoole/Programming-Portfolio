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
    internal class LSPEController : LightControllerBase
    {
        internal static bool CheckSerialPort(string portname)
        {
            int idx = ALT_LSPE_Create();
            bool result = ALT_LSPE_Uart_Check(idx, portname);
            ALT_LSPE_Close(idx);

            return result;
        }

        private const string DLLName = "ALT_LSPE.dll";

        #region P/Invoke Controller DLL
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern int ALT_LSPE_Create();
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern void ALT_LSPE_Close(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_Lan_Start(int index, bool useTCPProtocol, string ipString, int port, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_Uart_Check(int index, string portName);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_Uart_Start(int index, string portName, int baudRate, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_Setup(int index, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern ulong ALT_LSPE_GetLastErrorCode(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_IsTcpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_IsUdpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_IsUartConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_Protocol_Load(int index, ref long time, byte[] dataMem, ref int dataSize, ref bool sendMode);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_SendIPAddress(int index, byte[] ipAddress, byte[] netMask, byte[] gateWay, int portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_ReadIPAddress(int index, byte[] ipAddress, byte[] netMask, byte[] gateWay, int[] portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_SendMACAddress(int index, string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_ReadMACAddress(int index, out string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_SegmentValueSend(int index, byte pageIndex, byte SegmentIndex, byte segValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_AllSegmentValueSend(int index, byte pageIndex, byte[] segValues);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_AllSegmentValueRead(int index, byte pageIndex, out byte[] segValues);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_AdjustValueSend(int index, byte pageIndex, byte adjustIndex, byte adjustValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_AllAdjustValueSend(int index, byte[] adjustValues);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_AllAdjustValueRead(int index, out byte[] adjustValues);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_ChannelValueSend(int index, byte pageIndex, byte channelIndex, byte ChannelValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_AllChannelValueSend(int index, byte pageIndex, byte[] ChannelValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_AllChannelValueRead(int index, byte pageIndex, out byte[] ChannelValue);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_TriggerModeSend(int index, bool triggerMode);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_SaveSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_LoadSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_LSPE_PageIndexSend(int index, byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_LSPE_PageIndexRead(int index, ref byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_LSPE_ConfigModeRead(int index, out byte channelNum, out byte segmentNum);
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

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, ip, buffer1, buffer2, buffer3);

        //        return $"{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_LSPE_SendIPAddress(myControllerIndex, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), netMask, gateWay, port[0]);
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

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, buffer1, subnetMask, buffer2, buffer3);

        //        return $"{subnetMask[0]}.{subnetMask[1]}.{subnetMask[2]}.{subnetMask[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_LSPE_SendIPAddress(myControllerIndex, ip, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), gateWay, port[0]);
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

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, buffer1, buffer2, gateWay, buffer3);

        //        return $"{gateWay[0]}.{gateWay[1]}.{gateWay[2]}.{gateWay[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_LSPE_SendIPAddress(myControllerIndex, ip, netMask, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), port[0]);
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

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, buffer1, buffer2, buffer3, port);

        //        return port[0];
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_LSPE_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_LSPE_SendIPAddress(myControllerIndex, ip, netMask, gateWay, value);
        //        RaisePropertyChanged();
        //    }
        //}
        #endregion
        public override IEnumerable<ILightControllerParameter<int>> Values => values;
        public override TriggerEdge? TriggerEdge { get => null; set { } }
        public override int? Segment
        {
            get
            {
                if (myControllerIndex >= 0)
                {
                    ALT_LSPE_ConfigModeRead(myControllerIndex, out _, out byte segmentNum);
                    CheckError();
                    return segmentNum;
                }

                return -1;
            }
        }

        public override int Channel
        {
            get
            {
                if (myControllerIndex >= 0)
                {
                    ALT_LSPE_ConfigModeRead(myControllerIndex, out byte channelNum,  out _);
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
                    ALT_LSPE_PageIndexRead(myControllerIndex, ref pageIndex);

                CheckError();

                return Convert.ToInt32(pageIndex);
            }
            set
            {
                if (myControllerIndex >= 0)
                {
                    ALT_LSPE_PageIndexSend(myControllerIndex, Convert.ToByte(value));
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


        internal LSPEController() : base()
        {

        }

        public override void ChangeAllValue(int value)
        {
            byte[] values = new byte[Channel];

            for (int i = 0; i < Channel; i++)
            {
                values[i] = (byte)value;
            }


            ALT_LSPE_AllChannelValueSend(myControllerIndex, Convert.ToByte(Page), values);
            CheckError();
        }

        public override void ChangeValue(int channel, int value)
        {
            ALT_LSPE_ChannelValueSend(myControllerIndex, Convert.ToByte(Page), Convert.ToByte(channel), (byte)value);
            CheckError();
        }

        public override bool Connect(ConnectionProtocol protocol, string serialPortName, int baudRate = 9600, string ip = "192.168.10.10", int port = 1000)
        {
            myControllerIndex = ALT_LSPE_Create();
            //CheckError();


            switch (protocol)
            {
                case ConnectionProtocol.TCP:
                    if (!ALT_LSPE_Lan_Start(myControllerIndex, true, ip, port, Convert.ToByte(Segment)))
                        return false;                    
                    break;
                case ConnectionProtocol.UDP:
                    if (!ALT_LSPE_Lan_Start(myControllerIndex, false, ip, port, Convert.ToByte(Segment)))
                        return false;
                    break;
                case ConnectionProtocol.RS232:
                    if (!ALT_LSPE_Uart_Start(myControllerIndex, serialPortName, baudRate, Convert.ToByte(Segment)))
                        return false;
                    break;
            }


            if (!ALT_LSPE_Setup(myControllerIndex, Convert.ToByte(Segment)))
            {
                ALT_LSPE_Close(myControllerIndex);
                return false;
            }

            //채널 개수 읽어오기
            var segment = Segment;


            //채널 개수에 맞는 만큼 Values에 채널 파라미터 추가
            //채널 개수에 맞는 만큼 Values에 채널 파라미터 추가
            for (int i = 0; i < segment; i++)
            {
                values.Add(
                    new LightControllerParameter<int>(
                        //Name
                        "Segment" + (i + 1), i,
                        //Getter
                        (index) =>
                        {
                            var segmentIndex = i;
                            ALT_LSPE_AllSegmentValueRead(myControllerIndex, Convert.ToByte(Page), out byte[] values);
                            return values[segmentIndex];
                        },
                        //Setter
                        (index, setValue) =>
                        {
                            var segmentIndex = index;
                            ALT_LSPE_SegmentValueSend(myControllerIndex, Convert.ToByte(Page), Convert.ToByte(segmentIndex), (byte)setValue);
                        }
                        )
                    );
            }

            for (int i = 0; i < segment; i++)
            {
                values.Add(
                    new LightControllerParameter<int>(
                        //Name
                        "Adjust" + (i + 1), i,
                        //Getter
                        (index) =>
                        {
                            var adjustIndex = i;
                            ALT_LSPE_AllAdjustValueRead(myControllerIndex, out byte[] values);
                            return values[adjustIndex];
                        },
                        //Setter
                        (index, setValue) =>
                        {
                            var adjustIndex = index;
                            ALT_LSPE_ChannelValueSend(myControllerIndex, Convert.ToByte(Page), Convert.ToByte(adjustIndex), (byte)setValue);
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

            ALT_LSPE_Close(myControllerIndex);
        }

        protected override ulong GetLastErrorCode()
        {
            if (myControllerIndex >= 0)
                return ALT_LSPE_GetLastErrorCode(myControllerIndex);

            return 0;
        }
        

    }
}
