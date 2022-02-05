using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALT.CVL.ControllerClass
{
    internal class Non_Macro_Controller : INonMacroLightController, INotifyPropertyChanged
    {
        private ObservableCollection<LightControllerParameter<byte>> values = new ObservableCollection<LightControllerParameter<byte>>();


        //Serial
        private SerialPort serial;
        private int channelCount = 0;
        private readonly List<byte> tx_arr;
        private static byte[] receiveArr;
        private static List<byte> receive_tx_arr;
        private string serialPortName;
        private int baudrate;

        //Tcp
        private TcpClient client;
        private NetworkStream clientStream;
        private Thread ReceiveThread;
        private string ip;
        private int tcpPort;
        

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommandSet CommandSet { get; set; }
        public string IP => ip;
        public string COMPort => serialPortName; 
        public int Channel => channelCount;
        public int TCPPort => tcpPort;
        public IEnumerable<ILightControllerParameter<byte>> Values => values;
        public ConnectionProtocol Protocol { get; internal set; }
        public int BaudRate => baudrate;

        public void ChangeAllValue(byte[] values)
        {
            //Make Send Packet
            List<byte> packet = new List<byte>();
            packet.Add(CommandSet.Header);
            packet.Add(CommandSet.AllChannelWriteCommand);
            for (int i = 0; i < channelCount; i++)
            {
                packet.Add(values[i]);
            }

            byte checkSum = MakeCheckSum(packet.Skip(1).ToList());
            packet.Add(checkSum);
            packet.Add(CommandSet.Tail1);
            packet.Add(CommandSet.Tail2);
            //done


            if (Protocol == ConnectionProtocol.RS232)
            {
                if (serial.IsOpen)
                {
                    serial.Write(packet.ToArray(), 0, packet.Count);

                    //serial.


                }
            }
            else
            {
                if (client.Connected)
                {
                    clientStream.Write(packet.ToArray(), 0, packet.Count);


                    while (client.Available == 0)
                    {

                    }

                    var buffer = new byte[client.Available];

                    clientStream.Read(buffer, 0, buffer.Length);

                    if (!buffer.Take(packet.Count).SequenceEqual(packet))
                        throw new Exception("Change All Value Failed");
                }
            }
        }

        public void ChangeValue(int channel, byte value)
        {
            //Make Send Packet
            List<byte> packet = new List<byte>();
            packet.Add(CommandSet.Header);
            packet.Add(CommandSet.ChannelWriteCommand);
            packet.Add(Convert.ToByte(channel));
            packet.Add(value);

            byte checkSum = MakeCheckSum(packet.Skip(1).ToList());
            packet.Add(checkSum);
            packet.Add(CommandSet.Tail1);
            packet.Add(CommandSet.Tail2);
            //done


            if (Protocol == ConnectionProtocol.RS232)
            {
                if (serial.IsOpen)
                {
                    serial.Write(packet.ToArray(), 0, packet.Count);

                    //serial.


                }
            }
            else
            {
                if (client.Connected)
                {
                    clientStream.Write(packet.ToArray(), 0, packet.Count);


                    while (client.Available == 0)
                    {

                    }

                    var buffer = new byte[client.Available];

                    clientStream.Read(buffer, 0, buffer.Length);

                    if (!buffer.Take(packet.Count).SequenceEqual(packet))
                        throw new Exception("Change Single Value Failed");
                }
            }
        }

        public void Connect(ConnectionProtocol protocol, int channel, string serialPortName = "", int baudRate = 9600, string ip = "192.168.10.10", int port = 1000)
        {
            channelCount = channel;

            if (protocol == ConnectionProtocol.RS232)
            {
                if (serial == null)
                {
                    serial = new SerialPort(serialPortName, baudRate);
                    serial.Open();

                    if (serial.IsOpen)
                        this.serialPortName = serialPortName;
                }
            }
            else if(protocol == ConnectionProtocol.TCP)
            {
                if(client == null)
                {
                    client = new TcpClient(ip, port);
                    
                    if(client.Connected)
                    {
                        clientStream = client.GetStream();
                    }
                }
            }

            for (int i = 0; i < channel; i++)
            {
                values.Add(
                    new LightControllerParameter<byte>(
                        //Name
                        "Channel" + (i + 1), i,
                        //Getter
                        (index) =>
                        {
                            var values = ReadAllValue();
                            return values[index];
                        },
                        //Setter
                        (index, setValue) =>
                        {
                            
                            ChangeValue(index, setValue);
                        }
                        )
                    );

                if (protocol == ConnectionProtocol.RS232)
                    Task.Delay(10).Wait();
            }

            foreach (var val in values)
            {
                val.UpdateImmediately = false;
            }

            RaisePropertyChanged(nameof(COMPort));
            RaisePropertyChanged(nameof(BaudRate));
            RaisePropertyChanged(nameof(Channel));
            RaisePropertyChanged(nameof(Values));
        }

        public void Disconnect()
        {
            if(Protocol == ConnectionProtocol.RS232)
            {
                if(serial != null && serial.IsOpen)
                {
                    if(serial.BytesToRead > 0)
                    {
                        serial.DiscardOutBuffer();
                        serial.DiscardInBuffer();
                    }

                    serial.Close();
                    serial.Dispose();
                    serial = null;
                }
            }
            else if(Protocol == ConnectionProtocol.TCP)
            {
                if(client != null && client.Connected)
                {
                    client.Close();
                    client.Dispose();
                    client = null;
                }
            }
        }

        

        private byte MakeCheckSum(List<byte> list)
        {
            var checkSumByte = list[0];

            for (int i = 1; i < list.Count; i++)
            {
                checkSumByte = (byte)(checkSumByte ^ list[i]);
            }

            return checkSumByte;
        }

        private List<byte> ReadAllValue()
        {
            //Making Command Packet
            var sendPacket = new List<byte>();
            sendPacket.Add(CommandSet.Header);
            sendPacket.Add(CommandSet.AllChannelReadCommand);


            byte checkSum = MakeCheckSum(sendPacket.Skip(1).ToList());
            sendPacket.Add(checkSum);
            sendPacket.Add(CommandSet.Tail1);
            sendPacket.Add(CommandSet.Tail2);
            //Done

            if(Protocol == ConnectionProtocol.RS232)
            {
                if(serial != null && serial.IsOpen)
                {
                    serial.Write(sendPacket.ToArray(), 0, sendPacket.Count);

                    while (serial.BytesToRead < 2 + channelCount + 3)
                    {

                    }

                    var buffer = new byte[2 + channelCount + 3];

                    serial.Read(buffer, 0, buffer.Length);

                    return buffer.Skip(2).Take(buffer.Length - 5).ToList();
                }

            }
            else if(Protocol == ConnectionProtocol.TCP)
            {
                if (client != null && client.Connected)
                {
                    clientStream.Write(sendPacket.ToArray(), 0, sendPacket.Count);


                    while (client.Available == 0)
                    {

                    }

                    var buffer = new byte[client.Available];

                    clientStream.Read(buffer, 0, buffer.Length);

                    return buffer.Skip(2).Take(buffer.Length - 5).ToList();
                }
            }
            

            return null;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
