using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.ControllerClass
{
    internal class ERSController : LightControllerBase, ILightController
    {
        private Stopwatch watch = new Stopwatch();


        internal static bool CheckSerialPort(string portname)
        {
            int idx = ALT_ERS_Create();
            bool result = ALT_ERS_Uart_Check(idx, portname);
            ALT_ERS_Close(idx);

            return result;
        }

        private const string DLLName = "ALT_ERS.dll";

        #region P/Invoke Controller DLL
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern int ALT_ERS_Create();
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern void ALT_ERS_Close(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_Lan_Start(int index, bool useTCPProtocol, string ipString, int port, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_Uart_Check(int index, string portName);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_Uart_Start(int index, string portName, int baudRate, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_Setup(int index, byte channelNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern ulong ALT_ERS_GetLastErrorCode(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_IsTcpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_IsUdpConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_IsUartConnect(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_Protocol_Load(int index, ref long time, byte[] dataMem, ref int dataSize, ref bool sendMode);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_SendIPAddress(int index, byte[] ipAddress, byte[] netMask, byte[] gateWay, int portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_ReadIPAddress(int index, byte[] ipAddress, byte[] netMask, byte[] gateWay, int[] portNum);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_SendMACAddress(int index, string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_ReadMACAddress(int index, out string macAddress);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_ChannelBrightSend(int index, byte pageIndex, byte channelIndex, ushort ChannelBright);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_AllChannelBrightSend(int index, byte pageIndex, ushort[] ChannelBright);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_ERS_AllChannelBrightRead(int index, byte pageIndex, ushort[] ChannelBright);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_SaveSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_LoadSend(int index);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName)]
        private static extern bool ALT_ERS_PageIndexSend(int index, byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_ERS_PageIndexRead(int index, ref byte pageIndex);
        [DefaultDllImportSearchPaths(dllImportSearchPath)]
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ALT_ERS_ConfigModeRead(int index, out byte channelNum);
        #endregion

        private ObservableCollection<ILightControllerParameter<int>> values = new ObservableCollection<ILightControllerParameter<int>>();

        #region 연결 정보 로드 속성들
        //public override string IP
        //{
        //    get
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] buffer1 = new byte[4];
        //        byte[] buffer2 = new byte[4];
        //        int[] buffer3 = new int[1];

        //        ALT_ERS_ReadIPAddress(myControllerIndex, ip, buffer1, buffer2, buffer3);

        //        return $"{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ERS_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ERS_SendIPAddress(myControllerIndex, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), netMask, gateWay, port[0]);
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

        //        ALT_ERS_ReadIPAddress(myControllerIndex, buffer1, subnetMask, buffer2, buffer3);

        //        return $"{subnetMask[0]}.{subnetMask[1]}.{subnetMask[2]}.{subnetMask[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ERS_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ERS_SendIPAddress(myControllerIndex, ip, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), gateWay, port[0]);
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

        //        ALT_ERS_ReadIPAddress(myControllerIndex, buffer1, buffer2, gateWay, buffer3);

        //        return $"{gateWay[0]}.{gateWay[1]}.{gateWay[2]}.{gateWay[3]}";
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ERS_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ERS_SendIPAddress(myControllerIndex, ip, netMask, value.Split('.').Select(x => Convert.ToByte(x)).ToArray(), port[0]);
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

        //        ALT_ERS_ReadIPAddress(myControllerIndex, buffer1, buffer2, buffer3, port);

        //        return port[0];
        //    }
        //    set
        //    {
        //        byte[] ip = new byte[4];
        //        byte[] netMask = new byte[4];
        //        byte[] gateWay = new byte[4];
        //        int[] port = new int[1];

        //        ALT_ERS_ReadIPAddress(myControllerIndex, ip, netMask, gateWay, port);

        //        ALT_ERS_SendIPAddress(myControllerIndex, ip, netMask, gateWay, value);
        //        RaisePropertyChanged();
        //    }
        //}
        #endregion
        public override TriggerEdge? TriggerEdge { get => null; set { } }
        public override int? Segment => null;
        public override IEnumerable<ILightControllerParameter<int>> Values => values;
        public override int Channel
        {
            get
            {
                if (myControllerIndex >= 0)
                {
                    ALT_ERS_ConfigModeRead(myControllerIndex, out byte channelNum);

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
                    ALT_ERS_PageIndexRead(myControllerIndex, ref pageIndex);

                CheckError();

                return Convert.ToInt32(pageIndex);
            }
            set
            {
                if (myControllerIndex >= 0)
                {
                    ALT_ERS_PageIndexSend(myControllerIndex, Convert.ToByte(value));
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



        internal ERSController() : base()
        {

        }


        public override void ChangeAllValue(int value)
        {
            ushort[] values = new ushort[Channel];

            for (int i = 0; i < Channel; i++)
            {
                values[i] = (ushort)value;
            }


            ALT_ERS_AllChannelBrightSend(myControllerIndex, Convert.ToByte(Page), values);
        }

        public override void ChangeValue(int channel, int value)
        {
            ALT_ERS_ChannelBrightSend(myControllerIndex, Convert.ToByte(Page), Convert.ToByte(channel), (ushort)value);
        }

        public override bool Connect(ConnectionProtocol protocol, string serialPortName, int baudRate = 9600, string ip = "192.168.10.10", int port = 1000)
        {
            myControllerIndex = ALT_ERS_Create();
            //CheckError();


            switch (protocol)
            {
                case ConnectionProtocol.TCP:
                    if (!ALT_ERS_Lan_Start(myControllerIndex, true, ip, port, Convert.ToByte(Channel)))
                        return false;
                    break;
                case ConnectionProtocol.UDP:
                    if (!ALT_ERS_Lan_Start(myControllerIndex, false, ip, port, Convert.ToByte(Channel)))
                        return false;
                    break;
                case ConnectionProtocol.RS232:
                    if (!ALT_ERS_Uart_Start(myControllerIndex, serialPortName, baudRate, Convert.ToByte(Channel)))
                        return false;
                    break;
            }

            if (!ALT_ERS_Setup(myControllerIndex, Convert.ToByte(Channel)))
            {
                ALT_ERS_Close(myControllerIndex);
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

                            ALT_ERS_AllChannelBrightRead(myControllerIndex, Convert.ToByte(Page), vals);


                            return vals[channelIndex];
                        },
                        //Setter
                        (index, setValue) =>
                        {
                            var channelIndex = index;
                            //watch.Restart();
                            ALT_ERS_ChannelBrightSend(myControllerIndex, Convert.ToByte(Page), Convert.ToByte(channelIndex), (ushort)setValue);
                            //watch.Stop();
                            //using (var stream = System.IO.File.AppendText(@"C:\Users\MSI\Desktop\delayTest.txt"))
                            //    stream.WriteLine($"{setValue} delay : {watch.ElapsedMilliseconds}ms");
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

            ALT_ERS_Close(myControllerIndex);
            values.Clear();
            connectionParam.IP = string.Empty;
            connectionParam.TCPPort = 0;
        }

        protected override ulong GetLastErrorCode()
        {
            if (myControllerIndex >= 0)
                return ALT_ERS_GetLastErrorCode(myControllerIndex);

            return 0;
        }
    }
}
