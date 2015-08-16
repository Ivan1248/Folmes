Imports System.Text

Public Class IRCCI : Implements ICommunicationInterface

    Dim WithEvents _client As IrcClient
    Dim _synchronizingObject As Form

    Public Event NewCommonMessage(message As Message) Implements ICommunicationInterface.NewCommonMessage
    Public Event NewPrivateMessage(message As Message) Implements ICommunicationInterface.NewPrivateMessage
    Public Event PongReceived(rtt_in_ms As Long) Implements ICommunicationInterface.PongReceived
    Public Event PingError(message As String) Implements ICommunicationInterface.PingError
    Public Event PongTimeout(username As String) Implements ICommunicationInterface.PongTimeout

    Public Sub Start(SynchronizingObject As Form) Implements ICommunicationInterface.Start
        _client = New IrcClient()
        _synchronizingObject = SynchronizingObject
        _client.Run()
    End Sub

    Public Sub GetOldMessages(channel As String, count As Integer, loadSubRef As ICommunicationInterface.MessageLoadingSub) Implements ICommunicationInterface.GetOldMessages
        Throw New NotImplementedException()
    End Sub

    Public Sub SendMessage(channel As String, message As Message) Implements ICommunicationInterface.SendMessage
        Dim sb As New StringBuilder()
        sb.Append("PRIVMSG {0} :")
        sb.Append(Converter.Int64ToBase32String(message.Time))
        sb.Append(" ").Append(CInt(message.Type).ToString)
        sb.Append(If(channel = Channels.Common, "C"c, "P"c))
        sb.Append(" ").Append(message.Content.Replace(vbNewLine, "[newline]"))
        Dim IrcCommandF As String = sb.ToString
        If channel = Channels.Common Then
            For Each u As User In Users.Others
                _client.SendCommand(String.Format(IrcCommandF, u.Name))
            Next
        Else
            _client.SendCommand(String.Format(IrcCommandF, channel))
        End If
    End Sub

    Public Sub Ping(username As String) Implements ICommunicationInterface.Ping
        Throw New NotImplementedException()
    End Sub

    Public Sub InvokeOnSynchronizingObject(a As Action)
        If _synchronizingObject.InvokeRequired() Then
            _synchronizingObject.BeginInvoke(a)
        Else
            a()
        End If
    End Sub

End Class
