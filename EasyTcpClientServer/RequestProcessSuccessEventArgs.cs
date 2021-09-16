using System;

namespace EasyTcpClientServer
{
    public class RequestProcessSuccessEventArgs:EventArgs
    {
        public byte[] Message { get; set; }

        public RequestProcessSuccessEventArgs()
        {
            
        }

        // ReSharper disable once UnusedMember.Global
        public RequestProcessSuccessEventArgs(byte[] message)
        {
            this.Message = message;
        }
    }
}