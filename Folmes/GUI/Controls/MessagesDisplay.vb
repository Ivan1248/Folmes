Imports System.Security.Permissions
Imports System.Text

Namespace GUI.Controls
    'zbog komunikacije s Javascriptom
    <PermissionSet(SecurityAction.Demand, Name:="FullTrust")>
    <System.Runtime.InteropServices.ComVisibleAttribute(True)>
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

#Region "Called from Script.js"
        Public Shadows Event ContextMenu()
        Public Sub RaiseContextMenu()
            RaiseEvent ContextMenu()
        End Sub
        Public Sub ProcessStart(data As String)
            Try
                Process.Start(data)
            Catch
                Console.Beep()
            End Try
        End Sub
#End Region

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
                .Append(My.Resources.ScrollerScript).Append(vbNewLine).Append(My.Resources.Script)
                For Each scr As String In scripts
                    .Append(scr)
                Next
                '   body
                .Append("</script></head><body>" &
                        "<div id=""container""></div>")
                .Append("</body></html>")

                DocumentText = .ToString()
                Me.ObjectForScripting = Me
            End With
        End Sub

#Region "Javascript Interface"

        Private Sub RemoveOldestHtmlMessage()
            Document.InvokeScript("removeFirst")
            _htmlMessages.RemoveOldest()
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

        Private Class HtmlMessageList ' Necessary for inserting messages ordered by time
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
                    preceeding.Message.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, message)
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
                    current.Message = messageElements(i)
                    current = current.Succeeding
                Next
                Return Me
            End Function

            Private Class HtmlMessageListNode
                Public Message As HtmlElement
                Public ReadOnly Time As Long
                Public Preceeding As HtmlMessageListNode = Nothing
                Public Succeeding As HtmlMessageListNode = Nothing

                Sub New(message As HtmlElement, time As Long)
                    Me.Message = message
                    Me.Time = time
                End Sub
            End Class
        End Class

#End Region

#Region "Caching"
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
            LoadCachedChannelHtml = False
            If _cachedChannelHtmls IsNot Nothing Then
                For Each c As CachedChannelHtml In _cachedChannelHtmls
                    If c.Channel = channel Then
                        _msgContainer.InnerHtml = c.HtmlContent
                        _htmlMessages = c.HtmlMessages.RefreshReferencesInList(_msgContainer)
                        LoadCachedChannelHtml = True
                    End If
                Next
            End If
            If Not LoadCachedChannelHtml Then
                _msgContainer.InnerHtml = String.Empty
                _htmlMessages = New HtmlMessageList()
            End If
        End Function
#End Region

#Region "Messages adding"
        Public Sub AddMessage(declaration As String)
            Dim m As New Message
            m.Time = Date.UtcNow.ToBinary()
            m.Content = declaration
            m.Type = MessageType.FolmesDeclaration
            AddMessage(m)
        End Sub

        Public Sub AddMessage(message As Message)
            If _htmlMessages.Count >= My.Settings.NofMsgs Then
                RemoveOldestHtmlMessage()
            End If
            Dim messageElement As HtmlElement = CreateHtmlMessage(message)
            _htmlMessages.InsertElement(messageElement, message.Time, _msgContainer)
        End Sub

        Private Function CreateHtmlMessage(message As Message) As HtmlElement
            Dim messageElement As HtmlElement = Document.CreateElement("DIV")
            With messageElement.AppendChild(Document.CreateElement("DIV"))
                .SetAttribute("className", "time")
                Dim time As Date = Date.FromBinary(message.Time).ToLocalTime
                If (Date.Now - time).TotalHours > 24 Then
                    .InnerText = time.ToString("dd.MM.yyyy. HH:mm")
                Else
                    .InnerText = time.ToString("HH:mm")
                End If
            End With
            messageElement.SetAttribute("className", "message")
            Select Case message.Type
                Case MessageType.Normal, MessageType.Highlighted
                    If message.Type = MessageType.Highlighted Then messageElement.SetAttribute("className", "hl message")
                    With messageElement.AppendChild(Document.CreateElement("SPAN"))
                        .SetAttribute("className", "name")
                        .Style = "color:" & message.Sender.Color
                        .InnerText = message.Sender.Name
                    End With
                Case MessageType.FolmesDeclaration
                    With messageElement.AppendChild(Document.CreateElement("SPAN"))
                        .SetAttribute("className", "name")
                        .InnerText = "Folmes"
                    End With
            End Select
            With messageElement.AppendChild(Document.CreateElement("SPAN"))
                .SetAttribute("className", "content")
                .InnerHtml = If(message.Type = MessageType.Reflexive, "*", String.Empty) & message.Content
            End With
            Return messageElement
        End Function
    End Class
#End Region
End Namespace