using System;
using EasyTcpClientServer;
namespace ClientCLI
{
    public class RequestProcessGa:RequestProcessBase 
    {
        public RequestProcessGa():base(true,"Hallo")
        {
            this.NextMessageToSend = "hallo";
        }
        protected override void Process(string message)
        {
            
            switch (message)
            {
                case "aaa":
                    //Console.WriteLine(@"Message:" + message);
                    this.NextMessageToSend = "bbb";
                    
                    break;

                case "bbb":
                    this.NextMessageToSend = "aaa";
                    
                    break;
                default:
                    this.ReturnMessage = "";
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var client = new TCPClient("127.0.0.1", 12221);
            var req = new RequestProcessGa();
            client.RegisterRequestProcess(req);
            AttachEvents(client);
            client.Start();
            Console.WriteLine(@"Press any key to exit!");
            string line;
            do
            {
                line = Console.ReadLine();
                req.NextMessageToSend =   line;
                client.Send();
            } while (line != "exit");

            DetachEvents(client);
            client.Stop();
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
