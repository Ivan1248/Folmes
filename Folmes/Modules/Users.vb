Imports System.IO
Imports System.Runtime.CompilerServices

Public MustInherit Class Users
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
                Status = If(fs.CanRead, UserStatus.Offline, CType(fs.ReadByte(), UserStatus))
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

    Public Shared Others As List(Of User)
    Public Shared MyUser As User

    Public Shared Sub Initialize()
        For Each userDir As String In Directory.GetDirectories(Dirs.Users)
            Dim user As New User(userDir)
            If user.Name <> My.Settings.Username Then
                Others.Add(user)
            Else
                MyUser = user
            End If
        Next
    End Sub

    Public Enum UserStatus As Byte
        Online = 0
        Offline = 1
        Away = 2
    End Enum

    Public Shared Sub Create(username As String)
        Dirs.Create(Path.Combine(Dirs.PrivateMessages, username))
        Dirs.Create(Path.Combine(Dirs.Users, username))
    End Sub

    Public Shared Sub Delete(username As String)
        Dim dirsToDelete As New List(Of String)
        dirsToDelete.Add(Path.Combine(Dirs.Users, username))                        ' User folder
        dirsToDelete.Add(Path.Combine(Dirs.PrivateMessages, username))              ' Private inbox
        dirsToDelete.Add(Path.Combine(Dirs.CommonChannel, username))                ' Common outbox
        For Each userCh As String In Directory.GetDirectories(Dirs.PrivateMessages) ' Private outbox
            dirsToDelete.Add(Path.Combine(userCh, username))
        Next
        For Each dir As String In dirsToDelete
            Try
                Directory.Delete(dir, True)
            Catch ex As Exception
            End Try
        Next
    End Sub

    Shared Function IsOnline(username As String) As Boolean
        Dim user As User = Others.Find(Function(u) u.Name = username)
        Return If(user IsNot Nothing, user.IsOnline(), False)
    End Function


End Class