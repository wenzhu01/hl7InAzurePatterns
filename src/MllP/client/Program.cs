using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

char END_OF_BLOCK = '\u001c';
char START_OF_BLOCK = '\u000b';
char CARRIAGE_RETURN = (char)13;


TcpClient tcpClient = new TcpClient();
NetworkStream networkStream = null ;

var testHl7MessageToTransmit = new StringBuilder();
testHl7MessageToTransmit.Append(START_OF_BLOCK)
        .Append("MSH|^~\\&|AcmeHIS|StJohn|CATH|StJohn|20061019172719||ORM^O01|MSGID12349876|P|2.3")
        .Append(CARRIAGE_RETURN)
        .Append("PID|||20301||Durden^Tyler^^^Mr.||19700312|M|||88 Punchward Dr.^^Los Angeles^CA^11221^USA|||||||")
        .Append(CARRIAGE_RETURN)
        .Append("PV1||O|OP^^||||4652^Paulson^Robert|||OP|||||||||9|||||||||||||||||||||||||20061019172717|20061019172718")
        .Append(CARRIAGE_RETURN)
        .Append("ORC|NW|20061019172719")
        .Append(CARRIAGE_RETURN)
        .Append("OBR|1|20061019172719||76770^Ultrasound: retroperitoneal^C4|||12349876")
        .Append(CARRIAGE_RETURN)
        .Append(END_OF_BLOCK)
        .Append(CARRIAGE_RETURN);
try
{
    //initiate a TCP client connection to local loopback address at port 1080
    await tcpClient.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 1080));

    Console.WriteLine("Connected to server");

    // Get a client stream for reading and writing.
    networkStream = tcpClient.GetStream();

    //use UTF-8 and either 8-bit encoding due to MLLP-related recommendations
    //var messageToTransmit = Encoding.UTF8.GetBytes(testHl7MessageToTransmit.ToString());
    var byteBuffer = Encoding.UTF8.GetBytes(testHl7MessageToTransmit.ToString());

    //send a message through this connection using the IO stream
    networkStream.Write(byteBuffer, 0, byteBuffer.Length);    

    Console.WriteLine("Data was sent data to server successfully....");

    var receiveMessageByteBuffer = Encoding.UTF8.GetBytes(testHl7MessageToTransmit.ToString());
    var bytesReceivedFromServer = await networkStream.ReadAsync(receiveMessageByteBuffer, 0, receiveMessageByteBuffer.Length);

    while (bytesReceivedFromServer > 0)
    {
        if (networkStream.CanRead)
        {
            bytesReceivedFromServer = await networkStream.ReadAsync(receiveMessageByteBuffer, 0, receiveMessageByteBuffer.Length);
            if (bytesReceivedFromServer == 0)
            {
                break;
            }
        }
        
    }
    var receivedMessage = Encoding.UTF8.GetString(receiveMessageByteBuffer);

    Console.WriteLine("Received message from server: {0}", receivedMessage);
    // var bytesReceivedFromServer = await networkStream.ReadAsync(byteBuffer, 0, byteBuffer.Length);
    // // Our server for this example has been designed to echo back the message
    // // keep reading from this stream until the message is echoed back
    // while (bytesReceivedFromServer < byteBuffer.Length)
    // {
    //     bytesReceivedFromServer = await networkStream.ReadAsync(byteBuffer, 0, byteBuffer.Length);
    //     if (bytesReceivedFromServer == 0)
    //     {
    //         //exit the reading loop since there is no more data
    //         break;
    //     }
    // }  

    // var receivedMessage = Encoding.UTF8.GetString(byteBuffer);

    Console.WriteLine("Press any key to exit program...");
    Console.ReadLine();      
    
}
catch (System.Exception ex)
{    
    Console.WriteLine(ex.Message);
}
finally 
{
    networkStream?.Close();
    tcpClient.Close();
}
