
Public Interface ICommunicationModule
    Event MessageReceived(message As Message)
    Event PongReceived(rtt As Long)
    Event PongTimeout(username As String)
    Event PingError(message As String)

    Event Connected(userName As String)
    Event LostConnection()

    ReadOnly Property IsConnected As Boolean

    Sub Initialize(synchronizingObject As Form)

    Sub SendMessage(channel As String, message As Message)

    Sub Ping(username As String)

    Delegate Sub MessageLoader(message As Message)

    Sub LoadOldMessages(channel As String, count As Integer, loader As MessageLoader)
End Interface
