using System;
using System.Net;
using System.Net.Sockets;

namespace mllpServer;

public interface IMllpServer
{
    void Initialize();

    void Stop();
    
    Task WaitingForClientAsync();
}

public class MllpServer : IMllpServer
{
    private readonly int PORT;
    private byte[] _receivedByteBuffer;
    private readonly ILogger<MllpServer> _logger;
    private TcpListener _tcpListener;

    private TcpClient acceptedClient = null;
    private NetworkStream networkStream = null;

    public MllpServer(IConfiguration configuration, ILogger<MllpServer> logger)
    {
        if (string.IsNullOrEmpty(configuration["MllpServer:Port"]))
            PORT = 1080;
        else
            PORT = int.Parse(configuration["MllpServer:Port"]);

        _receivedByteBuffer = new byte[200];
        _logger = logger;
    }

    public void Initialize()
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Any, 1080);

            _tcpListener.Start();

            _logger.LogInformation("Started listening on port 1080");
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task WaitingForClientAsync()
    {
        acceptedClient = null;
        networkStream = null;

        try
        {
            _logger.LogInformation("Waiting for client to connect...");

            acceptedClient = await _tcpListener.AcceptTcpClientAsync(); // Get client connection
            networkStream = acceptedClient.GetStream(); // Get client stream

            _logger.LogInformation("Client connected");

            // Keep receiving data from client until connection is closed
            var totalBytesReceivedFromClient = 0;
            int bytesReceived;

            while ((bytesReceived = await networkStream.ReadAsync(_receivedByteBuffer, 0, _receivedByteBuffer.Length)) > 0)
            {
                if (networkStream.CanWrite)
                {
                    //echo the received data back to the client
                    await networkStream.WriteAsync(_receivedByteBuffer, 0, bytesReceived);
                }

                totalBytesReceivedFromClient += bytesReceived;
            }

            _logger.LogInformation("Echoed {0} bytes back to the client.", totalBytesReceivedFromClient);

        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        finally
        {
            networkStream?.Close();
            networkStream?.Dispose();
            acceptedClient?.Close();
        }

    }

    public void Stop()
    {
        try
        {
            networkStream?.Close();
            networkStream?.Dispose();
            acceptedClient?.Close();
        }
        catch
        {

        }
    }
}