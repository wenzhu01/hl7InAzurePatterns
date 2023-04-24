using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Contoso
{

    public class SendHl7Message
    {

        private char END_OF_BLOCK = '\u001c';
        private char START_OF_BLOCK = '\u000b';
        private char CARRIAGE_RETURN = (char)13;

        [FunctionName("SendHl7Message")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            try
            {
                int port = Environment.GetEnvironmentVariable("port") != null ? int.Parse(Environment.GetEnvironmentVariable("port")) : 1180;

                log.LogInformation("Client connecting on port: " + port);

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

                TcpClient tcpClient = new ();

                await tcpClient.ConnectAsync(new IPEndPoint(IPAddress.Loopback, port));  
                var byteBuffer = Encoding.UTF8.GetBytes(testHl7MessageToTransmit.ToString());

                await tcpClient.GetStream().WriteAsync(byteBuffer, 0, byteBuffer.Length);

                log.LogInformation("Data was sent data to server successfully....");

                var buffer = new byte[1_024];
                var received = await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
                var response = Encoding.UTF8.GetString(buffer, 0, received);                                     

            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);                
                return new BadRequestObjectResult(ex.Message);
            }

            string responseMessage = "Message sent to MLLP Server successfully!";

            return new OkObjectResult(responseMessage);
        }
    }
}
