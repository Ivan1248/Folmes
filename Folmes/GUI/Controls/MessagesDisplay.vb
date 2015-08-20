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

            Sub InsertElement(message As FolMessage, container As HtmlElement)
                Dim preceeding As HtmlMessageListNode = Nothing
                Dim succeeding As HtmlMessageListNode = Nothing
                Dim position As HtmlMessagePosition

                If _oldest Is Nothing Then ' nema ničega
                    position = HtmlMessagePosition.Only
                ElseIf message.Time >= _newest.Message.Time Then ' novija od najnovije
                    If message = _newest.Message Then
                        Exit Sub
                    End If
                    preceeding = _newest
                    position = HtmlMessagePosition.Newest
                ElseIf message.Time < _oldest.Message.Time Then ' starija od najstarije
                    succeeding = _oldest
                    position = HtmlMessagePosition.Oldest
                Else
                    succeeding = _newest
                    preceeding = succeeding.Preceeding
                    While message.Time < preceeding.Message.Time
                        succeeding = preceeding
                        preceeding = preceeding.Preceeding
                    End While
                    'Now preceeding <= node < current
                    If message = preceeding.Message Then
                        Exit Sub
                    End If
                    position = HtmlMessagePosition.In_between
                End If

                Dim node As New HtmlMessageListNode()
                node.MessageHtmlElement = CreateHtmlMessage(message, container.Document)
                node.Message = message
                node.Preceeding = preceeding
                node.Succeeding = succeeding

                Select Case position
                    Case HtmlMessagePosition.Newest
                        _newest = node
                        preceeding.Succeeding = node
                        container.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, node.MessageHtmlElement)
                    Case HtmlMessagePosition.Oldest
                        _oldest = node
                        succeeding.Preceeding = node
                        container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, node.MessageHtmlElement)
                    Case HtmlMessagePosition.In_between
                        preceeding.Succeeding = node
                        succeeding.Preceeding = node
                        preceeding.MessageHtmlElement.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, node.MessageHtmlElement)
                    Case HtmlMessagePosition.Only
                        _newest = node
                        _oldest = node
                        container.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, node.MessageHtmlElement)
                End Select

                Count += 1
            End Sub

            Private Enum HtmlMessagePosition
                Oldest
                Newest
                Only
                In_between
            End Enum

            Private Function CreateHtmlMessage(message As FolMessage, document As HtmlDocument) As HtmlElement
                Dim messageElement As HtmlElement = document.CreateElement("DIV")
                With messageElement.AppendChild(document.CreateElement("DIV"))
                    .SetAttribute("className", "time")

                    Dim time As Date = Date.FromBinary(message.Time).ToLocalTime
                    If (Date.Now - time).TotalHours > 24 Then
                        .InnerText = time.ToString("dd.MM.yyyy. HH:mm")
                    Else
                        .InnerText = time.ToString("HH:mm")
                    End If
                End With
                messageElement.SetAttribute("className", "message")
                If (message.Flags And FolMessageFlags.FolmesSystemMessage) = 0 Then
                    If (message.Flags And FolMessageFlags.Highlighted) > 0 Then messageElement.SetAttribute("className", "hl message")
                    With messageElement.AppendChild(document.CreateElement("SPAN"))
                        .SetAttribute("className", "name")
                        Dim user As User = Users.GetByName(message.Sender)
                        If user IsNot Nothing Then
                            .Style = "color:" & user.Color
                        End If
                        .InnerText = message.Sender
                    End With
                Else
                    With messageElement.AppendChild(document.CreateElement("SPAN"))
                        .SetAttribute("className", "name")
                        .InnerText = "Folmes"
                    End With
                End If
                With messageElement.AppendChild(document.CreateElement("SPAN"))
                    .SetAttribute("className", "content")
                    .InnerHtml = If((message.Flags And FolMessageFlags.MeIs) > 0, "*", String.Empty) & message.HtmlContent
                End With
                Return messageElement
            End Function

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
                Public Message As FolMessage
                Public Preceeding As HtmlMessageListNode = Nothing
                Public Succeeding As HtmlMessageListNode = Nothing
            End Class
        End Class

#End Region

#Region "Message-adding"
        Public Sub AddMessage(declaration As String)
            Dim m As New FolMessage
            m.Time = Time.UtcNow.ToBinary()
            m.HtmlContent = declaration
            m.Flags = FolMessageFlags.FolmesSystemMessage
            AddMessage(m)
        End Sub

        Public Sub AddMessage(message As FolMessage)
            If _htmlMessages.Count >= My.Settings.NofMsgs Then
                RemoveOldestHtmlMessage()
            End If
            _htmlMessages.InsertElement(message, _msgContainer)
        End Sub

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

    End Class
End Namespace