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
                //default:
                //    this.ReturnMessage = " ";
                    //break;
            }
        }
    }
}