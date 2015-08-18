Public Interface ICommunicationInterface
    Event NewMessage(message As Message)
    Event PongReceived(rtt_in_ms As Long)
    Event PongTimeout(username As String)
    Event PingError(message As String)

    Sub Start(SynchronizingObject As Form)

    Sub SendMessage(channel As String, message As Message)

    Sub Ping(username As String)

    Delegate Sub MessageLoadingSub(m As Message)

    Sub GetOldMessages(channel As String, count As Integer, loadSubRef As ICommunicationInterface.MessageLoadingSub)



End Interface
