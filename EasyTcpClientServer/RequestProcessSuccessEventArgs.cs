using System;

namespace EasyTcpClientServer
{
    public class RequestProcessSuccessEventArgs:EventArgs
    {
        public string Message { get; set; }

        public RequestProcessSuccessEventArgs()
        {
            
        }

        // ReSharper disable once UnusedMember.Global
        public RequestProcessSuccessEventArgs(string message)
        {
            this.Message = message;
        }
    }
}