﻿Imports System.IO
Imports Folmes.Classes

Partial Class Box
    Private MustInherit Class MessageFiles
        Friend Shared IngoingCommon As New List(Of MessageFile)
        Friend Shared IngoingPrivate As New List(Of MessageFile)
        Friend Shared OutgoingCommon As MessageFile = Nothing
        Friend Shared OutgoingPrivate As New List(Of MessageFile)

        Friend Shared SelectedIngoing As List(Of MessageFile)
        Friend Shared SelectedOutgoing As MessageFile = Nothing

        Shared Sub GetCommon()
            OutgoingCommon = New MessageFile(Path.Combine(MessagesDir, My.Settings.Username & ".fmsg"), True, My.Settings.Username)
            Dim msgFiles As String() = Directory.GetFiles(MessagesDir, "*.fmsg")
            For Each file As String In msgFiles
                If Path.GetFileNameWithoutExtension(file) <> My.Settings.Username Then _
                    IngoingCommon.Add(New MessageFile(file, False, Path.GetFileNameWithoutExtension(file), String.Empty))
            Next
        End Sub
        Shared Sub GetIngoingPrivate()
            Dim Files As String() = Directory.GetFiles(Path.Combine(MessagesDir, My.Settings.Username), "*.fmsg")
            Dim Name As String
            Dim NewFile As MessageFile
            For Each File As String In Files
                Name = Path.GetFileNameWithoutExtension(File)
                NewFile = New MessageFile(File, False, Name, My.Settings.Username)
                IngoingPrivate.Add(NewFile)
            Next
        End Sub

        Shared Sub SwitchPrivateChannel(ByVal channelName As String)
            SelectedIngoing = New List(Of MessageFile)
            Dim fpath As String
            Dim MsgFile As MessageFile
            If IngoingPrivate IsNot Nothing Then
                For Each MsgFile In IngoingPrivate
                    If MsgFile.Sender = channelName Then
                        GoTo havei 'ako već ima, ne treba dodavati
                    End If
                Next
            End If
            fpath = Path.Combine(MessagesDir, My.Settings.Username, channelName & ".fmsg")
            If File.Exists(fpath) Then
                MsgFile = New MessageFile(fpath, False, channelName, My.Settings.Username)
                IngoingPrivate.Add(MsgFile)
havei:          SelectedIngoing.Add(MsgFile)
            End If
            For Each MsgFile In OutgoingPrivate
                If MsgFile.Recipient = channelName Then
                    GoTo haveo
                End If
            Next
            fpath = Path.Combine(MessagesDir, channelName, My.Settings.Username & ".fmsg")
            MsgFile = New MessageFile(fpath, True, My.Settings.Username, channelName)
            OutgoingPrivate.Add(MsgFile)
haveo:      SelectedOutgoing = MsgFile
        End Sub
    End Class
End Class