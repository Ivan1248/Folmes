Imports System.IO

Partial Public Class SharedFolderCI

    Sub New()
        Dirs.Create(Dirs.PingPong)
        Dirs.Create(Path.Combine(Dirs.PingPong, My.Settings.Username))
    End Sub

    Private WithEvents _timer As New Timer() With {.Interval = 10000}

    Private Class PingInProgress
        Public username As String
        Public pingTime As Long
        Sub New(username As String, pingTime As Long)
            Me.username = username
            Me.pingTime = pingTime
        End Sub
    End Class

    Private _pingsInProgress As New List(Of PingInProgress)

    Private Sub _Ping(username As String)
        For Each p As PingInProgress In _pingsInProgress
            If p.username = username Then
                Exit Sub
            End If
        Next
        PingPongFile.Create(Path.Combine(Dirs.PingPong, username), PingPongFile.Extension.Ping)
        _pingsInProgress.Add(New PingInProgress(username, Date.UtcNow.Ticks))
        _timer.Enabled = True
    End Sub

    Private Sub Pong(username As String)
        PingPongFile.Create(Path.Combine(Dirs.PingPong, username), PingPongFile.Extension.Pong)
    End Sub

    Private Sub TimeoutCheck_Timer_Tick() Handles _timer.Tick
        If (Date.UtcNow.Ticks - _pingsInProgress(0).pingTime) > 10000000 * 15 Then ' 15 seconds
            RaiseEvent PongTimeout(_pingsInProgress(0).username)
            _pingsInProgress.RemoveAt(0)
        End If
        If _pingsInProgress.Count = 0 Then
            _timer.Enabled = False
        End If
    End Sub

    Private Function GetRtt(username As String) As Long
        Dim i As Integer
        For i = 0 To _pingsInProgress.Count - 1
            If _pingsInProgress(i).username = username Then
                GetRtt = (Date.UtcNow.Ticks - _pingsInProgress(i).pingTime) \ 10000
                _pingsInProgress.RemoveAt(i)
                If _pingsInProgress.Count = 0 Then
                    _timer.Enabled = False
                End If
                Exit Function
            End If
        Next
        Return -1
    End Function

    Private Sub CleanPing()
        Dim dirPath As String = Path.Combine(Dirs.PingPong, My.Settings.Username)
        If Directory.Exists(dirPath) Then
            For Each pingFile As String In Directory.GetFiles(dirPath)
                File.Delete(pingFile)
            Next
        End If
    End Sub
End Class
