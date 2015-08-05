Imports System.IO
Imports System.Runtime.CompilerServices

Public Class User
    Const InfoFileSize As Integer = 1

    Public Name As String
    Public Status As UserStatus
    Private InfoFilePath As String
    Sub New(userDirectory As String)
        Me.Name = Path.GetFileName(userDirectory)
        Me.InfoFilePath = Path.Combine(userDirectory, Files.Extension.UserInfo)
        RefreshStatus()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function UserFile() As FileStream
        Return New FileStream(InfoFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, InfoFileSize)
    End Function

    Public Sub RefreshStatus()
        Using fs As FileStream = UserFile()
            fs.Seek(0, SeekOrigin.Begin)
            Status = If(fs.CanRead, CType(fs.ReadByte(), UserStatus), UserStatus.Offline)
        End Using
    End Sub

    Public Sub SetStatus(status As UserStatus)
        Using fs As FileStream = UserFile()
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
    Online = 0
    Offline = 1
    Away = 2
End Enum