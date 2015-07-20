Imports System.IO
Imports System.Resources

Public MustInherit Class PingPong
    Private Shared PingTime As Long = 0

    Private Shared WithEvents TimeoutTimer As New System.Timers.Timer(10000) With {.AutoReset = False}

    Private Shared Sub Reset() Handles TimeoutTimer.Elapsed
        PingTime = 0
        MainGUI.Output.AddMessage("PingPong: timeout.")
    End Sub

    Public Shared Function PingFile(username As String, pong As Boolean) As Boolean
        If Not pong AndAlso PingTime <> 0 Then
            Return True
        End If
        If MainGUI.UserInfoFiles.IsOnline(username) OrElse username = My.Settings.Username Then
            Dim dir As String = Path.Combine(PingDir, username)
            MakeDir(dir)
            File.Create(Path.Combine(dir, My.Settings.Username & If(pong, Extension.Pong, Extension.Ping))).Close()
            PingTime = DateTime.UtcNow.Ticks \ 10000
            TimeoutTimer.Start()
            Return True
        Else
            MsgBox("Cannot ping " & username & ". User is not online.")
            Return False
        End If
    End Function

    Public Shared Sub GetFileRTT()
        MainGUI.Output.AddMessage("PingPong: MessageFile_RTT = " & (DateTime.UtcNow.Ticks \ 10000 - PingTime) & "ms")
        PingTime = 0
    End Sub

    Public Shared Sub CleanPing()
        Dim DirPath As String = Path.Combine(PingDir, My.Settings.Username)
        If Directory.Exists(DirPath) Then
            For Each PingFile As String In Directory.GetFiles(DirPath)
                Select Case Path.GetExtension(PingFile)
                    Case Extension.Ping, Extension.Pong : File.Delete(PingFile)
                End Select
            Next
        End If
    End Sub
End Class
