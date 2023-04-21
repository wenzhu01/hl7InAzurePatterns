using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

char END_OF_BLOCK = '\u001c';
char START_OF_BLOCK = '\u000b';
char CARRIAGE_RETURN = (char)13;

TcpClient tcpClient = new TcpClient();

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

    var byteBuffer = Encoding.UTF8.GetBytes(testHl7MessageToTransmit.ToString());

    await tcpClient.GetStream().WriteAsync(byteBuffer, 0, byteBuffer.Length);

    Console.WriteLine("Data was sent data to server successfully....");

    var buffer = new byte[1_024];
    var received = await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
    var response = Encoding.UTF8.GetString(buffer, 0, received);

    Console.WriteLine("Received message from server: {0}", response);

    Console.WriteLine("Press any key to exit program...");
    Console.ReadLine();      
    
}
catch (System.Exception ex)
{    
    Console.WriteLine(ex.Message);
}
finally 
{
    tcpClient.Close();
}
