using System;
using EasyTcpClientServer;
namespace ServerCLI
{
    public class RequstProcessGa:RequestProcessBase 
    {
        public RequstProcessGa():base(true,"Hallo")
        {
            
        }
        protected override void Process(string message)
        {
            switch (message)
            {
                case "aaa":
                    Console.WriteLine("Message:" + message);
                    this.ReturnMessage = "Alles ist gut";
                    break;

                case "bbb":
                    this.ReturnMessage = "Nicht implementiert";
                    break;
                case "ccc":
                    this.ReturnMessage = "aaa";
                    break;
                case "ddd":
                    this.ReturnMessage = "bbb";
                    break;
                default:
                    this.ReturnMessage = " ";
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var server = new TCPServer("127.0.0.1", 12221);
            var req = new RequstProcessGa();
            server.RegisterRequestProcess(req);

            AttachEvents(server);
            server.Start();

            Console.WriteLine("Press any key to exit!");

            Console.ReadKey();
            DetachEvents(server);

            server.Stop();
        }

        static void AttachEvents(TCPServer server)
        {
            server.RequestProcessError += OnError;
            server.RequestProcessSuccess += OnSuccess;
        }


        private static void OnSuccess(object sender, RequestProcessSuccessEventArgs e)
        {
            Console.WriteLine("Success:" + e.Message);
        }

        static void DetachEvents(TCPServer server)
        {
            server.RequestProcessError -= OnError;
            server.RequestProcessSuccess -= OnSuccess;

        }

        private static void OnError(object sender, RequestProcessErrorEventArgs e)
        {
            if (sender == null && e.ClientMessage.StartsWith("-!"))
            {  }
            else
                Console.WriteLine("Message:" + e.ClientMessage + "Exception:" + e.ExceptionMessage);
        }
    }
}
