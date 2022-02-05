using ALT.CVL.Common.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    /// <summary>
    /// 비 매크로 버전의 컨트롤러를 제어하는 객체입니다.
    /// </summary>
    public interface INonMacroLightController
    {
        /// <summary>
        /// 컨트롤러의 TCP 프로토콜 IP
        /// </summary>
        string IP { get; }
        
        /// <summary>
        /// 모델에 맞는 명령어 셋입니다.
        /// </summary>
        ICommandSet CommandSet { get; set; }

        /// <summary>
        /// 컨트롤러의 COMPort
        /// </summary>
        string COMPort { get; }

        /// <summary>
        /// 컨트롤러의 BaudRate
        /// </summary>
        int BaudRate { get; }

        /// <summary>
        /// 컨트롤러의 채널 수
        /// </summary>
        int Channel { get; }

        /// <summary>
        /// 컨트롤러의 TCP포트 번호
        /// </summary>
        int TCPPort { get; }

        /// <summary>
        /// 연결 프로토콜 방식
        /// </summary>
        ConnectionProtocol Protocol { get; }

        /// <summary>
        /// 채널 값들을 조절하는 파라미터 배열
        /// </summary>
        IEnumerable<ILightControllerParameter<byte>> Values { get; }

        /// <summary>
        /// 컨트롤러에 연결하는 함수입니다.
        /// </summary>
        /// <param name="protocol">
        /// 연결 프로토콜
        /// </param>
        /// <param name="channel">
        /// 채널 수
        /// </param>
        /// <param name="serialPortName">
        /// 연결 프로토콜이 Serial일 때 Serial포트 이름
        /// </param>
        /// <param name="baudRate">
        /// Serial 연결 프로토콜의 baudRate
        /// </param>
        /// <param name="ip">
        /// 연결 프로토콜이 TCP일 때 컨트롤러의 IP, default : 192.168.10.10
        /// </param>
        /// <param name="port">
        /// 연결 프로토콜이 TCP일 때 컨트롤러의 Port번호, default : 1000
        /// </param>
        void Connect(ConnectionProtocol protocol, int channel, string serialPortName = "", int baudRate = 9600, string ip = "192.168.10.10", int port = 1000);

        /// <summary>
        /// 컨트롤러를 연결해제합니다.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 모든 채널의 값을 입력한 value로 변경합니다.
        /// </summary>
        /// <param name="value">
        /// 바꾸려는 값
        /// </param>
        void ChangeAllValue(byte[] value);

        /// <summary>
        /// 단일 채널의 값을 입력한 value로 변경합니다.
        /// </summary>
        /// <param name="channel">
        /// 바꾸려는 채널의 번호
        /// </param>
        /// <param name="value">
        /// 바꾸려는 값
        /// </param>
        void ChangeValue(int channel, byte value);
    }
}
