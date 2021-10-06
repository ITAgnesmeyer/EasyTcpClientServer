using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace EasyTcpClientServer
{

    public abstract class RequestProcessBase
    {
        public bool SendBackToClient { get; }
        public byte[] ReturnMessage { get; protected set; }
        public List<byte[]> ReturnMessages { get; } 
        public bool Success { get; private set; }

        public string ExceptionMessage { get; private set; }
        public byte[] NextMessageToSend { get; set; }
        public RequestProcessBase() : this(false, Array.Empty<byte>()) { }

        public RequestProcessBase(bool sendBackToClient, byte[] returnMessage)
        {
            this.ReturnMessages = new List<byte[]>();
            this.SendBackToClient = sendBackToClient;
            this.ReturnMessage = returnMessage;
        }


        public static void SendMessageToClient(TCPClient client, byte[] message)
        {
            if (!client.Connected)
            {
                Console.WriteLine("SendMessageToClient:client not connected");
                return;
            }

            var stream = client.GetStream();
            stream.Write(message, 0, message.Length);
        }

        internal void Start(byte[] clientMessage)
        {
            try
            {
                string str = this.Encoding.GetString(clientMessage);
                Process(str);
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
        public static byte[] GetClientMessage(TCPClient client, ref int length)
        {
            try
            {
                var buf = new byte[client.ReceiveBufferSize];

                var stream = client.GetStream();

                length = stream.Read(buf, 0, buf.Length);
                byte[] buff = new byte[length];
                Array.Copy(buf, 0, buff, 0, buff.Length);
                return buff;



            }
            catch (Exception e)
            {
                Debug.Print("Error:" + e.Message);
                length = 0;
                return Array.Empty<byte>();
            }
        }

        private static string CleanUpNts(string returnString)
        {
            returnString = returnString.Replace("\0", string.Empty);
            return returnString;
        }

        protected virtual Encoding Encoding => Encoding.ASCII;

        protected virtual void Process(string message)
        {

        }

        protected abstract void Process(byte[] message);

    }
}
