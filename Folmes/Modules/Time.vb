Imports System.Net
Imports System.Net.Sockets

Public MustInherit Class Time

    Public Shared ReadOnly networkTimeSystemTimeDifference As Long = 0

    Shared Sub New()
        Const ntpServer As String = "pool.ntp.org"
        Dim ntpData As Byte() = New Byte(47) {}
        ntpData(0) = &H1B        'LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)
        Try
            Dim addresses As IPAddress() = Dns.GetHostEntry(ntpServer).AddressList
            Dim ipEndPoint As IPEndPoint = New IPEndPoint(addresses(0), 123)
            Dim socket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)

            socket.ReceiveTimeout = 1
            socket.Connect(ipEndPoint)
            socket.Send(ntpData)
            socket.Receive(ntpData)
            socket.Close()

            Dim intPart As Long = CLng(ntpData(40)) << 24 Or CLng(ntpData(41)) << 16 Or CLng(ntpData(42)) << 8 Or CLng(ntpData(43))
            Dim fractPart As Long = CLng(ntpData(44)) << 24 Or CLng(ntpData(45)) << 16 Or CLng(ntpData(46)) << 8 Or CLng(ntpData(47))

            Dim milliseconds As Long = (intPart * 1000L) + ((fractPart * 1000L) \ &H100000000L)
            Dim networkTime As Date = (New Date(1900, 1, 1)).AddMilliseconds(milliseconds)

            networkTimeSystemTimeDifference = networkTime.Ticks - Date.UtcNow.Ticks
        Catch
            networkTimeSystemTimeDifference = 0
        End Try
    End Sub

    Shared ReadOnly Property UtcNow As Date
        Get
            Return Date.UtcNow.AddTicks(networkTimeSystemTimeDifference)
        End Get
    End Property

End Class
