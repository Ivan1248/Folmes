Imports System.IO

Public MustInherit Class PingPong
    Private Shared _pingTime As Long = 0

    Private Shared WithEvents _timeoutTimer As New Timers.Timer(10000) With {.AutoReset = False}

    Private Shared Sub Reset() Handles _timeoutTimer.Elapsed
        _pingTime = 0
        MainGUI.Output.AddMessage("PingPong: timeout.")
    End Sub

    Public Shared Function PingFile(username As String, pong As Boolean) As Boolean
        If Not pong AndAlso _pingTime <> 0 Then
            Return True
        End If
        If UserInfoFiles.IsOnline(username) OrElse username = My.Settings.Username Then
            Dim dir As String = Path.Combine(Dirs.PingPong, username)
            Dirs.Create(dir)
            File.Create(Path.Combine(dir, My.Settings.Username & If(pong, Extension.Pong, Extension.Ping))).Close()
            _pingTime = DateTime.UtcNow.Ticks \ 10000
            _timeoutTimer.Start()
            Return True
        Else
            MsgBox("Cannot ping " & username & ". User is not online.")
            Return False
        End If
    End Function

    Public Shared Sub GetFileRtt()
        MainGUI.Output.AddMessage("PingPong: MessageFile_RTT = " & (DateTime.UtcNow.Ticks \ 10000 - _pingTime) & "ms")
        _pingTime = 0
    End Sub

    Public Shared Sub CleanPing()
        Dim dirPath As String = Path.Combine(Dirs.PingPong, My.Settings.Username)
        If Directory.Exists(dirPath) Then
            For Each pingFile As String In Directory.GetFiles(dirPath)
                Select Case Path.GetExtension(pingFile)
                    Case Extension.Ping, Extension.Pong : File.Delete(pingFile)
                End Select
            Next
        End If
    End Sub
End Class
