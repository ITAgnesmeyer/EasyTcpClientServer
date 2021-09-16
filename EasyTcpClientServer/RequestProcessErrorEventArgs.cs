using System;

namespace EasyTcpClientServer
{
    
    public class RequestProcessErrorEventArgs: EventArgs
    {
        public string ExceptionMessage { get; internal set; }
        public byte[] ClientMessage { get; internal set; }

        public RequestProcessErrorEventArgs()
        {
            
        }

        // ReSharper disable once UnusedMember.Global
        public RequestProcessErrorEventArgs(string exceptionMessage, byte[] clientMessage)
        {
            this.ExceptionMessage = exceptionMessage;
            this.ClientMessage = clientMessage;
        }
    }
}