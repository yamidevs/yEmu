using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm;
using yEmu.Realm.Packets;
using yEmu.Util;

namespace yEmu.Network
{
    class Client : IDisposable
    {
        public int CountPackets
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }
        public Processor Processor
        {
            get;
            set;
        }

        public ServerManager ServerManager
        {
            get;
            set;
        }
        public bool Connected
        {
            get;
            set;
        }
        public bool IsConnected
        {
            get { return ServerManager.Sock != null && ServerManager.Sock.Connected; }
        }
        public Client(ServerManager _sock )
        {
            ServerManager = _sock;
            ServerManager.DataReceive += this.DataArrivals;
            ServerManager.OnSocketClose += this.OnSocketClose;
            this.HelloClient();
            Processor = new Processor(this);
        }
        public void DataArrivals(byte[] data)
        {          
                foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
                {
                    Info.Write("realmR", packet, ConsoleColor.White);
                    Processor.Parser(packet);
                }            
        }

        public void OnSocketClose()
        {
            Info.Write("realmR", "DECONNEXION UTILISATEUR", ConsoleColor.White);
            //_server.OnClose();
           // RemoveMeOnList();
        }

        public  void Send(string data)
        {
            ServerManager.Sock.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", data)));
          Info.Write("realmS",data, ConsoleColor.Blue);
        }

        public void HelloClient()
        {
            Key = Hash.RandomString(32);
            Send(string.Format("{0}{1}", new HelloConnection(Key).ToString(), Key));     
        }

        ~Client()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
            if (ServerManager.Sock != null && ServerManager.Sock.Connected)
			{
				try
				{
                    ServerManager.Sock.Shutdown(SocketShutdown.Both);
                    ServerManager.Sock.Close();
                    ServerManager.Sock = null;
				}
				catch (SocketException)
				{				
				}
			}
		}
    }
}
