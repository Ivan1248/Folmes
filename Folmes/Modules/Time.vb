Imports System.Net
Imports System.Net.Sockets

Public MustInherit Class Time


    Public Shared Function GetNetworkTime() As DateTime
        Const ntpServer As String = "pool.ntp.org"
        Dim ntpData As Byte() = New Byte(47) {}
        ntpData(0) = &H1B
        'LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)
        Dim addresses As IPAddress() = Dns.GetHostEntry(ntpServer).AddressList
        Dim ipEndPoint As IPEndPoint = New IPEndPoint(addresses(0), 123)
        Dim socket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)

        socket.Connect(ipEndPoint)
        socket.Send(ntpData)
        socket.Receive(ntpData)
        socket.Close()

        Dim intPart As Long = CLng(ntpData(40)) << 24 Or CLng(ntpData(41) << 16) Or CLng(ntpData(42) << 8) Or CLng(ntpData(43))
        Dim fractPart As Long = CLng(ntpData(44) << 24) Or CLng(ntpData(45) << 16) Or CLng(ntpData(46) << 8) Or CLng(ntpData(47))

        Dim milliseconds As Long = intPart * 1000 + (fractPart * 1000) \ &H100000000L
        Dim networkDateTime As Date = (New DateTime(1900, 1, 1)).AddMilliseconds(milliseconds)

        Return networkDateTime
    End Function

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================


End Class
