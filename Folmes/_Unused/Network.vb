Imports System.Net.NetworkInformation
Imports System.IO
Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports System.ComponentModel
Imports NATUPNPLib

Module Connection
#Region "Addresses"
    ReadOnly MyAddressInfo As New List(Of String)

    Public Sub LoadAddresses() 'DOVRŠITI
        MyAddressInfo.Clear()
        For Each ni As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
            Dim gatewayAddrs As GatewayIPAddressInformationCollection = ni.GetIPProperties.GatewayAddresses
            If ni.NetworkInterfaceType <> NetworkInterfaceType.Loopback AndAlso
                ni.OperationalStatus = OperationalStatus.Up AndAlso gatewayAddrs.Count <> 0 Then
                'Dim GatewayAddr = ni.GetIPProperties.GatewayAddresses(0).Address
                Dim gatewayAddr As IPAddress = gatewayAddrs(0).Address
                If Not gatewayAddr.Equals(IPAddress.Any) Then
                    For Each u As UnicastIPAddressInformation In ni.GetIPProperties.UnicastAddresses
                        If u.Address.AddressFamily = AddressFamily.InterNetwork Then 'AndAlso Not address.address.isloopack Then
                            MyAddressInfo.Add(New ArpRequest(gatewayAddr).GetResponse().ToString) 'ne ni.GetPhysicalAddress.ToString
                            MyAddressInfo.Add(ni.Description)
                            MyAddressInfo.Add(u.Address.ToString)
                            Exit For
                        End If
                    Next
                End If
            End If
        Next
        If _portForwarded Then
            MyAddressInfo.Add("Internet")
            MyAddressInfo.Add("Internet")
            MyAddressInfo.Add(_publicIpAddress)
        End If
    End Sub

    Private Function AddressInfoToText() As String
        Dim sb As New StringBuilder
        For Each item As String In MyAddressInfo
            sb.Append(vbLf)
            sb.Append(item)
        Next
        Return sb.ToString
    End Function

    Private Function GetFellowAddresses(username As String) As List(Of String) 'OVO NE RADI ŠTA BI SE OČEKIVALO ZBOG RAZLIČITIH NAZIVA ADAPTERA
        Dim felAddrInf() As String
        Dim addresses As New List(Of String)
        Dim fpath As String = Path.Combine(MessagesDir, username, "online.info")
        Dim i, j As Integer
        If File.Exists(fpath) Then
            Dim tsa As String = File.ReadAllText(fpath)
            tsa = tsa.Substring(tsa.IndexOf(vbLf) + 1) 'uklanjanje prvog reda
            felAddrInf = tsa.Split(Chr(10))
        Else
            Return addresses
        End If
        For i = 0 To MyAddressInfo.Count - 1 Step 3 'MAC ADRESE
            For j = 0 To felAddrInf.Length - 1 Step 3
                If MyAddressInfo(i) = felAddrInf(j) AndAlso MyAddressInfo(i) <> "Internet" Then
                    addresses.Add(felAddrInf(j + 2))
                End If
            Next
        Next
        For i = 1 To MyAddressInfo.Count - 1 Step 3 'NAZIVI
            For j = 1 To felAddrInf.Length - 1 Step 3
                If _
                    MyAddressInfo(i) = felAddrInf(j) AndAlso Not addresses.Contains(felAddrInf(j + 1)) AndAlso
                    MyAddressInfo(i) <> "Internet" Then
                    addresses.Add(felAddrInf(j + 1))
                End If
            Next
        Next
        Return addresses
    End Function
#End Region

#Region "Učitavanje -> Internet + SetOnlineStatus"

    Friend Const Port As Integer = 64721

    Dim _publicIpAddress As String = "0.0.0.0"
    Dim _portForwarded As Boolean = False
    '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    Private Sub InternetBGWorker_DoWork(sender As Object, e As DoWorkEventArgs)
        'LoadAddresses()
        ' OMOGUĆITI KASNIJE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        e.Result = PortForwarding(True)
    End Sub

    Private Sub InternetBGWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        _portForwarded = CBool(e.Result)
        'SetOnlineStatus(True)
        Box.UserInfoFiles.Mine.SetOnlineStatus(True)
    End Sub

    Private Function PortForwarding(state As Boolean) As Boolean
        Dim mappings As IStaticPortMappingCollection = New UPnPNAT().StaticPortMappingCollection
        If mappings Is Nothing Then Return False

        If state Then
            Try
                _publicIpAddress = mappings.Item(PORT, "TCP").ExternalIPAddress
            Catch
                Try
                    mappings.Add(PORT, "TCP", PORT, MyAddressInfo(2), True, "Folmes")
                    _publicIpAddress = mappings.Item(PORT, "TCP").ExternalIPAddress
                Catch ex As Exception
                    Return False
                End Try
            End Try
        Else
            Try
                mappings.Remove(PORT, "TCP")
            Catch
            End Try
        End If

        Return state
    End Function

#End Region
End Module
