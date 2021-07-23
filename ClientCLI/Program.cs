using System;
using System.Linq;
using EasyTcpClientServer;
namespace ClientCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new TCPClient("127.0.0.1", 12221))
            {
               
                client.RegisterFromFolder(".\\");
                AttachEvents(client);
                client.Start();
                Console.WriteLine(@"Enter exit to quit!");
                var req = client.RequestProcesses.First();
                string line;
                do
                {
                    line = Console.ReadLine();
                    req.NextMessageToSend =   line;
                    client.Send();
                } while (line != "exit");

                DetachEvents(client);

            }
            
        }

        static void AttachEvents(TCPClient server)
        {
            server.RequestProcessError += OnError;
            server.RequestProcessSuccess += OnSuccess;
        }


        private static void OnSuccess(object sender, RequestProcessSuccessEventArgs e)
        {
            
            Console.WriteLine(@"Success:" + e.Message);
        }

        static void DetachEvents(TCPClient server)
        {
            server.RequestProcessError -= OnError;
            server.RequestProcessSuccess -= OnSuccess;

        }

        private static void OnError(object sender, RequestProcessErrorEventArgs e)
        {
            if (sender == null && e.ClientMessage.StartsWith("-!"))
            {  }
            else
                Console.WriteLine(@"Message:" + e.ClientMessage + @"Exception:" + e.ExceptionMessage);
        }
    }
}
