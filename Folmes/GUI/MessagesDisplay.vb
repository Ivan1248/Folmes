Imports System.Text

Namespace GUI
    Public Class MessagesDisplay : Inherits WebBrowser
        Private _msgContainer As HtmlElement
        Public Event Initialized()

        Sub New()
            Const defaultHtml As String = "<html><head></head><body style=""background-color:#222""></body></html>"
            AllowNavigation = False
            AllowWebBrowserDrop = False
            IsWebBrowserContextMenuEnabled = False
            ScrollBarsEnabled = False
            Url = New Uri("", UriKind.Relative)
            Document.Write(defaultHtml)
        End Sub

        Public Sub Initialize(scripts As String())
            Dim setRefs As WebBrowserDocumentCompletedEventHandler =
                    Sub()
                        RemoveHandler DocumentCompleted, setRefs
                        _msgContainer = Document.GetElementById("container")
                        RaiseEvent Initialized()
                    End Sub
            AddHandler DocumentCompleted, setRefs

            With New StringBuilder() 'kapacitet
                'HTML start
                .Append("<!DOCTYPE html><html><head>" &
                        "<meta name=""viewport"" content=""user-scalable=no, initial-scale=1"">" &
                        "<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">" &
                        "<style type=""text/css"">")
                '       style
                .Append("body{font-size:").Append(My.Settings.FontSize)
                .Append("px;line-height:").Append(Math.Round(My.Settings.FontSize * 4 / 3))
                .Append("px} img{max-height:").Append(My.Settings.ThumbnailHeight).Append("px} ")
                .Append(My.Resources.Style)
                .Append("</style><script>")
                '       script
                .Append(My.Resources.ScrollScript).Append(vbNewLine).Append(My.Resources.Script)
                For Each scr As String In scripts
                    .Append(scr)
                Next
                '   body
                .Append("</script></head>" &
                        "<body data-click="""" data-sel="""">" &
                        "<div id=""container"">")
                .Append(
                    "</div><div id=""scroller""></div><div id=""scrollertrack""></div><div id=""copybtn"">Copy</div></body></html>")
                DocumentText = .ToString()
            End With
        End Sub

#Region "Javascript Interface"

        Private Sub RefreshScroller() Handles Me.Resize
            Document.InvokeScript("refreshScroller")
        End Sub

        Private Sub ScrollDown()
            Document.InvokeScript("scrollDown", {0})
        End Sub

        Private Sub RemoveOldestHtmlMessage()
            Document.InvokeScript("removeFirst")
        End Sub

#End Region

#Region "Type definitions"

        Private Structure CachedChannelHtml
            ReadOnly Channel As String
            Dim HtmlContent As String
            Dim HtmlMessages As HtmlMessageList

            Sub New(channel As String, htmlContent As String, htmlMessages As HtmlMessageList)
                Me.Channel = channel
                Me.HtmlContent = htmlContent
                Me.HtmlMessages = htmlMessages
            End Sub
        End Structure

        Private Class HtmlMessageList ' Necessary for inserting messages oredered by time
            Dim _newest As HtmlMessageListNode = Nothing
            Dim _oldest As HtmlMessageListNode = Nothing
            Public Count As Integer = 0

            Sub InsertElement(message As HtmlElement, time As Long, container As HtmlElement)
                Dim node As New HtmlMessageListNode(message, time)
                Count += 1
                If _oldest Is Nothing Then ' nema ničega
                    _newest = node
                    _oldest = node
                    container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, message)
                ElseIf time >= _newest.Time Then ' novija od najnovije
                    _newest.Succeeding = node
                    node.Preceeding = _newest
                    _newest = node
                    container.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, message)
                ElseIf time < _oldest.Time Then ' starija od najstarije
                    node.Succeeding = _oldest
                    _oldest.Preceeding = node
                    _oldest = node
                    container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, message)
                Else
                    Dim current, preceeding As HtmlMessageListNode
                    current = _newest
                    preceeding = current.Preceeding
                    While time < preceeding.Time
                        current = preceeding
                        preceeding = preceeding.Preceeding
                    End While 'Current.Preceeding < Node < Current
                    node.Preceeding = preceeding
                    node.Succeeding = current
                    current.Preceeding = node
                    preceeding.Succeeding = node
                    preceeding.MessageHtmlElement.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd,
                                                                        message)
                End If
            End Sub

            Sub RemoveOldest()
                Count -= 1
                _oldest = _oldest.Succeeding
                If _oldest Is Nothing OrElse _oldest.Succeeding Is Nothing Then
                    _newest = _oldest
                    Exit Sub
                End If
                _oldest.Succeeding.Preceeding = _oldest
            End Sub

            Public Function RefreshReferencesInList(container As HtmlElement) As HtmlMessageList
                Dim current As HtmlMessageListNode = _oldest
                Dim messageElements As HtmlElementCollection = container.Children
                For i As Integer = 0 To messageElements.Count - 1
                    current.MessageHtmlElement = messageElements(i)
                    current = current.Succeeding
                Next
                Return Me
            End Function

            Private Class HtmlMessageListNode
                Public MessageHtmlElement As HtmlElement
                Public ReadOnly Time As Long
                Public Preceeding As HtmlMessageListNode = Nothing
                Public Succeeding As HtmlMessageListNode = Nothing

                Sub New(message As HtmlElement, time As Long)
                    MessageHtmlElement = message
                    Me.Time = time
                End Sub
            End Class
        End Class

#End Region


        Private ReadOnly _cachedChannelHtmls As New List(Of CachedChannelHtml)
        Private _htmlMessages As New HtmlMessageList()  'For inserting ordered by datetime

        Public Sub CacheChannelHtml(channel As String)
            If _cachedChannelHtmls IsNot Nothing Then
                For Each c As CachedChannelHtml In _cachedChannelHtmls
                    If c.Channel = channel Then
                        c.HtmlContent = _msgContainer.InnerHtml
                        c.HtmlMessages = _htmlMessages
                        Exit Sub
                    End If
                Next
            End If
            _cachedChannelHtmls.Add(New CachedChannelHtml(Channels.Current, _msgContainer.InnerHtml, _htmlMessages))
        End Sub

        Public Function LoadCachedChannelHtml(channel As String) As Boolean
            If _cachedChannelHtmls IsNot Nothing Then
                For Each c As CachedChannelHtml In _cachedChannelHtmls
                    If c.Channel = channel Then
                        _msgContainer.InnerHtml = c.HtmlContent
                        _htmlMessages = c.HtmlMessages.RefreshReferencesInList(_msgContainer)
                        Return True
                    End If
                Next
            End If
            _msgContainer.InnerHtml = String.Empty
            _htmlMessages = New HtmlMessageList()
            Return False
        End Function

        Public Sub AddMessage(message As Message)
            If _htmlMessages.Count >= My.Settings.NofMsgs Then
                RemoveOldestHtmlMessage()
                _htmlMessages.RemoveOldest()
            End If
            Dim messageElement As HtmlElement = InsertHtmlMessage(message)
            _htmlMessages.InsertElement(messageElement, message.Time, _msgContainer)
            ScrollDown()
            RefreshScroller()
        End Sub

        Private Function InsertHtmlMessage(message As Message) As HtmlElement
            Dim messageElement As HtmlElement = Document.CreateElement("DIV")
            messageElement.SetAttribute("className", "message")
            With messageElement.AppendChild(Document.CreateElement("DIV"))
                .SetAttribute("className", "time")
                .InnerText = DateTime.FromBinary(message.Time).ToLocalTime.ToString("dd.MM.yyyy. HH:mm")
            End With
            If message.Type = MessageType.Normal Then
                With messageElement.AppendChild(Document.CreateElement("SPAN"))
                    .SetAttribute("className", "name")
                    .SetAttribute("style", "color:" & My.Settings.UsernameColor) 'zamijeniti boju
                    .InnerText = message.Sender
                End With
            End If
            With messageElement.AppendChild(Document.CreateElement("SPAN"))
                .SetAttribute("className", "content")
                .InnerHtml = message.Content
            End With
            Return messageElement
        End Function
    End Class
End Namespace