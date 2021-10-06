using System;
using System.Text;
using EasyTcpClientServer;

namespace Implementation.ClientAddIn
{
    public class RequestProcessGa:RequestProcessBase 
    {
        public RequestProcessGa():base(true,Encoding.ASCII.GetBytes("Hallo"))
        {
            this.NextMessageToSend = this.Encoding.GetBytes( "hallo");
        }

       

        protected override void Process(byte[] message)
        {
            string msg = Encoding.ASCII.GetString(message);
            switch (msg)
            {
                case "aaa":
                    //Console.WriteLine(@"Message:" + message);
                    this.NextMessageToSend = this.Encoding.GetBytes( "bbb");
                    this.ReturnMessages.Add(new byte[] { 0x80, 0x00, 0x00 });
                    break;

                case "bbb":
                    this.NextMessageToSend =this.Encoding.GetBytes( "aaa");
                    
                    break;
                default:
                    this.ReturnMessage = this.Encoding.GetBytes("");
                    break;
            }
        }
    }
}
