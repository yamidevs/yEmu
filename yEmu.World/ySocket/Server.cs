using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Collections;

namespace yEmu.Network
{
    public abstract class Server : IDisposable
    {
        public event Action<Client> Connected;

        private  Socket _ysocket;

        protected  int _connexionMax = 2000;

        protected int _listenMax;

        protected bool _run;

        public static int BufferSize = 8192;

        protected IPEndPoint AdderssPort;

        public abstract Client CreateClient(Socket sock , Server server);

        public  ConcurrentList<Client> Clients
        {
            get;
            set;
        }

        public static int? IpMax = new int?(10);

        public virtual bool IsRun
        {
            get
            {
                return _run;
            }
            set
            {
                _run = value;
            }
        }

        public virtual int IsListen
        {
            get
            {
                return _listenMax;
            }
            set
            {
                _listenMax = value;
            }
        }

        public virtual int Port
        {
            get
            {
                return AdderssPort.Port;
            }
            set
            {
                AdderssPort.Port = value;
            }
        }

        public virtual IPAddress Address
        {
            get
            {
                return AdderssPort.Address;
            }
            set
            {
                AdderssPort.Address = value;
            }
        }

        public virtual IPEndPoint IPEndPoint
        {
            get
            {
                return AdderssPort;
            }
            set
            {
                AdderssPort = value;
            }
        }

        public Server()
        {
            Clients = new ConcurrentList<Client>();
        }
        public  void Start()
        {
          
            _ysocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                   _ysocket.Bind(IPEndPoint);
                }
                catch
                {
                    Console.WriteLine(" Connexion a IP/PORT impossible ");
                //    Stop();
                }

                _ysocket.Listen(_listenMax);

                StartAccept(null);
            
           
        }

        protected  void Stop()
        {
            try
            {
                RemoveAllClient();
                _ysocket.Close(60);
            }
            catch
            {
                Console.WriteLine("Impossible d'arreter la connexion ");
            }
            _ysocket = null;
        }

        public bool ConnectedClient(Client messages)
        {
            var data = this.Connected;
            if (data != null)
            {
                data(messages);
            }
            return true;  
        }

        protected void StartAccept(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += AcceptEventCompleted;
            }
            else
            {
                args.AcceptSocket = null;
            }

            bool willRaiseEvent = _ysocket.AcceptAsync(args);
            if (!willRaiseEvent)
            {
                ProcessAccept(args);
            }
        }

        private void AcceptEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);     
        }

        private void ProcessAccept(SocketAsyncEventArgs args)
        {

        /*    if (Server.IpMax.HasValue && this.CountIP(((IPEndPoint)args.AcceptSocket.RemoteEndPoint).Address) > Server.IpMax.Value) 
            {
                Console.WriteLine("IP adresse : " + ((IPEndPoint)args.AcceptSocket.RemoteEndPoint).Address + " a été limitée");
                return;
            }
*/
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
             Client client = CreateClient(args.AcceptSocket,this);


             Clients.Add(client);
                client.Receive();
                StartAccept(args);                          
        }

        public void Disconnect(Client client)
        {
            RemoveClient(client);

            client.Dispose();
        }

        public void RemoveClient(Client client)
        {
            Clients.Remove(client);
        }

        public void RemoveAllClient()
        {
            foreach (var client in Clients)
            {
                try
                {
                    Disconnect(client);

                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERREUR : "+ e.ToString());
                }
            }

            Clients.Clear();
        }

        public int CountIP(IPAddress ipAddress)
        {
            return Enumerable.Count<Client>((IEnumerable<Client>)this.Clients, (Func<Client, bool>)(t => t.Sock != null && t.Sock.Connected && ((object)((IPEndPoint)t.Sock.RemoteEndPoint).Address).Equals((object)ipAddress)));
        }

		~Server()
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
                Stop();			
		}

        

    }
}
