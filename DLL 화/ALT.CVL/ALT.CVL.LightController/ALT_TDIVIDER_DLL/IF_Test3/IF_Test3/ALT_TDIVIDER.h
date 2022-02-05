//===========================================================================================
// ALT_LSTPE Series DLL Library
// 2019-09-10
//		Made by woojinauto in ALTSystem 
//===========================================================================================
// History
// 001
// - Receive TimeOut 함수 위치 변경

#ifndef		__ALT_TDIV__
#define		__ALT_TDIV__

//=================================================================================================================================
// Define

#define		MAX_PHOTO_INPUT_NUM					(char)8
#define		MAX_DIFF_INPUT_NUM					(char)8
#define		MAX_ENC_INPUT_NUM					(char)1
#define		MAX_PHOTO_OUTPUT_NUM				(char)8
#define		MAX_DIFF_OUTPUT_NUM					(char)16
#define		MAX_ENC_OUTPUT_NUM					(char)4

#define		SYS_TRIGGER_ENCODER					(char)0
#define		SYS_TRIGGER_PEG						(char)1
#define		SYS_TRIGGER_TIMER					(char)2

#define		SYS_ENCMULTI_1						(char)0
#define		SYS_ENCMULTI_2						(char)1
#define		SYS_ENCMULTI_4						(char)2

#define		SYS_SIGNAL_POLE_LOW					(char)0
#define		SYS_SIGNAL_POLE_HIGH				(char)0xFF

#define		SYS_OUTPUT_SOURCE_ENCODER			(char)0
#define		SYS_OUTPUT_SOURCE_TRIGGER			(char)1
#define		SYS_OUTPUT_SOURCE_ISOLATION			(char)2

#define		SYS_ENCOUT_TYPE_AB					(char)0
#define		SYS_ENCOUT_TYPE_PD					(char)1
#define		SYS_ENCOUT_TYPE_CCW					(char)2

#define		SYS_SIGNAL_OUT_FORWARE				(char)0
#define		SYS_SIGNAL_OUT_REVERSE				(char)1
#define		SYS_SIGNAL_OUT_BIDIR				(char)2

#define		SYS_SOURCE_INPUT_NUMBER_0			(char)0
#define		SYS_SOURCE_INPUT_NUMBER_1			(char)1
#define		SYS_SOURCE_INPUT_NUMBER_2			(char)2
#define		SYS_SOURCE_INPUT_NUMBER_3			(char)3
#define		SYS_SOURCE_INPUT_NUMBER_4			(char)4
#define		SYS_SOURCE_INPUT_NUMBER_5			(char)5
#define		SYS_SOURCE_INPUT_NUMBER_6			(char)6
#define		SYS_SOURCE_INPUT_NUMBER_7			(char)7

#define		SYS_OUT_SINGLE_MODE					(char)0
#define		SYS_OUT_REPEAT_MODE					(char)0xFF

#define		TDIV_PARAMETER_VALUE_ERROR						(DWORD)0x00000100
#define		TDIV_MEM_ALLOCATION_ERROR						(DWORD)0x00000200
#define		TDIV_SYNC_OBJECT_CREATE_ERROR					(DWORD)0x00000300
#define		TDIV_RECEIVE_THREAD_CREATE_ERROR				(DWORD)0x00000400
#define		TDIV_RECEIVE_PROTOCOL_TIMEOUT					(DWORD)0x00000500
#define		TDIV_RECEIVE_NOT_EXIST_COMMAND_ERROR			(DWORD)0x00000600
#define		TDIV_PARAMETER_EXCEPTION_ERROR					(DWORD)0x00000700

#define EXPORT extern "C" __declspec(dllexport)

//=================================================================================================================================
// Init Function

EXPORT int ALT_TDIV_Create ( void ) ;
EXPORT void ALT_TDIV_Close ( int Index ) ;

EXPORT BOOL ALT_TDIV_Lan_Start ( int Index, BOOL TCPIP, char *lpServer, int port ) ;
EXPORT BOOL ALT_TDIV_Lan_WStart ( int Index, BOOL TCPIP, wchar_t *lpServer, int port ) ;
EXPORT BOOL ALT_TDIV_Uart_Check ( int Index, char *PortName ) ;
EXPORT BOOL ALT_TDIV_Uart_WCheck ( int Index, wchar_t *PortName ) ;
EXPORT BOOL ALT_TDIV_Uart_Start ( int Index, char *PortName, int Borate ) ;
EXPORT BOOL ALT_TDIV_Uart_WStart ( int Index, wchar_t *PortName, int Borate ) ;
EXPORT BOOL ALT_TDIV_Setup ( int Index, char EncInNum, char PhotoInNum, char DiffInNum, char EncOutNum, char PhotoOutNum, char DiffOutNum ) ;
EXPORT unsigned long ALT_TDIV_GetLastErrorCode ( int Index ) ;

//=================================================================================================================================
// Status Function

EXPORT BOOL ALT_TDIV_IsTcpConnect ( int Index ) ;
EXPORT BOOL ALT_TDIV_IsUdpConnect ( int Index ) ;
EXPORT BOOL ALT_TDIV_IsUartConnect ( int Index ) ;

//=================================================================================================================================
// Monitor Protocol Function

EXPORT BOOL ALT_TDIV_Protocol_Load ( int Index, unsigned long *Time, char *DataMem, int *DataSize, BOOL *SendMode ) ;
EXPORT BOOL ALT_TDIV_Protocol_LoadW ( int Index, unsigned long *Time, wchar_t *Data, int *BDataLen, BOOL *SendMode ) ;

//=================================================================================================================================
// Common Protocol Function

EXPORT BOOL ALT_TDIV_SendIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int PortNum ) ;
EXPORT BOOL ALT_TDIV_ReadIPAddress ( int Index, char *IP, char *NetMask, char *GateWay, int *PortNum ) ;
EXPORT BOOL ALT_TDIV_SendMACAddress ( int Index, char *MAC ) ;
EXPORT BOOL ALT_TDIV_ReadMACAddress ( int Index, char *MAC ) ;

//=================================================================================================================================
// Device Open Protocol Function

EXPORT BOOL ALT_TDIV_VersionInfoRead ( int Index, unsigned long *FVersion, unsigned long *LVersion ) ;
EXPORT BOOL ALT_TDIV_ConfigEnableSend ( int Index, BOOL MapEnable, BOOL EncDivEnable, BOOL TimerEnable, BOOL *EncEnable, BOOL *PhotoEnable, BOOL *DiffEnable ) ;
EXPORT BOOL ALT_TDIV_ConfigEnableRead ( int Index, BOOL *MapEnable, BOOL *EncDivEnable, BOOL *TimerEnable, BOOL *EncEnable, BOOL *PhotoEnable, BOOL *DiffEnable ) ;
EXPORT BOOL ALT_TDIV_ConfigDataSend ( int Index, char EncSelect, char EncMulti, BOOL PEGPole, BOOL PInputPole, BOOL DInputPole, unsigned long TimeDiv, unsigned short EncOutLength, 
									 unsigned char EncDivRate, unsigned char EncInDivRate, unsigned char EncZPhaseSyncSelect, unsigned char EncZPhaseTriggerNumber ) ;
EXPORT BOOL ALT_TDIV_ConfigDataRead ( int Index, char *EncSelect, char *EncMulti, BOOL *PEGPole, BOOL *PInputPole, BOOL *DInputPole, unsigned long *TimeDiv, unsigned short *EncOutLength, 
									 unsigned char *EncDivRate, unsigned char *EncInDivRate, unsigned char *EncZPhaseSyncSelect, unsigned char *EncZPhaseTriggerNumber ) ;
EXPORT BOOL ALT_TDIV_EncOutDataSend ( int Index, char EncIndex, 
									char EncOutSource, BOOL EncWindowMode, 
									char EncOutType, char EncOutReverse, char EncOutTriggerNum, 
									unsigned long EncWindowHigh, unsigned long EncWindowLow ) ;
EXPORT BOOL ALT_TDIV_EncOutDataRead ( int Index, char EncIndex, 
									char *EncOutSource, BOOL *EncWindowMode, 
									char *EncOutType, char *EncOutReverse, char *EncOutTriggerNum, 
									unsigned long *EncWindowHigh, unsigned long *EncWindowLow ) ;
EXPORT BOOL ALT_TDIV_MapInfoSend ( int Index, unsigned short MapDistance, unsigned short MapCount ) ;
EXPORT BOOL ALT_TDIV_MapDataSend ( int Index, unsigned char MapNum, char *MapData ) ;
EXPORT BOOL ALT_TDIV_PhotoOutDataSend ( int Index, char PhotoIndex, 
									char PhotoOutSource, BOOL PhotoOutRepeat, BOOL PhotoOutPole, 
									char PhotoOutReverse, unsigned long PhotoOutOffset, char PhotoOutTriggerNum, 
									unsigned long PhotoOutDistance, unsigned short DelayTime, unsigned short ActiveTime, unsigned long PhotoOutCount, unsigned char PhotoZCheck ) ;
EXPORT BOOL ALT_TDIV_PhotoOutDataRead ( int Index, char PhotoIndex, 
									char *PhotoOutSource, BOOL *PhotoOutRepeat, BOOL *PhotoOutPole, 
									char *PhotoOutReverse, unsigned long *PhotoOutOffset, char *PhotoOutTriggerNum, 
									unsigned long *PhotoOutDistance, unsigned short *DelayTime, unsigned short *ActiveTime, unsigned long *PhotoOutCount, unsigned char *PhotoZCheck ) ;
EXPORT BOOL ALT_TDIV_DiffOutDataSend ( int Index, char DiffIndex, 
									char DiffOutSource, BOOL DiffOutRepeat, BOOL DiffOutPole, 
									char DiffOutReverse, unsigned long DiffOutOffset, char DiffOutTriggerNum, 
									unsigned long DiffOutDistance, unsigned short DelayTime, unsigned short ActiveTime, unsigned long DiffOutCount, unsigned char DiffZCheck ) ;
EXPORT BOOL ALT_TDIV_DiffOutDataRead ( int Index, char DiffIndex, 
									char *DiffOutSource, BOOL *DiffOutRepeat, BOOL *DiffOutPole, 
									char *DiffOutReverse, unsigned long *DiffOutOffset, char *DiffOutTriggerNum, 
									unsigned long *DiffOutDistance, unsigned short *DelayTime, unsigned short *ActiveTime, unsigned long *DiffOutCount, unsigned char *DiffZCheck ) ;
EXPORT BOOL ALT_TDIV_SaveSend ( int Index ) ;
EXPORT BOOL ALT_TDIV_LoadSend ( int Index ) ;
EXPORT BOOL ALT_TDIV_EncClearSend ( int Index ) ;
EXPORT BOOL ALT_TDIV_EncCountRead ( int Index, unsigned long *SEncCount, unsigned long *AEncCount, unsigned long *DEncCount, unsigned long *ZEncCount ) ;
EXPORT BOOL ALT_TDIV_SoftTriggerSend ( int Index, unsigned char SoftTrigger ) ;

#endif