Imports Folmes

Public Class IRCCI : Implements ICommunicationInterface

    Public Event NewCommonMessage(message As Message) Implements ICommunicationInterface.NewCommonMessage
    Public Event NewPrivateMessage(message As Message) Implements ICommunicationInterface.NewPrivateMessage
    Public Event PongReceived(rtt_in_ms As Long) Implements ICommunicationInterface.PongReceived
    Public Event PongTimeout() Implements ICommunicationInterface.PongTimeout
    Public Event PingError(message As String) Implements ICommunicationInterface.PingError

    Public Sub Start(SynchronizingObject As Form) Implements ICommunicationInterface.Start
        Throw New NotImplementedException()
    End Sub

    Public Sub GetOldMessages(channel As String, count As Integer, loadSubRef As ICommunicationInterface.MessageLoadingSub) Implements ICommunicationInterface.GetOldMessages
        Throw New NotImplementedException()
    End Sub

    Public Sub SendMessage(channel As String, message As Message) Implements ICommunicationInterface.SendMessage
        Throw New NotImplementedException()
    End Sub

    Public Sub Ping(username As String) Implements ICommunicationInterface.Ping
        Throw New NotImplementedException()
    End Sub
End Class
