﻿Imports System.Text
Imports System.IO

Public MustInherit Class MessageFile
    Public Shared Sub Create(filePath As String, msg As Message)
        Dim sb As New StringBuilder
        sb.Append(ChrW(msg.Type))
        sb.AppendLine(msg.Sender.Name)
        sb.AppendLine(msg.Content)
        Dim fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)
        Using sw As New StreamWriter(fs) With {.AutoFlush = False}
            sw.Write(sb)
        End Using
    End Sub

    Public Shared Function LoadMessage(fpath As String) As Message
        Dim m As New Message() With {.Time = Convert.ToInt64(Path.GetFileNameWithoutExtension(fpath), 16)}
        Using sr As New StreamReader(New FileStream(fpath, FileMode.Open, FileAccess.Read, FileShare.Read))
            m.Type = CType(sr.Read(), MessageType)
            Dim username As String = sr.ReadLine()
            m.Sender = Users.GetUser(username)
            If m.Sender Is Nothing Then
                m.Sender = New User(username, UserKind.Virtual)
            End If
            m.Content = sr.ReadLine()
        End Using
        Return m
    End Function


End Class
