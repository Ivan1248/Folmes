Imports System.IO

Public MustInherit Class PingPongFile
    Structure Extension
        Const Ping As String = ".ping"
        Const Pong As String = ".pong"
    End Structure

    Public Shared Sub Create(dirPath As String, extension As String)
        Dirs.Create(dirPath)
        Using fs As New FileStream(IO.Path.Combine(dirPath, My.Settings.Username & extension), FileMode.Create, FileAccess.Write)
            fs.WriteByte(0) ' necessary for detection by LastWrite
        End Using
    End Sub
End Class
