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

        public  string _key;
        public Processor parse;

        public ServerManager _server;    
        public bool _connected
        {
            get;
            set;
        }
        public bool IsConnected
        {
            get { return _server._sock != null && _server._sock.Connected; }
        }
        public Client(ServerManager _sock )
        {
            _server = _sock;
            _server.DataReceive += this.DataArrivals;
            _server.OnSocketClose += this.OnSocketClose;
            this.HelloClient();
            parse = new Processor(this);
        }
        public void DataArrivals(byte[] data)
        {
          
                foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
                {
                    Info.Write("realmR", packet, ConsoleColor.White);
                    parse.Parser(packet);

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
          _server._sock.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", data)));
          Info.Write("realmS",data, ConsoleColor.Blue);

        }
        public void HelloClient()
        {
            _key = Hash.RandomString(32);
            Send(string.Format("{0}{1}", new HelloConnection(_key).ToString(), _key));
      

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
            if (_server._sock != null && _server._sock.Connected)
			{
				try
				{
                    _server._sock.Shutdown(SocketShutdown.Both);
                    _server._sock.Close();
                    _server._sock = null;
				}
				catch (SocketException)
				{
				
				}
			}
		}
    }
}
