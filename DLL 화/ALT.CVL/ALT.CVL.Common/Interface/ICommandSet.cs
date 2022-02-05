using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{

    /// <summary>
    /// 비 매크로 통신에 쓰이는 패킷들의 명령어 집합 인터페이스 입니다.
    /// </summary>
    public interface ICommandSet
    {
        /// <summary>
        /// 헤더 부분의 명령어
        /// </summary>
        byte Header { get; set; }

        /// <summary>
        /// 모든 채널에 값을 입력하는 명령어
        /// </summary>
        byte AllChannelWriteCommand { get; set; }
        /// <summary>
        /// 모든 채널의 값을 읽어오는 명령어
        /// </summary>
        byte AllChannelReadCommand { get; set; }

        /// <summary>
        /// 단일 채널의 값을 읽어오는 명령어
        /// </summary>
        byte ChannelWriteCommand { get; set; }

        /// <summary>
        /// EEPROM을 Load하는 명령어
        /// </summary>
        byte EEPROMLoadCommand { get; set; }

        /// <summary>
        /// EEPROM Save하는 명령어
        /// </summary>
        byte EEPROMSaveCommand { get; set; }

        /// <summary>
        /// 명령어 끝부분의 첫부분
        /// </summary>
        byte Tail1 { get; set; }

        /// <summary>
        /// 명령어 끝부분의 둘째부분
        /// </summary>
        byte Tail2 { get; set; }
    }
}
