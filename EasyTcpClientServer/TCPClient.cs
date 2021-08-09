using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace EasyTcpClientServer
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class TCPClient : IDisposable
    {
        private readonly int _WaitTimeOut;
        private readonly int _SendTimeOut;
        private readonly List<RequestProcessBase> _RequestProcesses;

        public List<RequestProcessBase> RequestProcesses
        {
            get => this._RequestProcesses;
        } 
        public string Ip { get; private set; }

        public int Port { get; private set; }

        public TCPClient(string ip, int port)
        {
            this._WaitTimeOut = 1000;
            this._SendTimeOut = 1000;
            this.Ip = ip;
            this.Port = port;

            this._RequestProcesses = new List<RequestProcessBase>();
        }

        private void InitTcpClient()
        {
            this._TcpClient = new TcpClient();
            
            this._TcpClient.Client.Poll(1000, SelectMode.SelectWrite);
            try
            {
                this._TcpClient.Connect(this.Ip, this.Port);
                this._TcpClient.ReceiveTimeout = this._WaitTimeOut;
                this._TcpClient.SendTimeout = this._SendTimeOut;
            }
            catch (Exception ex)
            {
                OnRequestProcessError(null, NewErrorEventArgs(ex));
                //throw;
                if (this._TcpClient != null)
                {
                    if (this._TcpClient.Connected) 
                        this._TcpClient.Close();
                    this._TcpClient.Dispose();
                    this._TcpClient = null;
                }
            }
        }


        private string SendRequest(string cmd)
        {
            string returnValue = null;

            try
            {
                if ((this._TcpClient == null))
                    InitTcpClient();
                if ((this._TcpClient.Connected == false))
                {
                    this._TcpClient?.Dispose();
                    this._TcpClient = null;
                    InitTcpClient();
                }

                byte[] buf = Encoding.ASCII.GetBytes(cmd);
                using (NetworkStream netStream = this._TcpClient.GetStream())
                {
                    // Schreiben der Daten
                    netStream.Write(buf, 0, buf.Length);

                    if (netStream.CanRead)
                    {
                        byte[] bytes = new byte[2049];

                        StringBuilder sb = new StringBuilder();
                        int counter = netStream.Read(bytes, 0, bytes.Length);
                        string data = Encoding.ASCII.GetString(bytes, 0, counter);

                        sb.Append(data);
                        while ((netStream.DataAvailable))
                        {
                            counter = netStream.Read(bytes, 0, bytes.Length);
                            data = Encoding.ASCII.GetString(bytes, 0, counter);
                            sb.Append(data);
                            //Thread.Sleep(100);
                        }

                        returnValue = sb.ToString();
                    }


                    return returnValue;
                }
            }

            catch (Exception ex)
            {
                OnRequestProcessError(null, NewErrorEventArgs(ex));

                return null;
            }
        }


        private void SendRequestMessages()
        {
            foreach (RequestProcessBase requestProcessBase in this._RequestProcesses)
            {
                SendRequestMessage(requestProcessBase);
            }
        }

        private void SendRequestMessage(RequestProcessBase requestProcess)
        {
            string response = SendRequest(requestProcess.NextMessageToSend);
            requestProcess.NextMessageToSend = "";
            if (string.IsNullOrEmpty(response))
                response = "";
            ExecuteRequestProcess(requestProcess, response);
        }

        private void ExecuteRequestProcess(RequestProcessBase requestProcess, string data)
        {
            requestProcess.Start(data);
            if (!requestProcess.Success)
            {
                RequestProcessErrorEventArgs requestProcessErrorEventArg = new RequestProcessErrorEventArgs()
                {
                    ClientMessage = data,
                    ExceptionMessage = requestProcess.ExceptionMessage
                };
                OnRequestProcessError(requestProcess, requestProcessErrorEventArg);
            }
            else
            {
                
                OnRequestProcessSuccess(requestProcess, new RequestProcessSuccessEventArgs()
                {
                    Message = data
                });
                if (!string.IsNullOrEmpty( requestProcess.NextMessageToSend) && data != requestProcess.NextMessageToSend && requestProcess.SendBackToClient)
                    this.Send();
            }
        }

        // ReSharper disable once UnusedMember.Local
        //private void ExecuteRequestProcesses(string data)
        //{
        //    foreach (RequestProcessBase requestProcess in this._RequestProcesses)
        //    {
        //        ExecuteRequestProcess(requestProcess, data);
        //    }
        //}

        private RequestProcessErrorEventArgs NewErrorEventArgs(Exception ex)
        {
            RequestProcessErrorEventArgs requestProcessErrorEventArg = new RequestProcessErrorEventArgs()
            {
                ClientMessage = string.Empty,
                ExceptionMessage = ex.Message
            };
            return requestProcessErrorEventArg;
        }

        protected virtual void OnRequestProcessError(RequestProcessBase obj, RequestProcessErrorEventArgs e)
        {
            RequestProcessError?.Invoke(obj, e);
        }

        protected virtual void OnRequestProcessSuccess(RequestProcessBase obj, RequestProcessSuccessEventArgs e)
        {
            RequestProcessSuccess?.Invoke(obj, e);
        }

        public void RegisterRequestProcess(RequestProcessBase process)
        {
            this._RequestProcesses.Add(process);
        }
        public void RegisterFromFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException("Folder not found:" + folder);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            var files = dirInfo.GetFiles("*.ClientAddIn.dll");
            if (files.Length == 0)
                throw new FileNotFoundException("Could not find DLL's in folder:" + folder);
            foreach (FileInfo fileInfo in files)
            {
                try
                {
                    var assembly = Assembly.LoadFile(fileInfo.FullName);
                    var types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.BaseType == typeof( RequestProcessBase))
                        {
                            try
                            {
                                var obj = (RequestProcessBase)Activator.CreateInstance(type, true);
                                this.RegisterRequestProcess(obj);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                
                            }
                            

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                }
            }

            if (this._RequestProcesses.Count <= 0)
            {
                throw new FileNotFoundException("Could not find any DLL containing RequestProcess - Class");
            }

        }
        public void Send()
        {
            SendRequestMessages();
        }

        public void Start()
        {
            //this.mainThread = new Thread(new ThreadStart(this.StartTCPClient));
            //this.mainThread.Start();
            InitTcpClient();
        }

        public void Stop()
        {
            if (this._TcpClient != null)
            {
                if (this._TcpClient.Connected)
                    this._TcpClient.Close();
                this._TcpClient = null;
            }
        }

        public event EventHandler<RequestProcessErrorEventArgs> RequestProcessError;

        public event EventHandler<RequestProcessSuccessEventArgs> RequestProcessSuccess;

        private TcpClient _TcpClient;

        public void Dispose()
        {
            Stop();
        }
    }
}