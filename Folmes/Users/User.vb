Imports System.IO

Public Class User
    Public Name As String
    Public Color As String = "#808080"
    Public Status As UserStatus
    Public IrcNick As String

    Private ReadOnly _statusFilePath As String
    Private ReadOnly _infoFilePath As String

    Sub New(username As String, kind As UserKind)
        Me.Name = username
        If kind = UserKind.OldReal OrElse kind = UserKind.NewReal Then
            Me._statusFilePath = IO.Path.Combine(Dirs.Users, username, UserFile.Extension.UserStatus)
            Me._infoFilePath = IO.Path.Combine(Dirs.Users, username, UserFile.Extension.UserInfo)
            If kind = UserKind.NewReal Then
                Me.Status = UserStatus.Online_Folder
                Me.SaveInfo()
            Else
                RefreshStatus()
                RefreshInfo()
            End If
        Else
            Me.Status = UserStatus.Unknown
        End If
    End Sub

    Sub New(username As String, status As UserStatus, color As String)
        Me.Name = username
        Me.Status = UserStatus.Unknown
        Me.Color = color
    End Sub

    Public Sub RefreshStatus()
        Try
            Using fs As New FileStream(_statusFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)
                If fs.CanRead Then
                    fs.Seek(0, SeekOrigin.Begin)
                    Status = CType(fs.ReadByte(), UserStatus)
                    Exit Sub
                End If
            End Using
        Catch
        End Try
        Status = UserStatus.Offline
    End Sub

    Public Sub RefreshInfo()
        Try
            Using sr As New StreamReader(New FileStream(_infoFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                Me.Color = sr.ReadLine()
                Me.IrcNick = sr.ReadLine()
            End Using
        Catch
        End Try
    End Sub


    Public Sub SetAndSaveStatus(status As UserStatus)
        Me.Status = status
        Using fs As FileStream = New FileStream(_statusFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite)
            fs.Seek(0, SeekOrigin.Begin)
            fs.WriteByte(status)
            fs.Flush()
        End Using
    End Sub

    Public Sub SaveInfo()
        Using sw As New StreamWriter(New FileStream(_infoFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            sw.WriteLine(Color)
            sw.WriteLine(IrcNick)
        End Using
    End Sub

    Public Function IsOnline() As Boolean
        Return Status <> UserStatus.Offline
    End Function
End Class

<Flags()> Public Enum UserStatus As Byte
    Offline = 0
    Unknown = 1
    Online_Folder = 2
    Online_IRC = 4
End Enum

Public Enum UserKind As Byte
    OldReal = 0
    NewReal = 1
    Virtual = 2
End Enum