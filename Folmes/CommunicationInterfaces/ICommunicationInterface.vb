Public Interface ICommunicationInterface
    Event NewMessage(message As FolMessage)
    Event PongReceived(rtt_in_ms As Long)
    Event PongTimeout(username As String)
    Event PingError(message As String)

    Sub Start(SynchronizingObject As Form)

    Sub SendMessage(channel As String, message As FolMessage)

    Sub Ping(username As String)

    Delegate Sub MessageLoadingSub(m As FolMessage)

    Sub LoadOldMessages(channel As String, count As Integer, loadSubRef As MessageLoadingSub)

End Interface
