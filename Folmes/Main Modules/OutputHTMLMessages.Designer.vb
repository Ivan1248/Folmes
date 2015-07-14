Imports System.IO
Imports System.Runtime.CompilerServices
Imports Folmes.Classes

Partial Class MainGUI
    Private Class OutputHtmlMessages
        Public Structure CachedChannelHtml
            Dim Channel As String
            Dim HtmlContent As String
            Dim HtmlMessages As HtmlMessageList
            Sub New(channel As String, htmlContent As String, htmlMessages As HtmlMessageList)
                Me.Channel = channel
                Me.HtmlContent = htmlContent
                Me.HtmlMessages=htmlMessages
            End Sub
        End Structure
        Protected Shared CachedChannelHtmls As New List(Of CachedChannelHtml)
        Shared Sub CacheChannelHtml()
            If CachedChannelHtmls IsNot Nothing Then
                For Each C As CachedChannelHtml In CachedChannelHtmls
                    If C.Channel = Channels.Current Then
                        C.HtmlContent = MainGUI.Output.Document.GetElementById("container").InnerHtml
                        C.HtmlMessages = HtmlMessages
                        Exit Sub
                    End If
                Next
            End If
            Dim a As HtmlDocument = MainGUI.Output.Document
            CachedChannelHtmls.Add(New CachedChannelHtml(Channels.Current, MainGUI.Output.Document.GetElementById("container").InnerHtml, HtmlMessages))
        End Sub
        Shared Function LoadCachedChannelHtml() As Boolean
            If CachedChannelHtmls IsNot Nothing Then
                For Each C As CachedChannelHtml In CachedChannelHtmls
                    If C.Channel = Channels.Current Then
                        MainGUI.Output.Document.GetElementById("container").InnerHtml = C.HtmlContent
                        HtmlMessages = C.HtmlMessages.RefreshReferences(MainGUI.Output.Document.GetElementById("container"))
                        Return True
                    End If
                Next
            End If
            MainGUI.Output.Document.GetElementById("container").InnerHtml = String.Empty
            HtmlMessages = New HtmlMessageList()
            Return False
        End Function
        Private Shared HtmlMessages As New HtmlMessageList()  'For inserting ordered by datetime
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
                LoadMessageToOutput(NextFile.GetNextOlder())
            Next
        End Sub

        Shared Sub LoadNew_File(msgFile As MessageFile)
            While msgFile.NewQueueLength > 0
                LoadMessageToOutput(msgFile.GetNextNewer())
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
                LoadMessageToOutput(NextFile.GetNextNewer)
            End While
        End Sub
        Shared Sub LoadMessageToOutput(message As Message)
            Dim doc As HtmlDocument = MainGUI.Output.Document
            While HtmlMessages.Count >= My.Settings.NofMsgs
                MainGUI.RemoveOldestHTMLMessage()
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
        End Sub
    End Class

#Region "Javascript"
    Private Sub RefreshScroller()
        Output.Document.InvokeScript("refreshScroller")
    End Sub
    Private Sub ScrollDown()
        Output.Document.InvokeScript("scrollDown", {0})
    End Sub
    Private Sub RemoveOldestHTMLMessage()
        Output.Document.InvokeScript("removeFirst")
    End Sub
#End Region

End Class

Public Class HtmlMessageList
    Dim Newest As HtmlMessageNode = Nothing
    Dim Oldest As HtmlMessageNode = Nothing
    Public Count As Integer = 0
    Sub InsertElement(message As HtmlElement, time As Long, container As HtmlElement)
        Dim Node As New HtmlMessageNode(message, time)
        Me.Count += 1
        If Oldest Is Nothing Then   ' nema ničega
            Me.Newest = Node
            Me.Oldest = Node
            container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, message)
        ElseIf time >= Newest.Time Then ' novija od najnovije
            Me.Newest.Succeeding = Node
            Node.Preceeding = Me.Newest
            Me.Newest = Node
            container.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, message)
        ElseIf time < Oldest.Time Then ' starija od najstarije
            Node.Succeeding = Me.Oldest
            Me.Oldest.Preceeding = Node
            Me.Oldest = Node
            container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, message)
        Else
            Dim Current, Preceeding As HtmlMessageNode
            Current = Me.Newest
            Preceeding = Current.Preceeding
            While time < Preceeding.Time
                Current = Preceeding
                Preceeding = Preceeding.Preceeding
            End While 'Current.Preceeding < Node < Current
            Node.Preceeding = Preceeding
            Node.Succeeding = Current
            Current.Preceeding = Node
            Preceeding.Succeeding = Node
            Preceeding.MessageHtmlElement.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, message)
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

    Function RefreshReferences(container As HtmlElement) As HtmlMessageList
        Dim Current As HtmlMessageNode = Oldest
        Dim MessageElements As HtmlElementCollection = container.Children
        For i As Integer = 0 To MessageElements.Count - 1
            Current.MessageHtmlElement = MessageElements(i)
            Current = Current.Succeeding
        Next
        Return Me
    End Function

    Private Class HtmlMessageNode
        Public MessageHtmlElement As HtmlElement
        Public Time As Long
        Public Preceeding As HtmlMessageNode = Nothing
        Public Succeeding As HtmlMessageNode = Nothing
        Sub New(message As HtmlElement, time As Long)
            Me.MessageHtmlElement = message
            Me.Time = time
        End Sub
    End Class
End Class
