Imports System.IO
Imports System.Runtime.CompilerServices
Imports Folmes.Classes
Imports Folmes.Datatypes

Partial Class MainGUI
    Private MustInherit Class OutputHtmlMessages
        Public Shared Sub LoadInitial_Once() ' učitava stare poruke do najnovije prije otvaranja
            Dim NextFile As MessageFile = Nothing
            Dim CurrTime As Long
            For i As Integer = 0 To My.Settings.NofMsgs
                CurrTime = 0
                For Each File As MessageFile In MessageFiles.SelectedIngoing
                    If File.OldQueueLength > 0 AndAlso File.NextUnreadOldTime > CurrTime Then
                        CurrTime = File.NextUnreadOldTime
                        NextFile = File
                    End If
                Next
                If MessageFiles.SelectedOutgoing.OldQueueLength > 0 AndAlso MessageFiles.SelectedOutgoing.NextUnreadOldTime > CurrTime Then
                    CurrTime = MessageFiles.SelectedOutgoing.NextUnreadOldTime
                    NextFile = MessageFiles.SelectedOutgoing
                End If
                If CurrTime = 0 Then Exit For
                MainGUI.Output.LoadMessage(NextFile.GetNextOlder())
            Next
        End Sub

        Shared Sub LoadNew_File(msgFile As MessageFile)
            While msgFile.NewQueueLength > 0
                MainGUI.Output.LoadMessage(msgFile.GetNextNewer())
            End While
        End Sub

        Shared Sub LoadNew() ' učitava sve nove poruke od najstarije poslije otvaranja
            Dim NextFile As MessageFile = Nothing
            Dim CurrTime As Long
            While True
                CurrTime = Long.MaxValue
                For Each File As MessageFile In MessageFiles.SelectedIngoing
                    If File.NewQueueLength > 0 AndAlso File.NextUnreadNewTime < CurrTime Then
                        NextFile = File
                        CurrTime = NextFile.NextUnreadOldTime
                    End If
                Next
                If MessageFiles.SelectedOutgoing.NewQueueLength > 0 AndAlso MessageFiles.SelectedOutgoing.NextUnreadNewTime < CurrTime Then
                    NextFile = MessageFiles.SelectedOutgoing
                    CurrTime = NextFile.NextUnreadOldTime
                End If
                If CurrTime = Long.MaxValue Then Exit Sub
                MainGUI.Output.LoadMessage(NextFile.GetNextNewer)
            End While
        End Sub
    End Class

End Class