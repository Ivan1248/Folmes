Imports System.IO

Public MustInherit Class Users
    Public Structure User
        Public Name As String
        Public Status As Status
    End Structure

    Public Shared List As List(Of User)   ' All users except The user

    Public Shared Sub InitializeList()
        For Each d As String In Directory.GetDirectories(Dirs.Users)
            Dim u As New User() With {.Name = Path.GetFileName(d)}
            For Each f As String In Directory.GetFiles(d, "*" & Files.Extension.UserStatus)
                For i As Integer = 0 To _status.Length - 1
                    If Path.GetFileNameWithoutExtension(f) = _status(i) Then
                        u.Status = CType(i, Status)
                        Exit For
                    End If
                Next
                Exit For
            Next
            List.Add(u)
        Next
    End Sub

    Public Enum Status
        Online = 0
        Offline = 1
        Away = 2
    End Enum
    Private Shared ReadOnly _status As String() = {"online", "offline", "away"}
    Public Shared Sub SetStatus(username As String, status As Status)
        File.Create(Path.Combine(Dirs.Users, username, _status(status)))
    End Sub

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
End Class