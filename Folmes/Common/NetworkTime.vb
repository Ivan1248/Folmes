Imports System.Net
Imports System.Net.Sockets

Public MustInherit Class NetworkTime
    Private Shared ReadOnly networkTimeSystemTimeDifference As Long

    Shared Sub New()
        Const ntpServer As String = "pool.ntp.org"
        Dim ntpData As Byte() = New Byte(47) {}
        ntpData(0) = &H1B        'LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)
        Try
            Using socket As New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) With
                {.ReceiveTimeout = 3000}
                socket.Connect(New IPEndPoint(Dns.GetHostEntry(ntpServer).AddressList(0), 123))
                socket.Send(ntpData)
                socket.Receive(ntpData)
            End Using
            Dim intPart As Long = CLng(ntpData(40)) << 24 Or CLng(ntpData(41)) << 16 Or CLng(ntpData(42)) << 8 Or
                                  CLng(ntpData(43))
            Dim fractPart As Long = CLng(ntpData(44)) << 24 Or CLng(ntpData(45)) << 16 Or CLng(ntpData(46)) << 8 Or
                                    CLng(ntpData(47))
            Dim milliseconds As Long = (intPart * 1000L) + ((fractPart * 1000L) \ &H100000000L)
            Dim networkTime As Date = (New Date(1900, 1, 1)).AddMilliseconds(milliseconds)
            networkTimeSystemTimeDifference = networkTime.Ticks - Date.UtcNow.Ticks
        Catch ex As Exception
            Debug.WriteLine(GetType(NetworkTime).ToString() & ": " & ex.Message)
            networkTimeSystemTimeDifference = 0
        End Try
    End Sub

    Shared ReadOnly Property UtcNow() As Date
        Get
            Return Date.UtcNow.AddTicks(networkTimeSystemTimeDifference)
        End Get
    End Property
End Class
