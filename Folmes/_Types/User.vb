Imports System.IO
Imports System.Runtime.CompilerServices

Public Class User
    Const InfoFileSize As Integer = 1

    Public Name As String
    Public Status As UserStatus
    Private InfoFilePath As String

    Sub New(username As String)
        Me.Name = username
        Me.InfoFilePath = Path.Combine(Dirs.Users, username, Files.Extension.UserInfo)
        RefreshStatus()
    End Sub

    Public Sub RefreshStatus()
        Using fs As New FileStream(InfoFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, InfoFileSize)
            If fs.CanRead Then
                fs.Seek(0, SeekOrigin.Begin)
                Dim a As Integer = fs.ReadByte()
                Try
                    Status = CType(a, UserStatus)
                    Exit Sub
                Catch
                End Try
            End If
            Status = UserStatus.Offline
        End Using
    End Sub

    Public Sub SetStatus(status As UserStatus)
        Using fs As FileStream = New FileStream(InfoFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, InfoFileSize)
            fs.Seek(0, SeekOrigin.Begin)
            fs.WriteByte(status)
            fs.Flush()
        End Using
    End Sub

    Public Function IsOnline() As Boolean
        Return Status <> UserStatus.Offline
    End Function
End Class

Public Enum UserStatus As Byte
    Offline = 0
    Online = 1
End Enum