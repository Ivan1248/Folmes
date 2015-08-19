Imports System.IO

Public MustInherit Class NewMessageQueues
    Private Shared CommonChannel As New MessageQueue(My.Settings.NofMsgs)
    Private Shared PrivateChannel As New List(Of MessageQueue)

    Public Shared Sub AddPrivate(channel As String, message As FolMessage)
        If Channels.Current = channel Then
            MainGUI.Output.AddMessage(message)
            Exit Sub
        End If
        Dim mq1 As MessageQueue = Nothing
        For Each mq As MessageQueue In PrivateChannel
            If mq.Id = channel Then
                mq1 = mq
                Exit For
            End If
        Next
        If mq1 Is Nothing Then
            mq1 = New MessageQueue(My.Settings.NofMsgs) With {.Id = channel}
            PrivateChannel.Add(mq1)
        End If
        mq1.Enqueue(message)
    End Sub

    Public Shared Sub AddCommon(message As FolMessage)
        If Channels.Current = Channels.Common Then
            MainGUI.Output.AddMessage(message)
        Else
            CommonChannel.Enqueue(message)
        End If
    End Sub

    Public Shared Sub LoadMessages(channel As String)
        Dim oml As MessageQueue = Nothing
        If channel = Channels.Common Then
            oml = CommonChannel
        Else
            For Each mq As MessageQueue In PrivateChannel
                If mq.Id = channel Then
                    oml = mq
                    Exit For
                End If
            Next
            If oml Is Nothing Then Exit Sub
        End If
        While oml.Count > 0
            MainGUI.Output.AddMessage(oml.Dequeue)
        End While
    End Sub

End Class
