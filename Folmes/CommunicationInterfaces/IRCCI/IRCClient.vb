Imports System.IO
Imports System.Net.Sockets

Public Class IrcClient
    Dim server As String = "irc.freenode.net"
    Dim port As Integer = 6667
    Dim sock As New TcpClient()
    Dim input As TextReader
    Dim output As TextWriter
    Dim channel As String = "#Folmes"
    Dim username As String = "FolmesTest"
    Dim owner As String = "Unknown"

    Sub Run()
        'Connect to irc server and get input and output text streams from TcpClient.
        sock.Connect(server, port)
        If Not sock.Connected Then
            Console.WriteLine("Failed to connect!")
            Return
        End If
        input = New StreamReader(sock.GetStream())
        output = New StreamWriter(sock.GetStream())

        'Starting USER and NICK login commands 
        output.WriteLine("USER " & username & " 8 * :" & owner) ' 8 - invisible
        output.WriteLine("NICK " & username)
        output.Flush()

        'Process each line received from irc server
        Dim buf As String

        While True
            buf = input.ReadLine()

            ' IRC commands come in one of these formats:
            '                * :NICK!USER@HOST COMMAND ARGS ... :DATA\r\n
            '                * :SERVER COMAND ARGS ... :DATA\r\n
            '                
            'After server sends 001 command, we can set mode to bot and join a channel
            If buf.Split(" "c)(1) = "001" Then
                output.WriteLine("MODE " & username & " +C")
                output.WriteLine("JOIN " & channel)
                output.Flush()
                Exit While
            End If
        End While

        While True
            buf = input.ReadLine()

            'Display received irc message
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
