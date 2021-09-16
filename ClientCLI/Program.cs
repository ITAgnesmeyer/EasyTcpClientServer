using System;
using System.Linq;
using System.Text;
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
                    req.NextMessageToSend = Array.Empty<byte>();
                    if (line != null)
                    {
                        req.NextMessageToSend =  Encoding.ASCII.GetBytes( line);
                    }
                    
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
            string msg = Encoding.ASCII.GetString(e.Message);
            Console.WriteLine(@"Success:" + msg);
        }

        static void DetachEvents(TCPClient server)
        {
            server.RequestProcessError -= OnError;
            server.RequestProcessSuccess -= OnSuccess;

        }

        private static void OnError(object sender, RequestProcessErrorEventArgs e)
        {
            if (e.ClientMessage == null)
                return;
            string msg = Encoding.ASCII.GetString(e.ClientMessage);
            if (sender == null && msg.StartsWith("-!"))
            {  }
            else
                Console.WriteLine(@"Message:" + msg + @"Exception:" + e.ExceptionMessage);
        }
    }
}
