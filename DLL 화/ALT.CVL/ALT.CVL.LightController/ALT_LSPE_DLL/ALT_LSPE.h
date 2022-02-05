//===========================================================================================
// ALT_LSPE Series DLL Library
// 2019-08-30
//		Made by woojinauto in ALTSystem 
//===========================================================================================
// History
// 001
// - Receive TimeOut 함수 위치 변경

#ifndef		__ALT_LSPE__
#define		__ALT_LSPE__

//=================================================================================================================================
// Define

#define		CHANNEL_MODE1			(char)0
#define		CHANNEL_MODE2			(char)1
#define		CHANNEL_MODE4			(char)2
#define		CHANNEL_MODE8			(char)3
#define		CHANNEL_MODE16			(char)4
#define		CHANNEL_MODE32			(char)5

#define		MODE1_MAX_CHANNEL		(char)1
#define		MODE2_MAX_CHANNEL		(char)2
#define		MODE4_MAX_CHANNEL		(char)4
#define		MODE8_MAX_CHANNEL		(char)8
#define		MODE16_MAX_CHANNEL		(char)16
#define		MODE32_MAX_CHANNEL		(char)32

#define		MAX_CHANNEL_4			(char)4
#define		MAX_CHANNEL_8			(char)8
#define		MAX_CHANNEL_16			(char)16
#define		MAX_CHANNEL_32			(char)32

#define		MAX_CHANNEL_NUM			MAX_CHANNEL_32

#define		MAX_RUN_CODE			(char)10
#define		MAX_MACRO_DATA_NUM		(int)255

#define		LSPE_PARAMETER_VALUE_ERROR						(DWORD)0x00000100
#define		LSPE_MEM_ALLOCATION_ERROR						(DWORD)0x00000200
#define		LSPE_SYNC_OBJECT_CREATE_ERROR					(DWORD)0x00000300
#define		LSPE_RECEIVE_THREAD_CREATE_ERROR				(DWORD)0x00000400
#define		LSPE_RECEIVE_PROTOCOL_TIMEOUT					(DWORD)0x00000500
#define		LSPE_RECEIVE_NOT_EXIST_COMMAND_ERROR			(DWORD)0x00000600
#define		LSPE_PARAMETER_EXCEPTION_ERROR					(DWORD)0x00000700

#define EXPORT extern "C" __declspec(dllexport)

//=================================================================================================================================
// Init Function

EXPORT int ALT_LSPE_Create ( void ) ;
EXPORT void ALT_LSPE_Close ( int Index ) ;

EXPORT BOOL ALT_LSPE_Lan_Start ( int Index, BOOL TCPIP, char *lpServer, int port, char ChannelNum ) ;
EXPORT BOOL ALT_LSPE_Lan_WStart ( int Index, BOOL TCPIP, wchar_t *lpServer, int port, char ChannelNum ) ;
EXPORT BOOL ALT_LSPE_Uart_Check ( int Index, char *PortName ) ;
EXPORT BOOL ALT_LSPE_Uart_WCheck ( int Index, wchar_t *PortName ) ;
EXPORT BOOL ALT_LSPE_Uart_Start ( int Index, char *PortName, int Borate, char ChannelNum ) ;
EXPORT BOOL ALT_LSPE_Uart_WStart ( int Index, wchar_t *PortName, int Borate, char ChannelNum ) ;
EXPORT BOOL ALT_LSPE_Setup ( int Index, char SegmentNum ) ;
EXPORT unsigned long ALT_LSPE_GetLastErrorCode ( int Index ) ;

//=================================================================================================================================
// Status Function

EXPORT BOOL ALT_LSPE_IsTcpConnect ( int Index ) ;
EXPORT BOOL ALT_LSPE_IsUdpConnect ( int Index ) ;
EXPORT BOOL ALT_LSPE_IsUartConnect ( int Index ) ;

//=================================================================================================================================
// Monitor Protocol Function

EXPORT BOOL ALT_LSPE_Protocol_Load ( int Index, unsigned long *Time, char *DataMem, int *DataSize, BOOL *SendMode ) ;
EXPORT BOOL ALT_LSPE_Protocol_LoadW ( int Index, unsigned long *Time, wchar_t *Data, int *BDataLen, BOOL *SendMode ) ;

//=================================================================================================================================
// Common Protocol Function

EXPORT BOOL ALT_LSPE_SendIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int PortNum ) ;
EXPORT BOOL ALT_LSPE_ReadIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int *PortNum ) ;
EXPORT BOOL ALT_LSPE_SendMACAddress ( int Index, char *MAC ) ;
EXPORT BOOL ALT_LSPE_ReadMACAddress ( int Index, char *MAC ) ;
EXPORT BOOL ALT_LSPE_SendMacroData ( int Index, unsigned char MacroIndex, unsigned char MacroSize, unsigned long *MacroData ) ;
EXPORT BOOL ALT_LSPE_ReadMacroData ( int Index, unsigned char MacroIndex, unsigned char *MacroSize, unsigned long *MacroData ) ;

//=================================================================================================================================
// Device Open Protocol Function

EXPORT BOOL ALT_LSPE_ChannelModeSend ( int Index, char ChannelMode ) ;
EXPORT BOOL ALT_LSPE_ChannelModeRead ( int Index, char *ChannelMode ) ;
EXPORT BOOL ALT_LSPE_SegmentValueSend ( int Index, char PageIndex, char SegIndex, char SegValue ) ;
EXPORT BOOL ALT_LSPE_AllSegmentValueSend ( int Index, char PageIndex, char *SegValue ) ;
EXPORT BOOL ALT_LSPE_AllSegmentValueRead ( int Index, char PageIndex, char *SegValue ) ;
EXPORT BOOL ALT_LSPE_ChannelValueSend ( int Index, char PageIndex, char ChannelIndex, char ChValue ) ;
EXPORT BOOL ALT_LSPE_AllChannelValueSend ( int Index, char PageIndex, char *ChValue ) ; 
EXPORT BOOL ALT_LSPE_AllChannelValueRead ( int Index, char PageIndex, char *ChValue ) ;
EXPORT BOOL ALT_LSPE_AdjustValueSend ( int Index, char AdjustIndex, char AdValue ) ;
EXPORT BOOL ALT_LSPE_AllAdjustValueSend ( int Index, char *AdValue ) ;
EXPORT BOOL ALT_LSPE_AllAdjustValueRead ( int Index, char *AdValue ) ;
EXPORT BOOL ALT_LSPE_SaveSend ( int Index ) ;
EXPORT BOOL ALT_LSPE_LoadSend ( int Index ) ;
EXPORT BOOL ALT_LSPE_RunCodeSend ( int Index, char RunCode ) ;
EXPORT BOOL ALT_LSPE_PageIndexSend ( int Index, char PageIndex ) ;
EXPORT BOOL ALT_LSPE_PageIndexRead ( int Index, char *PageIndex ) ;
EXPORT BOOL ALT_LSPE_ConfigModeRead ( int Index, char *ChannelMode, char *SegmentNum  ) ;

#endif