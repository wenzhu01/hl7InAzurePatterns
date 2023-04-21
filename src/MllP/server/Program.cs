using System;
using System.Net;
using System.Net.Sockets;

TcpListener tcpListener;

try
{
    tcpListener = new TcpListener(IPAddress.Any, 1080);

    tcpListener.Start();

    Console.WriteLine("Started listening on port 1080");
}
catch (System.Exception ex)
{
    Console.WriteLine(ex.Message);
    return;
}

var receivedByteBuffer  = new byte[200];

while(true)
{
    TcpClient acceptedClient = null;
    NetworkStream networkStream = null;

    try
    {
        Console.WriteLine("Waiting for client to connect...");

        acceptedClient = await tcpListener.AcceptTcpClientAsync(); // Get client connection
        networkStream = acceptedClient.GetStream(); // Get client stream

        Console.WriteLine("Client connected");

        // Keep receiving data from client until connection is closed
        var totalBytesReceivedFromClient = 0;
        int bytesReceived;

        while ((bytesReceived = await networkStream.ReadAsync(receivedByteBuffer, 0, receivedByteBuffer.Length)) > 0)
        {
            if (networkStream.CanWrite)
            {
                //echo the received data back to the client
                await networkStream.WriteAsync(receivedByteBuffer, 0, bytesReceived);
            }

            totalBytesReceivedFromClient += bytesReceived;
        }   

        Console.WriteLine("Echoed {0} bytes back to the client.", totalBytesReceivedFromClient);     

    }
    catch (System.Exception ex)
    {        
        Console.WriteLine(ex.Message);
    }
    finally 
    {
        networkStream?.Close();
        networkStream?.Dispose();
        acceptedClient?.Close();
    }
}