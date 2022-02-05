using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Interface;
using ALT.CVL.ControllerClass;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{
    /// <summary>
    /// 조명컨트롤러를 연결하여 객체를 생성하는 클래스입니다.
    /// </summary>
    public static class LightControllerConnector
    {
        /// <summary>
        /// 현재 조명컨트롤러와 연결가능한 시리얼 포트들입니다.
        /// </summary>
        public static List<string> AvailableSerialPorts
        {
            get
            {
                List<string> availablePorts = new List<string>();

                foreach (var portName in System.IO.Ports.SerialPort.GetPortNames())
                {
                    try
                    {
                        if (ERSController.CheckSerialPort(portName))
                        {
                            availablePorts.Add(portName);
                            continue;
                        }

                        if (ESTPController.CheckSerialPort(portName))
                        {
                            availablePorts.Add(portName);
                            continue;
                        }

                        if (LSPEController.CheckSerialPort(portName))
                        {
                            availablePorts.Add(portName);
                            continue;
                        }

                        if (LSTPEController.CheckSerialPort(portName))
                        {
                            availablePorts.Add(portName);
                            continue;
                        }

                        if (TDIVIDERController.CheckSerialPort(portName))
                        {
                            availablePorts.Add(portName);
                            continue;
                        }
                    }
                    catch (DllNotFoundException)
                    {
                        continue;
                    }
                }

                return availablePorts;
            }
        }

        private static Dictionary<string, ILightController> controllerDictionary = new Dictionary<string, ILightController>();

        /// <summary>
        /// 조명컨트롤러에 연결하는 함수입니다.
        /// </summary>
        /// <param name="controllerModel">
        /// 연결하려하는 조명컨트롤러의 종류입니다.
        /// </param>
        /// <param name="protocol">
        /// 연결하려는 프로토콜의 종류입니다.
        /// </param>
        /// <param name="serialPortName">
        /// RS232통신의 경우 연결할 포트이름입니다. <c>ALT.CVL.LightControllerConnector.AvailableSerialPorts</c>에서 가져옵니다.
        /// </param>
        /// <param name="baudRate">
        /// RS232통신에 사용할 보드레이트입니다.
        /// </param>
        /// <param name="ip">
        /// TCP/UDP에 사용될 IP주소입니다.
        /// </param>
        /// <param name="port">
        /// TCP/UDP에 사용될 포트 번호입니다.
        /// </param>
        /// <param name="keyName">
        /// Dictionary에 등록될 Key이름입니다.
        /// </param>
        /// <returns>
        /// ILightController 객체를 반환합니다.
        /// </returns>
        [Description("ERS모델 적용 가능, ESTP, LSPE, LSTPE, TDIVIDER는 현재 기능 X")]
        public static ILightController Connect(ControllerModel controllerModel, ConnectionProtocol protocol, string serialPortName = "", int baudRate = 9600, string ip = "192.168.10.10", int port = 1000, string keyName = null)
        {
            ILightController controller = default;
            string Key = keyName;
            switch (controllerModel)
            {
                case ControllerModel.ERS:
                    if (string.IsNullOrEmpty(Key))
                        Key = KeyNameGenerator("ERS");
                    controller = new ERSController();
                    break;
                case ControllerModel.ESTP:
                    if (string.IsNullOrEmpty(Key))
                        Key = KeyNameGenerator("ESTP");
                    controller = new ESTPController();
                    break;
                case ControllerModel.LSPE:
                    if (string.IsNullOrEmpty(Key))
                        Key = KeyNameGenerator("LSPE");
                    controller = new LSPEController();
                    break;
                case ControllerModel.LSTPE:
                    if (string.IsNullOrEmpty(Key))
                        Key = KeyNameGenerator("LSTPE");
                    controller = new LSTPEController();
                    break;
                case ControllerModel.TDIVIDER:
                    if (string.IsNullOrEmpty(Key))
                        Key = KeyNameGenerator("TDIVIDER");
                    controller = new TDIVIDERController();
                    break;
            }


            

            var isConnectionSuccess = controller.Connect(protocol, serialPortName, baudRate, ip, port);

            if (isConnectionSuccess)
            {
                controller.ConnectionParam.IP = ip;
                controller.ConnectionParam.SubnetMask = "255.255.255.0";
                controller.ConnectionParam.GateWay = "192.168.10.1";
                controller.ConnectionParam.TCPPort = port;
                controller.ConnectionParam.COMPort = serialPortName;
                controller.ConnectionParam.BaudRate = baudRate;
                controller.ConnectionParam.Model = controllerModel;
                controller.ConnectionParam.Protocol = protocol;
                ((ConnectionInfo)controller.ConnectionParam).isChangable = false;

                LightControllerDictionary.innerDictionary.Add(Key, controller);
                ((LightControllerBase)controller).OnDisconnect += LightControllerConnector_OnDisconnect;


                return controller;
            }



            return null;
        }

        public static ILightController Connect(ConnectionInfo conInfo, string keyName = null)
        {
            return Connect(conInfo.Model, conInfo.Protocol, conInfo.COMPort, conInfo.BaudRate, conInfo.IP, conInfo.TCPPort, keyName);
        }

        public static INonMacroLightController Connect(ControllerModel controllerModel, ConnectionProtocol protocol, int channel, string serialPortName = "", int baudRate = 9600, string ip = "192.168.10.10", int port = 1000)
        {
            Non_Macro_Controller controllerBody = new Non_Macro_Controller();
            controllerBody.Protocol = protocol;
            switch (controllerModel)
            {
                case ControllerModel.ERS:
                    controllerBody.CommandSet = new ER4SCommandSet();
                    break;
                case ControllerModel.ESTP:
                    break;
                case ControllerModel.LSPE:
                    break;
                case ControllerModel.LSTPE:
                    break;
                case ControllerModel.TDIVIDER:
                    break;
                default:
                    break;
            }

            controllerBody.Connect(protocol, channel, serialPortName, baudRate, ip, port);

            return controllerBody;
        }

        public static void DisconnectAllControllers()
        {
            int count = LightControllerDictionary.Controllers.Count;

            for (int i = 0; i < count; i++)
            {
                LightControllerDictionary.Controllers.Last().Value.Disconnect();
            }
        }

        private static string KeyNameGenerator(string modelName)
        {
            return modelName + controllerDictionary.Keys.Where(x => x.StartsWith(modelName)).Count();
        }


        

        private static void LightControllerConnector_OnDisconnect(object sender, EventArgs e)
        {
            var key = LightControllerDictionary.Controllers.Where(x => x.Value.Equals(sender)).FirstOrDefault().Key;

            if (!string.IsNullOrEmpty(key))
                LightControllerDictionary.innerDictionary.Remove(key);
        }
    }
}
