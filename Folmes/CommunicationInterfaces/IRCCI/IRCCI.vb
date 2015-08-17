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
        _client.SendCommand(FolmesIrcMesage.GetCommand(channel, message))
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
