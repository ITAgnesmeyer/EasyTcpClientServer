﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EasyTcpClientServer
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class TCPServer
    {
        #region Fields.

        private readonly List<RequestProcessBase> _RequestProcesses;
        private readonly TcpListener _Listener;
        private bool _Stop;
        private bool _IsRunning;

        #endregion

        #region Public.

        public event EventHandler<RequestProcessErrorEventArgs> RequestProcessError;
        public event EventHandler<RequestProcessSuccessEventArgs> RequestProcessSuccess;

        public int Port { get; private set; }
        public string Ip { get; private set; }
        public bool IsRunning => this._IsRunning;

        /// <summary>
        /// Create a new instance of TcpServer
        /// </summary>
        /// <remarks>Try to get the IP from the local mchine.</remarks>
        /// <param name="port">Port of the Server</param>
        public TCPServer(int port) : this(GetLocalIpAddress(), port)
        {
        }

        /// <summary>
        /// Create new instance of TcpServer.
        /// </summary>
        /// <param name="ip">Ip of server.</param>
        /// <param name="port">Port of server.</param>
        public TCPServer(string ip, int port)
        {
            this._IsRunning = false;
            this.Ip = ip;
            this.Port = port;
            this._RequestProcesses = new List<RequestProcessBase>();
            this._Listener = new TcpListener(IPAddress.Parse(this.Ip), this.Port);
        }

        protected virtual void OnRequestProcessSuccess(RequestProcessBase obj, RequestProcessSuccessEventArgs e)
        {
            RequestProcessSuccess?.Invoke(obj, e);
        }

        protected virtual void OnRequestProcessError(RequestProcessBase obj, RequestProcessErrorEventArgs e)
        {
            RequestProcessError?.Invoke(obj, e);
        }

        /// <summary>
        /// Starts receiving incoming requests.
        /// </summary>
        public void Start()
        {
            this._Stop = false;
            this._Listener.Start();
            this._Listener.BeginAcceptTcpClient(ProcessRequest, this._Listener);
            this._IsRunning = true;
        }

        public void RegisterRequestProcess(RequestProcessBase process)
        {
            this._RequestProcesses.Add(process);
        }

        /// <summary>
        /// Stops receiving incoming requests.
        /// </summary>
        public void Stop()
        {
            //If listening has been cancelled, simply go out from method.
            if (this._Stop)
            {
                return;
            }

            //Cancels listening.
            this._Stop = true;

            //Waits a little, to guarantee that all operation receive information about cancellation.
            Thread.Sleep(100);
            this._Listener.Stop();
        }

        #endregion

        #region Private.

        //Process single request.
        private void ProcessRequest(IAsyncResult ar)
        {
            //Stop if operation was cancelled.
            if (this._Stop)
            {
                return;
            }

            if (!(ar.AsyncState is TcpListener listener))
            {
                return;
            }

            //Check cancellation again. Stop if operation was cancelled.
            if (this._Stop)
            {
                return;
            }

            //Starts waiting for the next request.
            listener.BeginAcceptTcpClient(ProcessRequest, listener);

            TcpClient client = listener.EndAcceptTcpClient(ar);

            while (client.Connected == false)
            {
                Thread.Sleep(10);
            }

            while (client.Connected)
            {
                int bytesRead = 0;
                string message = RequestProcessBase.GetClientMessage(client, ref bytesRead);
                if (bytesRead == 0)
                {
                    client.Close();
                    break;
                }

                if (message != string.Empty)
                    ExecuteRequestProcess(client, message);
            }
        }

        private void ExecuteRequestProcess(TcpClient client, string message)
        {
            foreach (var item in this._RequestProcesses)
            {
                item.Start(message);
                if (item.Success)
                {
                    if (item.SendBackToClient)
                        RequestProcessBase.SentMessageToClient(client, item.ReturnMessage);

                    RequestProcessSuccessEventArgs e = new RequestProcessSuccessEventArgs {Message = message};
                    OnRequestProcessSuccess(item, e);
                }
                else
                {
                    RequestProcessErrorEventArgs e = new RequestProcessErrorEventArgs
                    {
                        ClientMessage = message, ExceptionMessage = item.ExceptionMessage
                    };
                    OnRequestProcessError(item, e);
                }
            }
        }

        #endregion

        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine(ip.ToString());
                    return ip.ToString();
                }
            }

            throw new Exception("Local IP Address Not Found!");
        }
    }
}