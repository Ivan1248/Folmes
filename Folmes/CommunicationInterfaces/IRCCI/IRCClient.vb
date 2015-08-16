Imports System.IO
Imports System.Net.Sockets
Imports System.Threading

Public Class IrcClient
    Dim InputQueue As New ThreadSafeQueue(Of String)
    Dim outputQueue As New ThreadSafeQueue(Of String)

    Public Event NewMessageReceived()

    Dim server As String = "irc.freenode.net"
    Dim port As Integer = 6667

    Dim sock As TcpClient
    Dim stream As NetworkStream
    Dim input As TextReader
    Dim output As TextWriter

    Dim channel As String = "#Folmes"
    Dim username As String = "FolmesTest"

    Public Sub Run()
        Dim t As New Thread(Sub() RunLoop())
        t.Start()
    End Sub

    Private Sub RunLoop()
        sock = New TcpClient

        ' Connect to the irc server and get input and output text streams from TcpClient.
        sock.Connect(server, port)
        If Not sock.Connected Then
            Console.WriteLine("Failed to connect!")
            Return
        End If
        stream = sock.GetStream
        input = New StreamReader(stream)
        output = New StreamWriter(stream)

        ' USER and NICK initial commands 
        output.WriteLine("USER " & username & " 8 * :" & username) ' 8 - invisible
        output.WriteLine("NICK " & username)
        output.Flush()

        Dim buf As String

        ' Wait for welcome message
        Do
            buf = input.ReadLine()
            Console.WriteLine(buf)
        Loop While buf.Split(" "c)(1) <> "001"

        ' Set mode and join channel
        output.WriteLine("MODE " & username & " +C")
        output.WriteLine("JOIN " & channel)
        output.Flush()

        ' Process each line received from irc server
        While True
            ' While no data is received, wait and send queued data if there is any
            While Not stream.DataAvailable
                While outputQueue.Count > 0
                    output.WriteLine(outputQueue.Dequeue)
                    output.Flush()
                End While
                Thread.Sleep(20)
            End While

            buf = input.ReadLine()

            Console.WriteLine(buf)

            'Send pong reply to any ping messages
            If buf.StartsWith("PING ") Then
                output.WriteLine("PO" & buf.Substring(2))
                output.Flush()
                Continue While
            End If

            If buf(0) <> ":"c Then
                Continue While
            End If

            If buf.Contains("yo") Then
                output.WriteLine("PRIVMSG " & username & " :" & "test")
                output.Flush()
            End If
        End While
    End Sub

    Public Sub SendCommand(command As String)
        outputQueue.Enqueue(command)
    End Sub

End Class
