Imports System.IO
Imports System.Net.Sockets
Imports System.Threading

Public Class IrcClient
    Dim sendingQueue As New ThreadSafeQueue(Of String)

    Public Event Connected(nickName As String)
    Public Event MessageReceived(message As Message)

    Dim server As String = "irc.freenode.net"
    Dim port As Integer = 6667

    Dim sock As TcpClient
    Dim stream As NetworkStream
    Dim input As TextReader
    Dim output As TextWriter

    Dim channel As String = "#Folmes"
    Dim username As String = "Ivan"

    Dim thr As Thread

    Public Sub Run()
        thr = New Thread(New ThreadStart(Sub() RunLoop()))
        thr.IsBackground = True
        thr.Start()
    End Sub

    Private Sub RunLoop()
        Dim buf As String

        ' Connect to the irc server and get input and output text streams from TcpClient.
conn:   sock = New TcpClient
        Using sock
            sock.Connect(server, port)
            If Not sock.Connected Then
                Console.WriteLine("Failed to connect.")
                Return
            End If
            stream = sock.GetStream
            input = New StreamReader(stream)
            output = New StreamWriter(stream)

            ' USER and NICK initial commands 
            SendUserNameCommands()

            ' Wait for welcome message
            While True
                While Not stream.DataAvailable AndAlso input.Peek() = -1
                    Thread.Sleep(10)
                End While
                buf = input.ReadLine()
                Console.WriteLine(buf)
                Select Case buf.Substring(buf.IndexOf(" "c) + 1, 3)
                    Case "001"
                        Console.WriteLine("Connected.")
                        Exit While
                    Case "433"
                        Console.WriteLine("Nickname in use.")
                        username &= "_"
                        SendUserNameCommands()
                        GoTo conn
                End Select
            End While

            ' Set mode and join channel
            output.WriteLine("MODE " & username & " +C")
            output.WriteLine("JOIN " & channel)
            output.Flush()

            RaiseEvent Connected(username)

            ' Process each line received from the server
            While True
                ' While no data is received, wait and send queued data if there is any
                While Not stream.DataAvailable
                    While sendingQueue.Count > 0
                        output.WriteLine(sendingQueue.Dequeue)
                        output.Flush()
                    End While
                    Thread.Sleep(20)
                End While

                buf = input.ReadLine()

                Console.WriteLine(buf)

                ' Reply to ping messages
                If buf.StartsWith("PING ") Then
                    output.WriteLine("PO" & buf.Substring(2))
                    output.Flush()
                    Continue While
                End If

                If buf(0) <> ":"c Then
                    Continue While
                End If

                ' https://tools.ietf.org/html/rfc2812#page-5 / message format

                'If buf.Contains("yo") Then
                '    output.WriteLine("PRIVMSG " & username & " :" & "yo")
                '    output.Flush()
                'End If

                Dim m As Message = IrcMesage.GetMessageFromCommand(buf)
                If m IsNot Nothing Then
                    RaiseEvent MessageReceived(m)
                End If

            End While
        End Using
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
