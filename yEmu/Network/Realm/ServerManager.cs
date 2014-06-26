using ServerToolkit.BufferManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace yEmu.Network
{
    class ServerManager
    {
        public event Action<byte[]> DataReceive;
        BufferPool Pool;
        private int Size_buffer = 8192;
        public bool Run
        {
            get
            {
                return true;
            }
            set
            {
                Run = value;
            }
        }
        public Socket Sock
        {
            get;
            set;
        }
        private object Lock = new object();

        public delegate void NotifictionClose();
        public event NotifictionClose OnSocketClose;


        public ServerManager()
        {

        }
        public ServerManager(Socket sock, int size)
        {
            Sock = sock;
            Pool = new BufferPool(1 * 1024 * 1024, 1, 1);
            this.Received();
        }

        public void OnClose()
        {

            var data = OnSocketClose;
            if (data != null)
            {
                data();
            }
            Run = false;

        }
        public void OnSocketClosed()
        {

            lock (this.Lock)
            {
                try
                {
                    this.Sock.Close();
                }
                catch { }
            }
        }

        public void Received()
        {
            try
            {
                if (Run)
                {
                    var buffer = Pool.GetBuffer(Size_buffer);
                    this.Sock.BeginReceive(buffer.GetSegments(), SocketFlags.None, OnReceived, buffer);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERREURs : " + e.Message);
            }
        }

        private void OnReceived(IAsyncResult ar)
        {
            if (!Sock.Connected)
                return;

         
            var recvBuffer = (IBuffer)ar.AsyncState;
            int bytesRead = 0;
            try
            {
                bytesRead = this.Sock.EndReceive(ar);
                byte[] data = new byte[bytesRead > 0 ? bytesRead : 0];

                if (bytesRead > 0)
                {
                    recvBuffer.CopyTo(data, 0, bytesRead);
                    this.DataReceive(data);
                    this.Received();
                }
                else
                {
                    this.OnClose();
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.HResult);
            }
            finally
            {
                if (recvBuffer != null)
                {
                    recvBuffer.Dispose();
                }
            }

        }

        public string ip(ServerManager s)
        {
            IPEndPoint remoteIpEndPoint = s.Sock.RemoteEndPoint as IPEndPoint;
            return remoteIpEndPoint.Address.ToString();
        }
    }
}


