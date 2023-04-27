using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Azure.Messaging.ServiceBus;
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

    private readonly char END_OF_BLOCK = '\u001c';
    private readonly char START_OF_BLOCK = '\u000b';
    private readonly char CARRIAGE_RETURN = (char)13;

    private readonly int MESSAGE_CONTROL_ID_LOCATION = 9;

    private readonly char FIELD_DELIMITER = '|';

// the client that owns the connection and can be used to create senders and receivers
private ServiceBusClient client;

// the sender used to publish messages to the queue
private ServiceBusSender sender;

    public MllpServer(IConfiguration configuration, ILogger<MllpServer> logger)
    {
        if (string.IsNullOrEmpty(configuration["MllpServer:Port"]))
            PORT = 1080;
        else
            PORT = int.Parse(configuration["MllpServer:Port"]);

        _receivedByteBuffer = new byte[200];
        _logger = logger;

        var clientOptions = new ServiceBusClientOptions()
        { 
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        
        sender = client.CreateSender("topic01");

    }

    public void Initialize()
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Any, PORT);

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
            string hl7Data = string.Empty;

            while ((bytesReceived = await networkStream.ReadAsync(_receivedByteBuffer, 0, _receivedByteBuffer.Length)) > 0)
            {

                hl7Data += Encoding.UTF8.GetString(_receivedByteBuffer, 0, bytesReceived);

                // Find start of MLLP frame, a VT character ...
                var startOfMllpEnvelope = hl7Data.IndexOf(START_OF_BLOCK);
                if (startOfMllpEnvelope >= 0)
                {
                    // Now look for the end of the frame, a FS character
                    var end = hl7Data.IndexOf(END_OF_BLOCK);
                    if (end >= startOfMllpEnvelope) //end of block received
                    {
                        //if both start and end of block are recognized in the data transmitted, then extract the entire message
                        var hl7MessageData = hl7Data.Substring(startOfMllpEnvelope + 1, end - startOfMllpEnvelope);
//string serializedMsg = JsonConvert.SerializeObject(hl7MessageData); 
var msg = new ServiceBusMessage(hl7MessageData); //serializedMsg); 
await sender.SendMessageAsync(msg);
                        //create a HL7 acknowledgement message
                        var ackMessage = GetSimpleAcknowledgementMessage(hl7MessageData);
                        hl7Data="";
                        Console.WriteLine(ackMessage);

                        //echo the received data back to the client
                        var buffer = Encoding.UTF8.GetBytes(ackMessage);

                        if (networkStream.CanWrite)
                        {
                            await networkStream.WriteAsync(buffer, 0, buffer.Length);

                            Console.WriteLine("Ack message was sent back to the client...");
                        }
                    }
                }

                // if (networkStream.CanWrite)
                // {
                //     //echo the received data back to the client
                //     await networkStream.WriteAsync(_receivedByteBuffer, 0, bytesReceived);
                // }

                // totalBytesReceivedFromClient += bytesReceived;
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

    private string GetSimpleAcknowledgementMessage(string incomingHl7Message) 
    {
        if (string.IsNullOrEmpty(incomingHl7Message))
            throw new ApplicationException("Invalid HL7 message for parsing operation. Please check your inputs");

        //retrieve the message control ID of the incoming HL7 message
        var messageControlId = GetMessageControlID(incomingHl7Message);

        //build an acknowledgement message and include the control ID with it
        var ackMessage = new StringBuilder();
        ackMessage = ackMessage.Append(START_OF_BLOCK)
            .Append("MSH|^~\\&|||||||ACK||P|2.2")
            .Append(CARRIAGE_RETURN)
            .Append("MSA|AA|")
            .Append(messageControlId)
            .Append(CARRIAGE_RETURN)
            .Append(END_OF_BLOCK)
            .Append(CARRIAGE_RETURN);

        return ackMessage.ToString();
    }    

    private string GetMessageControlID(string incomingHl7Message) 
    {

            var fieldCount = 0;
            //parse the message into segments using the end of segment separter
            var hl7MessageSegments = incomingHl7Message.Split(CARRIAGE_RETURN);

            //tokenize the MSH segment into fields using the field separator
            var hl7FieldsInMshSegment = hl7MessageSegments[0].Split(FIELD_DELIMITER);

            //retrieve the message control ID in order to reply back with the message ack
            foreach (var field in hl7FieldsInMshSegment)
            {
                if (fieldCount == MESSAGE_CONTROL_ID_LOCATION)
                {
                    return field;
                }
                fieldCount++;
            }

            return string.Empty; //you can also throw an exception here if you wish
        }    
    }