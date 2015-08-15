Imports System.IO
Imports System.Net.Sockets

Public Class IrcClient
    Dim server As String = "irc.freenode.net"
    Dim port As Integer = 6667
    Dim sock As TcpClient
    Dim input As TextReader
    Dim output As TextWriter
    Dim channel As String = "#Folmes"
    Dim username As String = "FolmesTest"

    Sub Run()
        sock = New TcpClient

        ' Connect to the irc server and get input and output text streams from TcpClient.
        sock.Connect(server, port)
        If Not sock.Connected Then
            Console.WriteLine("Failed to connect!")
            Return
        End If
        input = New StreamReader(sock.GetStream())
        output = New StreamWriter(sock.GetStream())

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
            buf = input.ReadLine()

            Console.WriteLine(buf)

            'Send pong reply to any ping messages
            If buf.StartsWith("PING ") Then
                output.WriteLine("PO" & buf.Substring(2))
                output.Flush()
            End If
            If buf(0) <> ":"c Then
                Continue While
            End If
        End While
    End Sub
End Class
