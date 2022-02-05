using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Enum
{
    /// <summary>
    /// 조명 컨트롤러의 에러코드에 대한 Enum입니다.
    /// </summary>
    public enum ErrorCode
    {
        PARAMETER_VALUE_ERROR = 0x00000100,
        MEM_ALLOCATION_ERROR = 0x00000200,
        SYNC_OBJECT_CREATE_ERROR = 0x00000300,
        RECEIVE_THREAD_CREATE_ERROR = 0x00000400,
        RECEIVE_PROTOCOL_TIMEOUT = 0x00000500,
        RECEIVE_NOT_EXIST_COMMAND_ERROR = 0x00000600,
        PARAMETER_EXCEPTION_ERROR = 0x00000700
    }

    /// <summary>
    /// 조명컨트롤러의 모델종류입니다.
    /// </summary>
    public enum ControllerModel
    {
        ERS,
        ESTP,
        LSPE,
        LSTPE,
        TDIVIDER
    }

    /// <summary>
    /// 조명컨트롤러의 TriggerEdge 종류입니다.
    /// </summary>
    public enum TriggerEdge
    {
        RisingEdge,
        FallingEdge
    }

    /// <summary>
    /// 조명컨트롤러에 쓰이는 프로토콜 종류입니다.
    /// </summary>
    public enum ConnectionProtocol
    {
        TCP,
        UDP,
        RS232
    }
}
