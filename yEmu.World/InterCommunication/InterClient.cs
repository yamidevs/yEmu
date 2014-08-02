using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Util;

namespace yEmu.World.InterCommunication
{
    class InterClient
    {

        private string Ip 
        { 
            get;
            set; 
        }

        private int Port 
        { 
            get;
            set;
        }

        public int ServerId
        { 
            get;
            set; 
        }

        private Socket Sock;

        private static ManualResetEvent connectDone =  new ManualResetEvent(false);

        private static ManualResetEvent sendDone    =  new ManualResetEvent(false);

        private static ManualResetEvent receiveDone =  new ManualResetEvent(false);

        public void Initialize(string ip, int port)
        {
            Info.Write("", string.Format(" Loading InterCommunication  < {0} : {1} >", ip, port), ConsoleColor.White);
            Ip = ip;
            Port = port;
            Connect();
        }
      
        private void Connect()
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
            Thread.Sleep(1000);
            Sock.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectCall), Sock);
            connectDone.WaitOne();
            if (Sock.Connected)
            {
                Send(string.Format("{0}{1}", "HR", "loool"));
            }
            sendDone.WaitOne();

            Sock.BeginReceive(new byte[0], 0, 0, 0, CallBack, null);
            receiveDone.WaitOne();

        }
        private void ConnectCall(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Info.Write("", string.Format(" Socket connected to  :  < {0} >", client.RemoteEndPoint.ToString()), ConsoleColor.White);


                connectDone.Set();
            }
            catch 
            {
                Disconnected();
            }
        }
        private void CallBack(IAsyncResult ar)
        {
            try
            {
                Sock.EndReceive(ar);
                byte[] buff = new byte[8192];
                int rec = Sock.Receive(buff, buff.Length, 0);
                if (rec < buff.Length)
                {
                    Array.Resize<byte>(ref buff, rec);
                }
                if (rec == 0)
                {
                    Disconnected();
                    return;
                }
                Sock.BeginReceive(new byte[0], 0, 0, 0, CallBack, null);
            }
            catch (Exception)
            {
            }
        }

        public void Send(string Data)
        {
            Info.Write("gameS", Data, ConsoleColor.Gray);
            Sock.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", Data)));
        }

        public void Disconnected()
        {
            Sock.Close();
            Connect();
        }
    }
}
