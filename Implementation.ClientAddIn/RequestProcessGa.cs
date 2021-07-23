using System;
using EasyTcpClientServer;

namespace Implementation.ClientAddIn
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
}
