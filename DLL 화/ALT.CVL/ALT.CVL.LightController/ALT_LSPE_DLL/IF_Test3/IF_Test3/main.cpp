//==================================================================================
// Common Header File Include
//==================================================================================

#include <windows.h>
#include <stdio.h>
#include <process.h>

//==================================================================================
// Resource Header File Include
//==================================================================================

#include "resource.h"

//==================================================================================
// Private Header File Include
//==================================================================================

#include "main.h"

#include "ALT_LSPE.h"

//===================================================================================
// Constant Define
//==================================================================================

//==================================================================================
// Grobal Variable Define
//==================================================================================

//==================================================================================
// Local Variable Define
//==================================================================================

BOOL g_Data_ThreadEnd = FALSE ;
BOOL g_Comm_ThreadEnd = TRUE ;
BOOL g_Start = FALSE ;
int g_NetIndex = -1 ;

// Common
char g_IP[4] ;
char g_NetMask[4] ;
char g_GateWay[4] ;
int g_PortNum ;
char g_MAC[6] ;

// LSPE Data
char g_ChValueArray[MODE8_MAX_CHANNEL] ;
char g_ChValue = 0 ;
char g_PageIndex = 0 ;
char g_ChannelIndex = 0 ;

char g_ChannelMode = 0 ;
char g_SegmentNum = 8 ;

//==================================================================================
// Local Function Define
//==================================================================================

LRESULT CALLBACK MainProc ( HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam ) ;

void DataViewProc ( void *Data ) ;
void CommProc ( void *Data ) ;

void DataViewInsert ( HWND hwnd, BOOL Send, char *Buffer, unsigned long Time ) ;

//==================================================================================
// Window Main Program Function
//==================================================================================

int WINAPI WinMain ( HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow )
{
	HWND hWnd ;
	MSG msg ;
	
    hWnd = (HWND)CreateDialog ( hInstance, MAKEINTRESOURCE ( IDD_IF_TEST3 ), NULL, (DLGPROC)MainProc ) ;
	ShowWindow ( hWnd, SW_SHOW ) ;

	while ( GetMessage ( &msg, NULL, 0, 0 ) != 0 )
	{			
		if ( ! IsWindow ( hWnd ) || ! IsDialogMessage ( hWnd, &msg ) )
		{
			TranslateMessage ( &msg ) ;
			DispatchMessage	( &msg ) ;															
		}
	}

    return 0 ;
}

//===================================================================================
// Window Main Process Function
//==================================================================================

LRESULT CALLBACK MainProc ( HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam )
{
	switch ( message ) 
	{
	case WM_INITDIALOG :	
		SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_RESETCONTENT, 0, 0 ) ; 
		SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_SETHORIZONTALEXTENT, 15000, 0 ) ; 

		g_Data_ThreadEnd = FALSE ;
		_beginthread ( DataViewProc, 0, (void*)hwnd ) ;		
		g_Comm_ThreadEnd = TRUE ;
		_beginthread ( CommProc, 0, (void*)hwnd ) ;
				
		return TRUE ;
		
	case WM_COMMAND :	
		switch ( LOWORD ( wParam ) )
		{
		case IDC_START :
			g_Start = ! g_Start ;
			if ( g_Start )
			{				
				SetWindowText ( GetDlgItem ( hwnd, IDC_START ), TEXT ( "Stop" ) ) ;

				g_NetIndex = ALT_LSPE_Create ( ) ;
				if ( g_NetIndex == -1 )
					return FALSE ;

				if ( ! ALT_LSPE_Lan_Start ( g_NetIndex, TRUE, "192.168.10.10", 1000, MAX_CHANNEL_4 ) )
					MessageBox ( NULL, TEXT ( "ALT_LSPE_Lan_Start 1 Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;
				//if ( ! ALT_LSPE_Uart_Start ( g_NetIndex, "\\\\.\\COM3", 115200, 4 ) )
				//	MessageBox ( NULL, TEXT ( "UartOpen Fail !" ), TEXT ( "TEST" ), MB_OK ) ;	

				g_Comm_ThreadEnd = FALSE ;				
			}
			else
			{
				g_Comm_ThreadEnd = TRUE ;
				ALT_LSPE_Close ( g_NetIndex ) ;
				SetWindowText ( GetDlgItem ( hwnd, IDC_START ), TEXT ( "Start" ) ) ;
			}
			return TRUE ;

		case IDC_RESET :
			SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_RESETCONTENT, 0, 0 ) ; 
			SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_SETHORIZONTALEXTENT, 10000, 0 ) ;
			return TRUE ;
			
		case IDCANCEL :		
			g_Data_ThreadEnd = TRUE ;
			g_Comm_ThreadEnd = TRUE ;
			DestroyWindow ( hwnd ) ;
			return TRUE ;
		}		
		break ;

	case WM_DESTROY :			
		PostQuitMessage ( 1 ) ;
		break ;
	}

	return FALSE ;
}

//========================================================================================

void DataViewInsert ( HWND hwnd, BOOL Send, char *Buffer, unsigned long Time )
{
	int count, index ;	
	char Target[2200] ;
	
	count = (int)SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_GETCOUNT, 0, 0 ) ;
	if ( count >= 1000 )
		SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_DELETESTRING, 0, 0 ) ;	
	
	if ( Send )
		sprintf_s ( Target, sizeof ( Target ), "Send: %ld: %s", Time, Buffer ) ;
	else
		sprintf_s ( Target, sizeof ( Target ), "Recv: %ld: %s", Time, Buffer ) ;
	
	index = (int)SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_ADDSTRING, 0, (WPARAM)Target ) ;	
	
	if ( index != LB_ERRSPACE )
	{
		SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_SETCURSEL, (WPARAM)index, 0 ) ;	
		SendDlgItemMessageA ( hwnd, IDC_DATA_LIST, LB_SETTOPINDEX, (WPARAM)index, 0 ) ;	
	}	
}

void DataViewProc ( void *Data )
{
	HWND hwnd ;
	unsigned long Time ;
	wchar_t Target[2200] ;
	char string[2200] ;
	int DataSize ;
	BOOL SendMode ;
	size_t tcnt ;

	hwnd = (HWND)Data ;

	while ( 1 )
	{
		if ( g_Comm_ThreadEnd )
			continue ;

		if ( g_Data_ThreadEnd )
			break ;
		
		DataSize = 4400 ;
		if ( ALT_LSPE_Protocol_LoadW ( g_NetIndex, &Time, Target, &DataSize, &SendMode ) )
		{
			wcstombs_s ( &tcnt, string, sizeof ( string ), Target, sizeof ( string )-1 ) ;
			string[tcnt] = 0 ;		
			DataViewInsert ( hwnd, SendMode, string, Time ) ;
		}
	}

	_endthread ( ) ;
}

void CommProc ( void *Data )
{
	HWND hwnd ;
	unsigned long Count = 0 ;
	char Title[256] ;
	wchar_t *wstring ;

	hwnd = (HWND)Data ;
	
	while ( 1 )
	{
		if ( g_Comm_ThreadEnd )
		{
			Count = 0 ;
			continue ;
		}

		if ( ! ALT_LSPE_IsTcpConnect ( g_NetIndex ) && 
			! ALT_LSPE_IsUdpConnect ( g_NetIndex ) &&
			! ALT_LSPE_IsUartConnect ( g_NetIndex ) )
			continue ;		
		
		if ( ! ALT_LSPE_ReadIPAddress ( g_NetIndex, g_IP, g_NetMask, g_GateWay, &g_PortNum ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_ReadIPAddress Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;
		if ( ! ALT_LSPE_ReadMACAddress ( g_NetIndex, g_MAC ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_ReadMACAddress Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;

		if ( ! ALT_LSPE_ChannelValueSend ( g_NetIndex, g_PageIndex, g_ChannelIndex, g_ChValue ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_ChannelValueSend Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;
		if ( ! ALT_LSPE_AllChannelValueSend ( g_NetIndex, g_PageIndex, g_ChValueArray ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_AllChannelValueSend Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;
		if ( ! ALT_LSPE_AllChannelValueRead ( g_NetIndex, g_PageIndex, g_ChValueArray ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_AllChannelValueRead Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;

		if ( ! ALT_LSPE_PageIndexSend ( g_NetIndex, g_PageIndex ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_PageIndexSend Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;
		if ( ! ALT_LSPE_PageIndexRead ( g_NetIndex, &g_PageIndex ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_PageIndexRead Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;
		if ( ! ALT_LSPE_ConfigModeRead ( g_NetIndex, &g_ChannelMode, &g_SegmentNum ) )
			MessageBox ( NULL, TEXT ( "ALT_LSPE_ConfigModeRead Error!" ), TEXT ( "ALT-LSPE" ), MB_OK ) ;

		++ Count ;
		sprintf_s ( Title, sizeof ( Title ), "Loop Count : %ld", Count ) ;
		wstring = new wchar_t[strlen ( Title )+1] ;
		mbstowcs_s ( NULL, wstring, strlen ( Title )+1, Title, strlen ( Title ) ) ;
		SetWindowText( hwnd, wstring ) ;
		delete wstring ;		
	}

	_endthread ( ) ;
}

