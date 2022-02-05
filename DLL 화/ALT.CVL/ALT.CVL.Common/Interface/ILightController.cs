using ALT.CVL.Common.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    /// <summary>
    /// 조명 컨트롤러를 제어하는 인터페이스 객체입니다.
    /// </summary>
    public interface ILightController
    {
        /// <summary>
        /// 조명컨트롤러의 연결정보를 담고있는 파라미터 입니다.
        /// </summary>
        IConnectionInfo ConnectionParam { get; }

        /// <summary>
        /// 조명컨트롤러의 Value값을 조절할 때 즉시 적용할 것인지 나타내는 플래그입니다. True일 때 즉각 적용이 됩니다.
        /// </summary>
        bool UpdateValueImmediately { get; set; }

        /// <summary>
        /// Strobe 타입의 조명 컨트롤러에서 Trigger를 받을 때 Detect할 EdgeMode입니다.
        /// </summary>
        TriggerEdge? TriggerEdge { get; set; }

        /// <summary>
        /// 조명 컨트롤러의 채널 수 입니다.
        /// </summary>
        int Channel { get; }

        /// <summary>
        /// 라인스캔 조명컨트롤러의 Segment입니다.
        /// </summary>
        int? Segment { get; }

        /// <summary>
        /// 조명컨트롤러의 Page 인덱스 입니다. 0 ~ 255
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// 조명컨트롤러의 채널 값들 입니다.
        /// </summary>
        IEnumerable<ILightControllerParameter<int>> Values { get; }


        /// <summary>
        /// 조명컨트롤러에 연결을 시도하는 함수입니다.
        /// </summary>
        /// <param name="protocol">
        /// 조명컨트롤러와 통신할 프로토콜 종류
        /// </param>
        /// <param name="serialPortName">
        /// RS232통신을 선택한 경우, <see cref="ALT.CVL.LightControllerConnector.AvailableSerialPorts"/>에서 포트를 가져와 해당 포트에 연결합니다.
        /// </param>
        /// <param name="baudRate">
        /// RS232통신에 쓰이는 보드레이트 입니다.
        /// </param>
        /// <param name="ip">
        /// TCP/UDP 통신에 쓰이는 IP 주소입니다. 조명 컨트롤러의 IP를 입력하면 됩니다.
        /// </param>
        /// <param name="port">
        /// TCP/UDP 통신에 쓰이는 포트 번호입니다.
        /// </param>
        bool Connect(ConnectionProtocol protocol, string serialPortName = "", int baudRate = 9600, string ip = "192.168.10.10", int port = 1000);

        /// <summary>
        /// 조명컨트롤러와의 연결을 해제합니다.
        /// </summary>
        void Disconnect();


        void ChangeValue(int channel, int value);

        void ChangeAllValue(int value);


        /// <summary>
        /// 페이지가 변경되면 일어나는 이벤트입니다.
        /// </summary>
        event EventHandler<int> OnPageSetCompleted;
        event EventHandler<ErrorCode> OnErrorOccurred;
    }
}
