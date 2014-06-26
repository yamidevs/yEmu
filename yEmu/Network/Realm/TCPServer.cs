using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yEmu.Network
{
    abstract class TCPServer : IDisposable 
    {
        public event Action<ServerManager> Connected;
                      
        public const Int32 Port = 4444;

        protected IPEndPoint LisenAdress;

        protected Socket Sock;

        protected int _maxConnexion = 20;

        protected bool _run = true;

        public bool IsRun
        {
            set { _run = value; }
            get { return _run; }
        }
        public virtual int MaxConnexion
        {
            get { return _maxConnexion; }
            set
            {
                if (value > 0)
                {
                    _maxConnexion = value;
                }
            }
        }
        public virtual int TcpPort
        {
            get { return LisenAdress.Port; }
            set { LisenAdress.Port = value; }
        }
        public virtual IPAddress TcpIP
        {
            get { return LisenAdress.Address; }
            set { LisenAdress.Address = value; }
        }
        public virtual IPEndPoint EndPoint
        {
            get { return LisenAdress; }
            set { LisenAdress = value; }
        }

   
        public async virtual void Start()
        {

            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    Sock.Bind(EndPoint);
                }
                catch
                {
                    Console.WriteLine("PORT INVALIDE ");
                    return;
                }
                Sock.Listen(MaxConnexion);

              await  Task.Factory.StartNew(() => AcceptConnexion());

        }
        public  void AcceptConnexion()
        {
            try
            {
                this.Sock.BeginAccept(new AsyncCallback(this.AcceptCallBack), (object)this.Sock);

            }
            catch(Exception ex){
                Console.WriteLine("ERREURa : " + ex.HResult);
                return;
            }

        }

        private  void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                var data = Connected;
                if (data != null)
                {
                    this.Connected(new ServerManager(this.Sock.EndAccept(ar), 5000));
                }

                this.AcceptConnexion();
            }
            catch
            {
                this.AcceptConnexion();
            }
            
        }
        public void Close()
        {
            lock (this)
            {
                if (Sock != null && Sock.Connected)
                {
                    Sock.Shutdown(SocketShutdown.Both);
                    Sock.Close();

                    Sock = null;
                }
            }
        }

       ~TCPServer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{			
				Close();			
		}
    }
}
