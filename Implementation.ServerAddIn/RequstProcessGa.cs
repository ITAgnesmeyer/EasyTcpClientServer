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
            string msg = this.Encoding.GetString(message);
            switch (msg)
            {
                case "aaa":
                    Console.WriteLine("Message:" + msg);
                    this.ReturnMessage = this.Encoding.GetBytes("Alles ist gut");
                    this.ReturnMessages.Enqueue(this.Encoding.GetBytes("Alles ist nicht gut"));
                    break;

                case "bbb":
                    this.ReturnMessage = this.Encoding.GetBytes("Nicht implementiert");
                    break;
                case "ccc":
                    this.ReturnMessage = this.Encoding.GetBytes("aaa");
                    break;
                case "ddd":
                    this.ReturnMessage = this.Encoding.GetBytes("bbb");
                    break;
                //default:
                //    this.ReturnMessage = " ";
                //break;
            }
        }
    }
}
