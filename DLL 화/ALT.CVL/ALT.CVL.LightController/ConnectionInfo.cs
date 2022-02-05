using ALT.CVL.Common.Interface;
using ALT.CVL.Common.Enum;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{
    /// <summary>
    /// 컨트롤러의 연결 정보를 담고있는 클래스입니다.
    /// </summary>
    public class ConnectionInfo : INotifyPropertyChanged, IConnectionInfo
    {
        private string iP;
        private string subnetMask;
        private string gateWay;
        private int tCPPort;
        private string cOMPort;
        private int baudRate;
        private ControllerModel model;
        private ConnectionProtocol protocol;

        internal bool isChangable = true;


        /// <summary>
        /// 현재 프로퍼티들을 수정할 수 있는지 나타냅니다.
        /// </summary>
        public bool IsChangable => isChangable;
        /// <summary>
        /// 조명 컨트롤러의 IP주소 입니다.
        /// </summary>
        public string IP
        {
            get => iP; set
            {
                if (IsChangable)
                {
                    iP = value;
                    RaisePropertyChanged();
                }
            }
        }
        /// <summary>
        /// 조명 컨트롤러의 서브넷 마스크 입니다.
        /// </summary>
        public string SubnetMask
        {
            get => subnetMask; set
            {
                if (IsChangable)
                {
                    subnetMask = value;
                    RaisePropertyChanged();
                }
            }
        }
        /// <summary>
        /// 조명 컨트롤러의 GateWay입니다.
        /// </summary>
        public string GateWay
        {
            get => gateWay; set
            {
                if (IsChangable)
                {
                    gateWay = value;
                    RaisePropertyChanged();
                }
            }
        }
        /// <summary>
        /// 조명컨트롤러의 TCP 포트 번호 입니다.
        /// </summary>
        public int TCPPort
        {
            get => tCPPort; set
            {
                if (IsChangable)
                {
                    tCPPort = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 조명컨트롤러의 Serial 포트 이름 입니다.
        /// </summary>
        public string COMPort
        {
            get => cOMPort; set
            {
                if (IsChangable)
                {
                    cOMPort = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 조명컨트롤러의 Serial 통신의 보레이트 입니다.
        /// </summary>
        public int BaudRate
        {
            get => baudRate; set
            {
                if (IsChangable)
                {
                    baudRate = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 조명컨트롤러의 모델 정보입니다.
        /// </summary>
        public ControllerModel Model
        {
            get => model; set
            {
                if (IsChangable)
                {
                    model = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 조명컨트롤러의 연결 프로토콜 종류입니다.
        /// </summary>
        public ConnectionProtocol Protocol
        {
            get => protocol; set
            {
                if (IsChangable)
                {
                    protocol = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
