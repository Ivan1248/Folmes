Imports System.IO

Public MustInherit Class PingPong

    Private Shared _pingTime As Long = 0

    Private Shared WithEvents _timeoutTimer As New Timer() With {.Interval = 20000}

    Private Shared Sub Reset() Handles _timeoutTimer.Tick
        _timeoutTimer.Stop()
        _pingTime = 0
        'MainGUI.Output.AddMessage("Ping-pong: timeout.")
    End Sub

    Public Shared Sub Ping(username As String)
        If _pingTime <> 0 Then
            'MainGUI.Output.AddMessage("Pinging in progress.")
        End If
        Dim dir As String = Path.Combine(Dirs.PingPong, username)
        Dirs.Create(dir)
        Using fs As New FileStream(Path.Combine(dir, My.Settings.Username & PingPongFile.Extension.Ping), FileMode.Create, FileAccess.Write)
            fs.WriteByte(0) ' necessary for detection
        End Using
        _pingTime = Date.UtcNow.Ticks \ 10000
        _timeoutTimer.Start()
    End Sub

    Public Shared Sub Pong(username As String)
        Dim dir As String = Path.Combine(Dirs.PingPong, username)
        Dirs.Create(dir)
        Using fs As New FileStream(Path.Combine(dir, My.Settings.Username & PingPongFile.Extension.Pong), FileMode.Create, FileAccess.Write)
            fs.WriteByte(0) ' necessary for detection
        End Using
    End Sub

    Public Shared Function GetFileRtt() As Long
        _timeoutTimer.Stop()
        _pingTime = 0
        Return Date.UtcNow.Ticks \ 10000 - _pingTime
    End Function

    Public Shared Sub CleanPing()
        Dim dirPath As String = Path.Combine(Dirs.PingPong, My.Settings.Username)
        If Directory.Exists(dirPath) Then
            For Each pingFile As String In Directory.GetFiles(dirPath)
                File.Delete(pingFile)
            Next
        End If
    End Sub
End Class
