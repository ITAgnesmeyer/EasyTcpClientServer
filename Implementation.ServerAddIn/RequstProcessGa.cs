using System;
using System.Text;
using EasyTcpClientServer;

namespace Implementation.ServerAddIn
{
    public class RequstProcessGa:RequestProcessBase 
    {
        public RequstProcessGa():base(true,Encoding.ASCII.GetBytes("Hallo"))
        {
            
        }
        protected override void Process(byte[] message)
        {
            string msg = Encoding.ASCII.GetString(message);
            switch (msg)
            {
                case "aaa":
                    Console.WriteLine("Message:" + msg);
                    this.ReturnMessage = Encoding.ASCII.GetBytes("Alles ist gut");
                    break;

                case "bbb":
                    this.ReturnMessage = Encoding.ASCII.GetBytes("Nicht implementiert");
                    break;
                case "ccc":
                    this.ReturnMessage = Encoding.ASCII.GetBytes("aaa");
                    break;
                case "ddd":
                    this.ReturnMessage = Encoding.ASCII.GetBytes("bbb");
                    break;
                //default:
                //    this.ReturnMessage = " ";
                //break;
            }
        }
    }
}
