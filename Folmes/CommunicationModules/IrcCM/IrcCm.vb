
Public Class IrcCm : Implements ICommunicationModule
    Dim WithEvents _client As IrcClient
    Dim _synchronizingObject As Form

    Public Event MessageReceived(message As Message) Implements ICommunicationModule.MessageReceived
    Public Event PongReceived(rtt As Long) Implements ICommunicationModule.PongReceived
    Public Event PingError(message As String) Implements ICommunicationModule.PingError
    Public Event PongTimeout(username As String) Implements ICommunicationModule.PongTimeout

    Public Event Connected(userName As String) Implements ICommunicationModule.Connected
    Public Event LostConnection() Implements ICommunicationModule.LostConnection

    Public Property IsConnected As Boolean Implements ICommunicationModule.IsConnected 

    Sub New ()
        IsConnected = False
    End Sub

    Public Sub Initialize(synchronizingObject As Form) Implements ICommunicationModule.Initialize
        _client = New IrcClient(My.Settings.Username, "irc.freenode.net")
        _synchronizingObject = synchronizingObject
        _client.Run()

        AddHandler _client.MessageReceived, Sub(msg) _synchronizingObject.BeginInvoke(Sub() RaiseEvent MessageReceived(msg))
        AddHandler _client.PongReceived, Sub(rtt) _synchronizingObject.BeginInvoke(Sub() RaiseEvent PongReceived(rtt))
        AddHandler _client.Connected, Sub(nick)
                                          IsConnected = True
                                          _synchronizingObject.BeginInvoke(Sub() RaiseEvent Connected(nick))
                                      End Sub
        AddHandler _client.LostConnection, Sub()
                                               IsConnected=False
                                               _synchronizingObject.BeginInvoke(Sub() RaiseEvent LostConnection())
                                               _client.Run()
                                           End Sub
    End Sub

    Public Sub LoadOldMessages(channel As String, count As Integer, loader As ICommunicationModule.MessageLoader) Implements ICommunicationModule.LoadOldMessages
        Throw New NotImplementedException()
    End Sub

    Public Sub SendMessage(channel As String, message As Message) Implements ICommunicationModule.SendMessage
        _client.SendCommand(IrcMessage.GetCommand(channel, message))
    End Sub

    Public Sub Ping(username As String) Implements ICommunicationModule.Ping
        '_client.SendCommand()
        Throw New NotImplementedException()
    End Sub

    'Private Sub RaiseMessageRecievedEvent(message As Message) Handles _client.MessageReceived
    '    _synchronizingObject.BeginInvoke(Sub() RaiseEvent MessageReceived(message))
    'End Sub

    'Private Sub RaisePongReceivedEvent(rtt As Long) Handles _client.PongReceived
    '    _synchronizingObject.BeginInvoke(Sub() RaiseEvent PongReceived(rtt))
    'End Sub

    'Private Sub RaiseConnectedEvent(nick As String) Handles _client.Connected
    '   _synchronizingObject.BeginInvoke(Sub() RaiseEvent Connected(nick))
    'End Sub

    'Private Sub RaiseLostConnectionEvent() Handles _client.LostConnection
    '    _synchronizingObject.BeginInvoke(Sub() RaiseEvent LostConnection())
    '    _client.Run()
    'End Sub

    Public Sub SendMessage(recipientNick As String, message As String)
        _client.SendCommand("PRIVMSG " & recipientNick & " " & message.Replace(vbNewLine, "[newline]"))
    End Sub

End Class
