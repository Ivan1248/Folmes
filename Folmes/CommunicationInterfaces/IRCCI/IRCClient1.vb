Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Public Class IrcClient
    Private _server As String = String.Empty '-- IRC server name
    Private _channel As String = String.Empty '-- the channel you want to join (prefex with #)
    Private _port As Integer = 6667 '-- the port to connect to.  Default is 6667
    Private _invisible As Boolean = False '-- shows up as an invisible user.  Still working on this.
    Private _realName As String = "nodibot" '-- More naming
    Private _userName As String = "nodi_the_bot" '-- Unique name (ident) so of the IRC network has a unique handle to you regardless of the nickname.

    Private _tcpclientConnection As TcpClient = Nothing '-- main connection to the IRC network.
    Private _networkStream As NetworkStream = Nothing '-- break that connection down to a network stream.
    Private _writer As StreamWriter = Nothing '-- provide a convenient access to writing commands.
    Private _reader As StreamReader = Nothing '-- provide a convenient access to reading commands.

    Public Sub New(server As String, channel As String, username As String)
        _server = server
        _channel = channel
        _userName = username
    End Sub

    Public Sub Connect()
        Dim sCommand As String = String.Empty '-- commands to process from the room.

        Try
            '-- Start the main connection to the IRC server.
            Console.WriteLine("**Creating Connection**")
            _tcpclientConnection = New TcpClient(_server, _port)
            _networkStream = _tcpclientConnection.GetStream
            _reader = New StreamReader(_networkStream)
            _writer = New StreamWriter(_networkStream)

            '-- Send in your information
            Console.WriteLine("**Setting up name**")
            _writer.WriteLine(String.Format("USER {0} {1} * :{2}", _userName, If(_invisible, "8"c, "0"c), _realName))
            _writer.Flush()

            '-- Tell the server you want to connect to a specific room.
            Console.WriteLine("**Joining Room**")
            _writer.WriteLine(String.Format("JOIN {0}", _channel))
            _writer.Flush()

            '-- By now the IDENT should be sent to your port 113.  Listen to it, grab the text, 
            '-- and send a response.
            Ident()

            '-- By now you should be connected to your room and visible to anyone else.  
            '-- If you are receiving errors they are pretty explicit and you can maneuver 
            '-- to debuggin them.
            '-- 
            '-- What happens here is the command processing.  In an infinite loop the bot 
            '-- read in commands and act on them.
            While True
                sCommand = _reader.ReadLine
                Console.WriteLine(sCommand)

                '-- Not the best method but for the time being it works.  
                '-- 
                '-- Example of a command it picks up
                ' :nodi123!nodi12312@ipxxx-xx.net PRIVMSG #nodi123_test :? hola!
                '-- You can extend the program to better read the lines!
                Dim sCommandParts(sCommand.Split(" "c).Length) As String
                sCommandParts = sCommand.Split(" "c)

                '-- Occasionally the IRC server will ping the app.  If it doesn't respond in an 
                '-- appropriate amount of time the connection is closed.
                '-- How does one respond to a ping, but with a pong! (and the hash it sends)
                If sCommandParts(0) = "PING" Then
                    Dim sPing As String = String.Empty
                    For i As Int32 = 1 To sCommandParts.Length - 1
                        sPing += sCommandParts(i) + " "
                    Next
                    _writer.WriteLine("PONG " + sPing)
                    _writer.Flush()
                    Console.WriteLine("PONG " + sPing)
                End If

                '-- With my jank split command we want to look for specific commands sent and react to them!
                '-- In theory this should be dumped to a method, but for this small tutorial you can see them here.
                '-- Also any user can input this.  If you want to respond to commands from you only you would 
                '-- have to extend the program to look for your non-bot-id in the sCommandParts(0)
                If sCommandParts.Length >= 4 Then
                    '-- If a statement is proceeded by a question mark (the semi colon's there automatically) 
                    '-- then repeat the rest of the string!
                    If sCommandParts(3).StartsWith(":?") Then
                        Dim sVal As String = String.Empty
                        Dim sOut As String = String.Empty
                        '-- the text might have other spaces in them so concatenate the rest of the parts 
                        '-- because it's all text.
                        For i As Int32 = 3 To sCommandParts.Length - 1
                            sVal += sCommandParts(i)
                            sVal += " "
                        Next
                        '-- remove the :? part.
                        sVal = sVal.Substring(2, sVal.Length - 2)
                        '-- Trim for good measure.
                        sVal = sVal.Trim
                        '-- Send the text back out.  The format is they command to send the text and the room you are in.
                        sOut = String.Format("PRIVMSG {0} : You said '{1}'", _channel, sVal)
                        _writer.WriteLine(sOut)
                        _writer.Flush()
                    End If
                    '-- If you don't quit the bot correctly the connection will be active until a ping/pong is failed.  
                    '-- Even if your programming isn't running!
                    '-- To stop that here's a command to have the bot quit!
                    If sCommandParts(3).Contains(":!Q") Then
                        ' Stop
                        _writer.WriteLine("QUIT")
                        _writer.Flush()
                        Exit Sub
                    End If
                End If
            End While

        Catch ex As Exception
            '-- Any exception quits the bot gracefully.
            Console.WriteLine("Error in Connecting.  " + ex.Message)
            _writer.WriteLine("QUIT")
            _writer.Flush()
        Finally
            '-- close your connections
            _reader.Dispose()
            _writer.Dispose()
            _networkStream.Dispose()
        End Try

    End Sub

    Sub Ident()
        '-- Idents are usually #### , ####
        '-- That is four digits, a space, a comma, and four more digits.  You need to send 
        '-- this back with your user name you connected with and your system.
        Dim identListener As New TcpListener(IPAddress.Any, 113)
        identListener.Start()
        Dim identClient As TcpClient = identListener.AcceptTcpClient
        identListener.Stop()
        Console.WriteLine("ident connection?")
        Dim identNetworkStream As NetworkStream = identClient.GetStream
        Dim identStreamReader As New StreamReader(identNetworkStream)
        Dim IdentResponse As String = identStreamReader.ReadLine
        Console.WriteLine("ident got: " + IdentResponse)
        Dim identStreamWriter As New StreamWriter(identNetworkStream)
        '-- The general format for the IDENT response.  You can use UNIX, WINDOWS VISTA, WINDOWS XP, or what ever your system is.
        identStreamWriter.WriteLine(String.Format("{0} : USERID : WINDOWS 7 : {1}", IdentResponse, _userName))
        identStreamWriter.Flush()
    End Sub
End Class
