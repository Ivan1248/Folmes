
Public Class IRCCI : Implements ICommunicationInterface

    Dim WithEvents _client As IrcClient
    Dim _synchronizingObject As Form

    Public Event NewMessage(message As FolMessage) Implements ICommunicationInterface.NewMessage
    Public Event PongReceived(rtt_in_ms As Long) Implements ICommunicationInterface.PongReceived
    Public Event PingError(message As String) Implements ICommunicationInterface.PingError
    Public Event PongTimeout(username As String) Implements ICommunicationInterface.PongTimeout

    Public Event Connected(IrcNick As String)
    Public Event LostConnection()

    Public Sub Start(synchronizingObject As Form) Implements ICommunicationInterface.Start
        _client = New IrcClient(My.Settings.Username)
        _synchronizingObject = SynchronizingObject
        _client.Run()
    End Sub

    Public Sub LoadOldMessages(channel As String, count As Integer, loadSubRef As ICommunicationInterface.MessageLoadingSub) Implements ICommunicationInterface.LoadOldMessages
        Throw New NotImplementedException()
    End Sub

    Public Sub SendMessage(channel As String, message As FolMessage) Implements ICommunicationInterface.SendMessage
        _client.SendCommand(IrcMesage.GetCommand(channel, message))
    End Sub

    Public Sub Ping(username As String) Implements ICommunicationInterface.Ping
        Throw New NotImplementedException()
    End Sub

    Private Sub MessageRecieved(message As FolMessage) Handles _client.MessageReceived
        _synchronizingObject.BeginInvoke(Sub() RaiseEvent NewMessage(message))
    End Sub

    Private Sub RaiseMessageRecievedEvent(message As FolMessage) Handles _client.MessageReceived
        _synchronizingObject.BeginInvoke(Sub() RaiseEvent NewMessage(message))
    End Sub

    Private Sub RaiseConnectedEvent(nick As String) Handles _client.Connected
        _synchronizingObject.BeginInvoke(Sub() RaiseEvent Connected(nick))
    End Sub

    Private Sub RaiseLostConnectionEvent() Handles _client.LostConnection
        _synchronizingObject.BeginInvoke(Sub() RaiseEvent LostConnection())
        _client.Run()
    End Sub

    Public Sub SendMessage(recipientNick As String, message As String)
        _client.SendCommand("PRIVMSG " & recipientNick & " " & message.Replace(vbNewLine, "[newline]"))
    End Sub

End Class
