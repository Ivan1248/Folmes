Imports System.ComponentModel
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices

Module NativeMethods

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, BestFitMapping:=False)>
    Public Function MoveFile(src As String, dest As String) As Boolean
    End Function

    Friend NotInheritable Class IPHelper
        Private Sub New()
        End Sub

        ' Possible return values
        Friend Const NO_ERROR As Integer = 0
        Friend Const ERROR_BAD_NET_NAME As Integer = 67
        Friend Const ERROR_BUFFER_OVERFLOW As Integer = 111
        Friend Const ERROR_GEN_FAILURE As Integer = 31
        Friend Const ERROR_INVALID_PARAMETER As Integer = 87
        Friend Const ERROR_INVALID_USER_BUFFER As Integer = 1784
        Friend Const ERROR_NOT_FOUND As Integer = 1168
        Friend Const ERROR_NOT_SUPPORTED As Integer = 50

        ' API function declaration.
        <DllImport("iphlpapi.dll", SetLastError:=True)>
        Friend Shared Function SendARP(
                     DestIP As UInt32,
                     SrcIP As UInt32,
                     pMacAddr() As Byte,
                     ByRef PhyAddrLen As Int32) As UInt32
        End Function

    End Class
End Module

Public Class ArpRequest

    Private _address As IPAddress

    Public Sub New(address As IPAddress)
        _address = address
    End Sub

    ''' <summary>
    ''' Gets the MAC address that belongs to the specified IP address.
    ''' </summary>
    ''' <remarks>This uses a native method and should be replaced when a managed alternative becomes available.</remarks>
    Public Function GetResponse() As PhysicalAddress
        Dim ip As UInteger = BitConverter.ToUInt32(_address.GetAddressBytes(), 0)
        Dim mac() As Byte = New Byte(5) {}

        Dim ReturnValue As Integer = CInt(NativeMethods.IPHelper.SendARP(CUInt(ip), 0, mac, mac.Length))

        If ReturnValue = NativeMethods.IPHelper.NO_ERROR Then
            Return New PhysicalAddress(mac)
        Else
            ' TODO: handle various SendARP errors
            ' http://msdn.microsoft.com/en-us/library/windows/desktop/aa366358(v=vs.85).aspx
            Throw New Win32Exception(CInt(ReturnValue))
        End If
    End Function
End Class