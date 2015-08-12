Imports System.IO

Partial Public Class SharedFolderCI

    Private WithEvents _timer As New Timer() With {.Interval = 10000}

    Private Structure PingInProgress
        Dim username As String
        Dim pingTime As Long
        Sub New(username As String, pingTime As Long)
            Me.username = username
            Me.pingTime = pingTime
        End Sub
    End Structure

    Private PingsInProgress As New List(Of PingInProgress)

    Private Sub _Ping(username As String)
        For Each p As PingInProgress In PingsInProgress
            If p.username = username Then
                Exit Sub
            End If
        Next
        PingPongFile.Create(Path.Combine(Dirs.PingPong, username), PingPongFile.Extension.Ping)
        PingsInProgress.Add(New PingInProgress(username, Date.UtcNow.Ticks))
        If Not _timer.Enabled Then _timer.Start()
    End Sub

    Private Sub Pong(username As String)
        PingPongFile.Create(Path.Combine(Dirs.PingPong, username), PingPongFile.Extension.Pong)
    End Sub

    Private Sub TimeoutCheck_Timer_Tick() Handles _timer.Tick
        If (Date.UtcNow.Ticks - PingsInProgress(0).pingTime) > 10000 * 20000 Then ' 20 seconds
            RaiseEvent PongTimeout(PingsInProgress(0).username)
            PingsInProgress.RemoveAt(0)
        End If
        If PingsInProgress.Count = 0 Then
            _timer.Stop()
        End If
    End Sub

    Private Function GetRtt(username As String) As Long
        Dim i As Integer
        Dim pipc As Integer = PingsInProgress.Count - 1
        For i = 0 To pipc
            If PingsInProgress(i).username = username Then
                GetRtt = (Date.UtcNow.Ticks - PingsInProgress(i).pingTime) \ 10000
                PingsInProgress.RemoveAt(i)
                If pipc = 0 Then _timer.Stop()
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
