Imports System.IO
Imports System.Runtime.CompilerServices
Imports Folmes.Classes

Partial Class MainGUI
    Private Class OutputHtmlMessages
        Public Structure CachedChannelHtml
            Dim Channel As String
            Dim HtmlContents As String
            Dim Count As Integer
        End Structure
        Protected Shared CachedChannelHtmls As New List(Of CachedChannelHtml)
        Shared Sub CacheChannelHtml()
            If CachedChannelHtmls IsNot Nothing Then
                For Each C As CachedChannelHtml In CachedChannelHtmls
                    If C.Channel = Channels.Current Then
                        C.HtmlContents = MainGUI.Output.Document.GetElementById("container").InnerHtml
                        C.Count = Count
                        Exit Sub
                    End If
                Next
            End If
            Dim a As HtmlDocument = MainGUI.Output.Document
            CachedChannelHtmls.Add(New CachedChannelHtml With {.Channel = Channels.Current, .HtmlContents = MainGUI.Output.Document.GetElementById("container").InnerHtml, .Count = Count})
        End Sub
        Shared Function LoadCachedChannelHtml() As Boolean
            If CachedChannelHtmls IsNot Nothing Then
                For Each C As CachedChannelHtml In CachedChannelHtmls
                    If C.Channel = Channels.Current Then
                        MainGUI.Output.Document.GetElementById("container").InnerHtml = C.HtmlContents
                        Count = C.Count
                        'HTMLMessages = C.List
                        HtmlMessages = New HtmlMessageList
                        Return True
                    End If
                Next
            End If
            MainGUI.Output.Document.GetElementById("container").InnerHtml = String.Empty
            HtmlMessages = New HtmlMessageList
            Count = 0
            Return False
        End Function

        Private Shared HtmlMessages As New HtmlMessageList  'For inserting ordered by datetime
        Friend Shared Count As Integer = 0
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
                LoadMessageToOutput(NextFile.GetNextOlder)
            Next
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
                LoadMessageToOutput(NextFile.GetNextNewer)
            End While
        End Sub
        Shared Sub LoadMessageToOutput(message As Message)
            Dim doc As HtmlDocument = MainGUI.Output.Document
            While Count >= My.Settings.NofMsgs
                MainGUI.RemoveOldestHTMLMessage()
                Count -= 1
                If HtmlMessages.Count = My.Settings.NofMsgs Then
                    HtmlMessages.RemoveOldest()
                End If
            End While
            Dim MessageElement As HtmlElement = doc.CreateElement("DIV")
            MessageElement.SetAttribute("className", "message")
            With MessageElement.AppendChild(doc.CreateElement("DIV"))
                .SetAttribute("className", "time")
                .InnerText = DateTime.FromBinary(message.Time).ToLocalTime.ToString("dd.MM.yyyy. HH:mm")
            End With
            If message.Type = message.MessageType.Normal Then
                With MessageElement.AppendChild(doc.CreateElement("SPAN"))
                    .SetAttribute("className", "name")
                    .SetAttribute("style", "color:" & My.Settings.UsernameColor) 'zamijeniti boju
                    .InnerText = message.Sender
                End With
            End If
            With MessageElement.AppendChild(doc.CreateElement("SPAN"))
                .SetAttribute("className", "content")
                .InnerHtml = message.Content
            End With
            HtmlMessages.InsertElement(MessageElement, message.Time, doc.GetElementById("container"))
            MainGUI.ScrollDown()
            MainGUI.RefreshScroller()
            Count += 1
        End Sub
    End Class

#Region "Script"
    Private Sub RefreshScroller()
        Output.Document.InvokeScript("refreshScroller")
    End Sub
    Private Sub ScrollDown()
        Output.Document.InvokeScript("scrollDown", {0})
    End Sub
    Private Sub RemoveOldestHTMLMessage()
        Output.Document.InvokeScript("refreshScroller")
    End Sub
#End Region

End Class

Public Class HtmlMessageList
    Dim Newest As HtmlMessageNode = Nothing
    Dim Oldest As HtmlMessageNode = Nothing
    Public Count As Integer = 0
    Sub InsertElement(message As HtmlElement, time As Long, container As HtmlElement)
        Dim Node As New HtmlMessageNode(message, time)
        Count += 1
        If Oldest Is Nothing Then   ' nema ničega
            Newest = Node
            Oldest = Node
            container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, message)
        ElseIf time >= Newest.Time Then ' novija od najstarije
            Newest.Succeeding = Node
            Node.Preceeding = Newest
            Newest = Node
            container.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, message)
        ElseIf time < Oldest.Time Then ' starija od najnovije
            Node.Succeeding = Oldest
            Oldest.Preceeding = Node
            Oldest = Node
            container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, message)
        Else
            Dim Current, Preceeding As HtmlMessageNode
            Current = Newest
            Preceeding = Current.Preceeding
            While time < Preceeding.Time
                Current = Preceeding
                Preceeding = Current.Preceeding
            End While 'Current.Preceeding < Node < Current
            Node.Preceeding = Preceeding
            Node.Succeeding = Current
            Current.Preceeding = Node
            Preceeding.Succeeding = Node
            Preceeding.Message.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, message)
        End If
    End Sub
    Sub RemoveOldest()
        Count -= 1
        Oldest = Oldest.Succeeding
        If Oldest Is Nothing OrElse Oldest.Succeeding Is Nothing Then
            Newest = Oldest
            Exit Sub
        End If
        Oldest.Succeeding.Preceeding = Oldest
    End Sub
    Private Class HtmlMessageNode
        Public Message As HtmlElement
        Public Time As Long
        Public Preceeding As HtmlMessageNode = Nothing
        Public Succeeding As HtmlMessageNode = Nothing
        Sub New(message As HtmlElement, time As Long)
            Me.Message = message
            Me.Time = time
        End Sub
    End Class

End Class