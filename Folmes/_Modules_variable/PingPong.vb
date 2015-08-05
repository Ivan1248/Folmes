Imports System.IO

Public MustInherit Class PingPong
    Private Shared _pingTime As Long = 0

    Private Shared WithEvents _timeoutTimer As New Timer() With {.Interval = 10000}

    Private Shared Sub Reset() Handles _timeoutTimer.Tick
        _timeoutTimer.Stop()
        _pingTime = 0
        MainGUI.Output.AddMessage("Ping-pong:: timeout.")
    End Sub

    Public Shared Function PingFile(username As String, pong As Boolean) As Boolean
        If Not pong AndAlso _pingTime <> 0 Then
            Return False
        End If
        If Users.IsOnline(username) OrElse username = My.Settings.Username Then
            Dim dir As String = Path.Combine(Dirs.PingPong, username)
            Dirs.Create(dir)
            File.Create(Path.Combine(dir, My.Settings.Username & If(pong, Extension.Pong, Extension.Ping))).Close()
            _pingTime = DateTime.UtcNow.Ticks \ 10000
            _timeoutTimer.Start()
            Return True
        Else
            MainGUI.Output.AddMessage("Cannot ping " & username & ". User is not online.")
            Return False
        End If
    End Function

    Public Shared Sub GetFileRtt()
        _timeoutTimer.Stop()
        MainGUI.Output.AddMessage("Ping-pong: File_RTT = " & (DateTime.UtcNow.Ticks \ 10000 - _pingTime) & "ms")
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
