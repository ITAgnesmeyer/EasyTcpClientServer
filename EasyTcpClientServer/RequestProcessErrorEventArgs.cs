using System;

namespace EasyTcpClientServer
{
    
    public class RequestProcessErrorEventArgs: EventArgs
    {
        public string ExceptionMessage { get; internal set; }
        public string ClientMessage { get; internal set; }

        public RequestProcessErrorEventArgs()
        {
            
        }

        // ReSharper disable once UnusedMember.Global
        public RequestProcessErrorEventArgs(string exceptionMessage, string clientMessage)
        {
            this.ExceptionMessage = exceptionMessage;
            this.ClientMessage = clientMessage;
        }
    }
}