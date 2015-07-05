Imports System.IO

Partial Class Box
#Region "Ping-pong (round-trip-time)"
    Private PingTime As Long = 0

    Private Function Ping(userName As String) As Boolean
        If PingTime <> 0 Then
            MsgBox("Already waiting for a pong.")
            Return False
        End If
        If Box.UserInfoFiles.IsOnline(userName) Then
            File.Create(Path.Combine(MessagesDir, userName, My.Settings.Username & Files.Extension.Ping))
            PingTime = DateTime.Now.Ticks \ 10000
            Return True
        Else
            MsgBox("Cannot ping " & userName & ". User is not online.")
            Return False
        End If
    End Function

    Private Sub GetPing()
        MsgBox("RTT = " & (DateTime.Now.Ticks \ 10000 - PingTime) / 1000 & " s")
        PingTime = 0
    End Sub

    Private Sub CleanPing()
        Dim DirPath As String = Path.Combine(MessagesDir, My.Settings.Username)
        If Directory.Exists(DirPath) Then
            For Each PingFile As String In Directory.GetFiles(DirPath)
                Select Case Path.GetExtension(PingFile)
                    Case "ping", "pong" : File.Delete(PingFile)
                End Select
            Next
        End If
    End Sub


#End Region

End Class
