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

        private object Lock = new object();

        public delegate void NotifictionClose();
        public event NotifictionClose OnSocketClose;

        private byte[] _buffer = new byte[3004];

        public Socket _sock;

        public ServerManager()
        {

        }
        public ServerManager(Socket sock, int size)
        {
            _sock = sock;
            this._buffer = new byte[size];
            this.Received();


        }
        public void OnClose()
        {
            var data = OnSocketClose;
            if (data != null)
            {
                data();
            }
        }
        public void OnSocketClosed(){

            lock (this.Lock)
            {
                try
                {
                    this._sock.Close();
                }
                catch { }
            }
        }
        
        public void Received()
        {
            try
            {
                this._sock.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceived), (object)this._sock);

            }catch(Exception e){

            }
        }

        private void OnReceived(IAsyncResult ar)
        {
            if (!_sock.Connected) 
                return;
            lock (Lock)
            {
                try
                {
                    int rcv = this._sock.EndReceive(ar);

                    if (rcv > 0)
                    {
                        byte[] bytes = new byte[rcv];

                        for (int b = 0; b <= rcv - 1; ++b)
                            bytes[b] = this._buffer[b];

                        this.DataReceive(bytes);
                        this._buffer = new byte[5000];
                        this.Received();
                    }
                    else
                        this.OnClose();
                }
                catch
                {
                    this.OnClose();
                }
            }
        }

        public string ip(ServerManager s)
        {
            IPEndPoint remoteIpEndPoint = s._sock.RemoteEndPoint as IPEndPoint;
            return remoteIpEndPoint.Address.ToString();
        }
    }
}
