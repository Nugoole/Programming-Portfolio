//===========================================================================================
// ALT_ERS Series DLL Library
// 2019-09-09
//		Made by woojinauto in ALTSystem 
//===========================================================================================
// History
// 001
// - Receive TimeOut 함수 위치 변경

#ifndef		__ALT_ERS__
#define		__ALT_ERS__

//=================================================================================================================================
// Define

#define		MAX_CHANNEL_4			(char)4
#define		MAX_CHANNEL_8			(char)8
#define		MAX_CHANNEL_16			(char)16
#define		MAX_CHANNEL_32			(char)32
#define		MAX_CHANNEL_64			(char)64

#define		MAX_RUN_CODE			(char)10
#define		MAX_MACRO_DATA_NUM		(int)255

#define		ERS_PARAMETER_VALUE_ERROR						(DWORD)0x00000100
#define		ERS_MEM_ALLOCATION_ERROR						(DWORD)0x00000200
#define		ERS_SYNC_OBJECT_CREATE_ERROR					(DWORD)0x00000300
#define		ERS_RECEIVE_THREAD_CREATE_ERROR					(DWORD)0x00000400
#define		ERS_RECEIVE_PROTOCOL_TIMEOUT					(DWORD)0x00000500
#define		ERS_RECEIVE_NOT_EXIST_COMMAND_ERROR				(DWORD)0x00000600
#define		ERS_PARAMETER_EXCEPTION_ERROR					(DWORD)0x00000700

#define EXPORT extern "C" __declspec(dllexport)

//=================================================================================================================================
// Init Function

EXPORT int ALT_ERS_Create ( void ) ;
EXPORT void ALT_ERS_Close ( int Index ) ;

EXPORT BOOL ALT_ERS_Lan_Start ( int Index, BOOL TCPIP, char *lpServer, int port, char ChannelNum ) ;
EXPORT BOOL ALT_ERS_Lan_WStart ( int Index, BOOL TCPIP, wchar_t *lpServer, int port, char ChannelNum ) ;
EXPORT BOOL ALT_ERS_Uart_Check ( int Index, char *PortName ) ;
EXPORT BOOL ALT_ERS_Uart_WCheck ( int Index, wchar_t *PortName ) ;
EXPORT BOOL ALT_ERS_Uart_Start ( int Index, char *PortName, int Borate, char ChannelNum ) ;
EXPORT BOOL ALT_ERS_Uart_WStart ( int Index, wchar_t *PortName, int Borate, char ChannelNum ) ;
EXPORT BOOL ALT_ERS_Setup ( int Index, char ChannelNum ) ;
EXPORT unsigned long ALT_ERS_GetLastErrorCode ( int Index ) ;

//=================================================================================================================================
// Status Function

EXPORT BOOL ALT_ERS_IsTcpConnect ( int Index ) ;
EXPORT BOOL ALT_ERS_IsUdpConnect ( int Index ) ;
EXPORT BOOL ALT_ERS_IsUartConnect ( int Index ) ;

//=================================================================================================================================
// Monitor Protocol Function

EXPORT BOOL ALT_ERS_Protocol_Load ( int Index, unsigned long *Time, char *DataMem, int *DataSize, BOOL *SendMode ) ;
EXPORT BOOL ALT_ERS_Protocol_LoadW ( int Index, unsigned long *Time, wchar_t *Data, int *BDataLen, BOOL *SendMode ) ;

//=================================================================================================================================
// Common Protocol Function

EXPORT BOOL ALT_ERS_SendIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int PortNum ) ;
EXPORT BOOL ALT_ERS_ReadIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int *PortNum ) ;
EXPORT BOOL ALT_ERS_SendMACAddress ( int Index, char *MAC ) ;
EXPORT BOOL ALT_ERS_ReadMACAddress ( int Index, char *MAC ) ;
EXPORT BOOL ALT_ERS_SendMacroData ( int Index, unsigned char MacroIndex, unsigned char MacroSize, unsigned long *MacroData ) ;
EXPORT BOOL ALT_ERS_ReadMacroData ( int Index, unsigned char MacroIndex, unsigned char *MacroSize, unsigned long *MacroData ) ;

//=================================================================================================================================
// Device Open Protocol Function

EXPORT BOOL ALT_ERS_ChannelBrightSend ( int Index, char PageIndex, char ChannelIndex, unsigned short BrightValue ) ;
EXPORT BOOL ALT_ERS_AllChannelBrightSend ( int Index, char PageIndex, unsigned short *BrightValue ) ;
EXPORT BOOL ALT_ERS_AllChannelBrightRead ( int Index, char PageIndex, unsigned short *BrightValue ) ;
EXPORT BOOL ALT_ERS_SaveSend ( int Index ) ;
EXPORT BOOL ALT_ERS_LoadSend ( int Index ) ;
EXPORT BOOL ALT_ERS_RunCodeSend ( int Index, char RunCode ) ;
EXPORT BOOL ALT_ERS_PageIndexSend ( int Index, char PageIndex ) ;
EXPORT BOOL ALT_ERS_PageIndexRead ( int Index, char *PageIndex ) ;
EXPORT BOOL ALT_ERS_ConfigModeRead ( int Index, char *ChannelNum ) ;

#endif