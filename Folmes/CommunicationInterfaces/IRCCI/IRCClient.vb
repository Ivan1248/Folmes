Imports System.IO
Imports System.Net.Sockets
Imports System.Threading

Public Class IrcClient
    Dim sendingQueue As New ThreadSafeQueue(Of String)

    Public Event Connected(nickName As String)
    Public Event LostConnection()
    Public Event MessageReceived(message As FolMessage)

    Dim server As String = "irc.freenode.net"
    Dim port As Integer = 6667

    Dim sock As TcpClient
    Dim stream As NetworkStream
    Dim input As TextReader
    Dim output As TextWriter

    Dim channel As String = "#Folmes"
    Dim username As String

    Dim thr As Thread

    Sub New(username As String)
        Me.username = username
    End Sub

    Public Sub Run()
        thr = New Thread(New ThreadStart(Sub() RunSub()))
        thr.IsBackground = True
        thr.Start()
    End Sub

    Private Sub RunSub()
        ' Connect to the irc server and get input and output text streams from TcpClient.
        If Not Connect() Then
            Exit Sub
        End If
        RaiseEvent Connected(username)

        ' Process each line received from the server

        Dim buf As String
        Dim sw As New Stopwatch

        While True
            Const sleepTime As Integer = 100 ' 100 ms
            Const PingPeriod As Integer = sleepTime * 50 \ 100 ' 5 s / 100
            Dim PingCounter As Integer = 0

            While Not IsDataReceived()
                While sendingQueue.Count > 0
                    output.WriteLine(sendingQueue.Dequeue)
                    output.Flush()
                End While

                Dim PongTimeoutCounter As Integer = 0

                If PingCounter < PingPeriod Then
                    PingCounter += 1
                    Thread.Sleep(sleepTime)
                Else
                    output.WriteLine("PING irc.freenode.net")
                    output.Flush()
                    Debug.WriteLine("PING")
                    sw.Restart()
                    PongTimeoutCounter = 0
                    While Not IsDataReceived()
                        Thread.Sleep(10)
                        PongTimeoutCounter += 1
                        If PongTimeoutCounter > 300 Then
                            RaiseEvent LostConnection()
                            Exit Sub
                        End If
                    End While
                    buf = input.ReadLine()
                    sw.Stop()
                    If IsPongCommand(buf) Then
                        Debug.WriteLine(sw.ElapsedMilliseconds)
                    Else
                        ProcessReceivedCommand(input.ReadLine())
                    End If
                End If
            End While
            ProcessReceivedCommand(input.ReadLine())
        End While
        sock.Close()
    End Sub

    Private Function Connect() As Boolean
conn:   sock = New TcpClient
        Dim buf As String
        Try
            sock.Connect(server, port)
        Catch ex As Exception
            MsgBox(ex.Message)
            sock.Close()
            Return False
        End Try
        If Not sock.Connected Then
            sock.Close()
            Console.WriteLine("Failed to connect.")
            Return False
        End If

        stream = sock.GetStream
        stream.ReadTimeout = 1
        input = New StreamReader(stream)
        output = New StreamWriter(stream)

        ' USER and NICK initial commands 
        SendUserNameCommands()

        ' Wait for welcome message
        While True
            While Not IsDataReceived() ' strange bahaviour when one of conditions used
                Thread.Sleep(10)
            End While
            buf = input.ReadLine()
            Debug.WriteLine(buf)
            Select Case buf.Substring(buf.IndexOf(" "c) + 1, 3)
                Case "001"
                    Debug.WriteLine("Connected.")
                    Exit While
                Case "433"
                    Debug.WriteLine("Nickname in use.")
                    username &= "_"
                    sock.Close()
                    GoTo conn
            End Select
        End While

        ' Set mode and join channel
        output.WriteLine("MODE " & username & " +C")
        output.WriteLine("JOIN " & channel)
        output.Flush()
        Return True
    End Function

    Private Function IsPongCommand(command As String) As Boolean
        Dim i As Integer = command.IndexOf(" "c)
        Return command.Substring(i + 1, 4) = "PONG"
    End Function

    Private Function IsDataReceived() As Boolean
        Return stream.DataAvailable OrElse input.Peek() <> -1 ' strange bahaviour when one of conditions used
    End Function

    Private Sub ProcessReceivedCommand(command As String)
        Debug.WriteLine(command)

        ' Reply to ping messages
        If command.StartsWith("PING ") Then
            output.WriteLine("PO" & command.Substring(2))
            output.Flush()
            Exit Sub
        End If

        If command(0) <> ":"c Then
            Exit Sub
        End If

        Dim m As FolMessage = IrcMesage.GetMessageFromCommand(command)
        If m IsNot Nothing Then
            RaiseEvent MessageReceived(m)
        End If
    End Sub

    Private Sub SendUserNameCommands()
        output.WriteLine("USER " & username & " 8 * :" & username) ' 8 - invisible
        output.WriteLine("NICK " & username)
        output.Flush()
    End Sub

    Public Sub SendCommand(command As String)
        If command Is Nothing Then Exit Sub
        sendingQueue.Enqueue(command)
    End Sub
    Public Sub Close(command As String)
        If command Is Nothing Then Exit Sub
        sendingQueue.Enqueue(command)
    End Sub

End Class
