using System;
using System.Text;
using EasyTcpClientServer;

namespace Implementation.ClientAddIn
{
    public class RequestProcessGa:RequestProcessBase 
    {
        public RequestProcessGa():base(true,Encoding.ASCII.GetBytes("Hallo"))
        {
            this.NextMessageToSend = Encoding.ASCII.GetBytes( "hallo");
        }
        protected override void Process(byte[] message)
        {
            string msg = Encoding.ASCII.GetString(message);
            switch (msg)
            {
                case "aaa":
                    //Console.WriteLine(@"Message:" + message);
                    this.NextMessageToSend = Encoding.ASCII.GetBytes( "bbb");
                    
                    break;

                case "bbb":
                    this.NextMessageToSend =Encoding.ASCII.GetBytes( "aaa");
                    
                    break;
                default:
                    this.ReturnMessage = Encoding.ASCII.GetBytes("");
                    break;
            }
        }
    }
}
