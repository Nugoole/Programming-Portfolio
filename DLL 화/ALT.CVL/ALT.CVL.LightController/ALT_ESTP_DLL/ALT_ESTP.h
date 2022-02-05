//===========================================================================================
// ALT_ERS Series DLL Library
// 2019-09-10
//		Made by woojinauto in ALTSystem 
//===========================================================================================
// History
// 001
// - Receive TimeOut 함수 위치 변경

#ifndef		__ALT_ESTP__
#define		__ALT_ESTP__

//=================================================================================================================================
// Define

#define		MAX_CHANNEL_4			(char)4
#define		MAX_CHANNEL_8			(char)8
#define		MAX_CHANNEL_16			(char)16
#define		MAX_CHANNEL_32			(char)32
#define		MAX_CHANNEL_64			(char)64

#define		MAX_RUN_CODE			(char)10
#define		MAX_MACRO_DATA_NUM		(int)255

#define		ENC_ND_1_1				(char)0
#define		ENC_ND_1_2				(char)1
#define		ENC_ND_1_4				(char)2
#define		ENC_D_1_1				(char)3
#define		ENC_D_1_2				(char)4
#define		ENC_D_1_4				(char)5

#define		ESTP_PARAMETER_VALUE_ERROR						(DWORD)0x00000100
#define		ESTP_MEM_ALLOCATION_ERROR						(DWORD)0x00000200
#define		ESTP_SYNC_OBJECT_CREATE_ERROR					(DWORD)0x00000300
#define		ESTP_RECEIVE_THREAD_CREATE_ERROR				(DWORD)0x00000400
#define		ESTP_RECEIVE_PROTOCOL_TIMEOUT					(DWORD)0x00000500
#define		ESTP_RECEIVE_NOT_EXIST_COMMAND_ERROR			(DWORD)0x00000600
#define		ESTP_PARAMETER_EXCEPTION_ERROR					(DWORD)0x00000700

#define		MAX_ENC_OFFSET_LIST_DATA		(unsigned short)256

#define EXPORT extern "C" __declspec(dllexport)

//=================================================================================================================================
// Init Function

EXPORT int ALT_ESTP_Create ( void ) ;
EXPORT void ALT_ESTP_Close ( int Index ) ;

EXPORT BOOL ALT_ESTP_Lan_Start ( int Index, BOOL TCPIP, char *lpServer, int port, char ChannelNum ) ;
EXPORT BOOL ALT_ESTP_Lan_WStart ( int Index, BOOL TCPIP, wchar_t *lpServer, int port, char ChannelNum ) ;
EXPORT BOOL ALT_ESTP_Uart_Check ( int Index, char *PortName ) ;
EXPORT BOOL ALT_ESTP_Uart_WCheck ( int Index, wchar_t *PortName ) ;
EXPORT BOOL ALT_ESTP_Uart_Start ( int Index, char *PortName, int Borate, char ChannelNum ) ;
EXPORT BOOL ALT_ESTP_Uart_WStart ( int Index, wchar_t *PortName, int Borate, char ChannelNum ) ;
EXPORT BOOL ALT_ESTP_Setup ( int Index, char ChannelNum ) ;
EXPORT unsigned long ALT_ESTP_GetLastErrorCode ( int Index ) ;

//=================================================================================================================================
// Status Function

EXPORT BOOL ALT_ESTP_IsTcpConnect ( int Index ) ;
EXPORT BOOL ALT_ESTP_IsUdpConnect ( int Index ) ;
EXPORT BOOL ALT_ESTP_IsUartConnect ( int Index ) ;

//=================================================================================================================================
// Monitor Protocol Function

EXPORT BOOL ALT_ESTP_Protocol_Load ( int Index, unsigned long *Time, char *DataMem, int *DataSize, BOOL *SendMode ) ;
EXPORT BOOL ALT_ESTP_Protocol_LoadW ( int Index, unsigned long *Time, wchar_t *Data, int *BDataLen, BOOL *SendMode ) ;

//=================================================================================================================================
// Common Protocol Function

EXPORT BOOL ALT_ESTP_SendIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int PortNum ) ;
EXPORT BOOL ALT_ESTP_ReadIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int *PortNum ) ;
EXPORT BOOL ALT_ESTP_SendMACAddress ( int Index, char *MAC ) ;
EXPORT BOOL ALT_ESTP_ReadMACAddress ( int Index, char *MAC ) ;
EXPORT BOOL ALT_ESTP_SendMacroData ( int Index, unsigned char MacroIndex, unsigned char MacroSize, unsigned long *MacroData ) ;
EXPORT BOOL ALT_ESTP_ReadMacroData ( int Index, unsigned char MacroIndex, unsigned char *MacroSize, unsigned long *MacroData ) ;

//=================================================================================================================================
// Device Open Protocol Function

EXPORT BOOL ALT_ESTP_StrobeValueSend ( int Index, char PageIndex, char ChannelIndex, unsigned short StrobeValue ) ;
EXPORT BOOL ALT_ESTP_AllStrobeValueSend ( int Index, char PageIndex, unsigned short *StrobeValue ) ;
EXPORT BOOL ALT_ESTP_AllStrobeValueRead ( int Index, char PageIndex, unsigned short *StrobeValue ) ;
EXPORT BOOL ALT_ESTP_TriggerModeSend ( int Index, BOOL TriggerMode ) ;
EXPORT BOOL ALT_ESTP_EncoderModeSend ( int Index, char EncoderMode ) ;
EXPORT BOOL ALT_ESTP_SaveSend ( int Index ) ;
EXPORT BOOL ALT_ESTP_LoadSend ( int Index ) ;
EXPORT BOOL ALT_ESTP_EncClearSend ( int Index ) ;
EXPORT BOOL ALT_ESTP_EncCountRead ( int Index, unsigned long *EncCount ) ;
EXPORT BOOL ALT_ESTP_RunCodeSend ( int Index, char RunCode ) ;
EXPORT BOOL ALT_ESTP_PageIndexSend ( int Index, char PageIndex ) ;
EXPORT BOOL ALT_ESTP_PageIndexRead ( int Index, char *PageIndex ) ;
EXPORT BOOL ALT_ESTP_SoftTriggerSend ( int Index, unsigned long TriggerHigh, unsigned long TriggerLow ) ;
EXPORT BOOL ALT_ESTP_ConfigModeRead ( int Index, char *ChannelNum, BOOL *TriggerMode, char *EncoderMode ) ;
EXPORT BOOL ALT_ESTP_EncOffsetListSend ( int Index, unsigned short EncCount, unsigned short *EncOffsetList ) ;
EXPORT BOOL ALT_ESTP_EncOffsetListRead ( int Index, unsigned short *EncCount, unsigned short *EncOffsetList ) ;

#endif