using System;
using EasyTcpClientServer;
namespace ServerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new TCPServer("127.0.0.1", 12221))
            {
                
                server.RegisterFromFolder(".\\server");

                AttachEvents(server);
                server.Start();

                Console.WriteLine("Press any key to exit!");

                Console.ReadKey();
                DetachEvents(server);
            }
          

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
