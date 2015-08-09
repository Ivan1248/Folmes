Imports System.IO
Imports System.Runtime.CompilerServices

Public Class User
    Const InfoFileSize As Integer = 1

    Private StatusFilePath As String
    Private InfoFilePath As String

    Public Name As String
    Public Color As String = "#808080"
    Public Status As UserStatus

    Sub New(username As String, kind As UserKind)
        Me.Name = username
        If kind = UserKind.OldReal OrElse kind = UserKind.NewReal Then
            Me.StatusFilePath = Path.Combine(Dirs.Users, username, Extension.UserStatus)
            Me.InfoFilePath = Path.Combine(Dirs.Users, username, Extension.UserInfo)
            If kind = UserKind.NewReal Then
                Me.Status = UserStatus.Online
                Me.SaveInfo()
            Else
                RefreshStatus()
                RefreshInfo()
            End If
        Else
            Me.Status = UserStatus.Unknown
        End If
    End Sub

    Public Sub RefreshStatus()
        Try
            Using fs As New FileStream(StatusFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, InfoFileSize)
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
            Using sr As New StreamReader(New FileStream(InfoFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, InfoFileSize))
                Me.Color = sr.ReadLine()
            End Using
        Catch
        End Try
    End Sub


    Public Sub SetAndSaveStatus(status As UserStatus)
        Me.Status = status
        Using fs As FileStream = New FileStream(StatusFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, InfoFileSize)
            fs.Seek(0, SeekOrigin.Begin)
            fs.WriteByte(status)
            fs.Flush()
        End Using
    End Sub

    Public Sub SaveInfo()
        Using sw As New StreamWriter(New FileStream(InfoFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, InfoFileSize))
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