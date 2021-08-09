using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace EasyTcpClientServer
{

    public abstract class RequestProcessBase
    {
        public bool SendBackToClient { get; }
        public string ReturnMessage { get; protected set;}

        public bool Success { get; private set; }

        public string ExceptionMessage { get; private set; }
        public string NextMessageToSend{get;set;}
        public RequestProcessBase():this(false,string.Empty){}

        public RequestProcessBase(bool sendBackToClient,string returnMessage)
        {
            this.SendBackToClient = sendBackToClient;
            this.ReturnMessage = returnMessage;
        }

       
        public static void SendMessageToClient(TcpClient client, string message)
        {
            if (!client.Connected)
            {
                Console.WriteLine("SendMessageToClient:client not connected");
                return;
            }
                
            var buf = Encoding.ASCII.GetBytes(message);
            
            var stream = client.GetStream();
            stream.Write(buf, 0, buf.Length);
        }

        internal void Start(string clientMessage)
        {
            try
            {
                Process(clientMessage);
                this.Success = true;
            }
            catch (Exception ex)
            {
                this.ExceptionMessage = ex.Message;
                this.Success = false;
            }
        }

        // ReSharper disable once RedundantAssignment
        public static string GetClientMessage(TcpClient client, ref int length)
        {
            try
            {
                var buf = new byte[client.ReceiveBufferSize];
            
                var stream = client.GetStream();

                length = stream.Read(buf, 0, buf.Length);
                string returnString = Encoding.ASCII.GetString(buf);
                return CleanUpNts(returnString);

            }
            catch (Exception e)
            {
                Debug.Print( "Error:" + e.Message);
                length = 0;
                return "";
            }
        }

        private static string CleanUpNts(string returnString)
        {
            returnString = returnString.Replace("\0", string.Empty);
            return returnString;
        }

     

        protected abstract void Process(string message);

    }
}
