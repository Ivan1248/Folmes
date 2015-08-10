Imports System.IO

Public Class User
    Public Name As String
    Public Color As String = "#808080"
    Public Status As UserStatus

    Private StatusFilePath As String
    Private SettingsFilePath As String

    Sub New(username As String, kind As UserKind)
        Me.Name = username
        If kind = UserKind.OldReal OrElse kind = UserKind.NewReal Then
            Me.StatusFilePath = Path.Combine(Dirs.Users, username, Files.Extension.UserStatus)
            Me.SettingsFilePath = Path.Combine(Dirs.Users, username, Files.Extension.UserInfo)
            If kind = UserKind.NewReal Then
                Me.Status = UserStatus.Online
                Me.SaveSettings()
            Else
                RefreshStatus()
                RefreshSettings()
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
            Using fs As New FileStream(StatusFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)
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

    Public Sub RefreshSettings()
        Try
            Using sr As New StreamReader(New FileStream(SettingsFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                Me.Color = sr.ReadLine()
            End Using
        Catch
        End Try
    End Sub


    Public Sub SetAndSaveStatus(status As UserStatus)
        Me.Status = status
        Using fs As FileStream = New FileStream(StatusFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite)
            fs.Seek(0, SeekOrigin.Begin)
            fs.WriteByte(status)
            fs.Flush()
        End Using
    End Sub

    Public Sub SaveSettings()
        Using sw As New StreamWriter(New FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            sw.WriteLine(Color)
        End Using
    End Sub

    Public Function IsOnline() As Boolean
        Return Status <> UserStatus.Offline
    End Function
End Class

Public Enum UserStatus As Byte
    Offline = 0
    Online = 1
    Unknown = 2
End Enum

Public Enum UserKind As Byte
    OldReal = 0
    NewReal = 1
    Virtual = 2
End Enum