using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace EasyTcpClientServer
{
    public class EasyNetWorkStream : NetworkStream
    {
        
        public EasyNetWorkStream(Socket socket) : base(socket)
        {
        }

        public EasyNetWorkStream(Socket socket, bool ownsSocket) : base(socket, ownsSocket)
        {
            
        }

        public EasyNetWorkStream(Socket socket, FileAccess access) : base(socket, access)
        {
        }

        public EasyNetWorkStream(Socket socket, FileAccess access, bool ownsSocket) : base(socket, access, ownsSocket)
        {
        }

        public Socket GetSocket()
        {
            return this.Socket;
        }
        public int TryRead(byte[] destination, int startPos, int len)
        {
            try
            {
               return this.Read(destination, startPos , len);
            }
            catch (Exception ex)
            {

                Debug.Print("TryRead:" + ex.Message);
            }
            return -1;
        }
        public bool Connected => this.Socket.Connected;

        public bool IsConnected()
        {
            // This is how you can determine whether a socket is still connected.
            bool blockingState = this.Socket.Blocking;
            try
            {
                byte [] tmp = new byte[1];

                this.Socket.Blocking = false;
                this.Socket.Send(tmp, 0, 0);
                Console.WriteLine("Connected!");
            }
            catch (SocketException e) 
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                    Console.WriteLine("Still Connected, but the Send would block");
                else
                {
                    Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                }
            }
            catch(ObjectDisposedException e)
            {
                Console.WriteLine("Socket Dispased:" + e.Message);
                return false;
            }
            finally
            {
                try
                {
                    this.Socket.Blocking = blockingState;
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Socked error:" + ex.Message);
                }
                
            }

            
            return this.Socket.Connected;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}