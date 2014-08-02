using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ySocket
{
    public abstract class Client : IDisposable
    {

        byte[] Buffer;

        byte[] BufferSent;

        public abstract bool DataArriavls(byte[] data);

        public  Server _server
        {
            get;
            set;
        }
        public int sizeBuffer = 8192;

        private object m_lock = new object();

        public Socket Sock 
        {
            get;
            private set;
        }

        protected Client(Socket _sock , Server server)
        {
            this.Sock = _sock;

            _server = server;
        }

        public void Receive()
        {
            if (Sock != null || Sock.Connected)
            {
     
                var args = new SocketAsyncEventArgs();

              
                    this.Buffer = new byte[sizeBuffer];
                    args.SetBuffer(this.Buffer,args.Offset, sizeBuffer);
                    Array.Clear(this.Buffer, 0, 0);
                                   
                    args.UserToken = this;
                    args.Completed += ReceiveAsyncComplete;

                    var willRaiseEvent = Sock.ReceiveAsync(args);
                    if (!willRaiseEvent)
                    {
                        ProcessRecieve(args);
                    }                    
                }
             }
            

        private void ProcessRecieve(SocketAsyncEventArgs args)
        {
            try
            {
                    var data = args.Buffer;
                    Array.Resize<byte>(ref data, args.BytesTransferred);

                    if (args.BytesTransferred <= 0)
                    {
                        Console.WriteLine("Deconnexion utilisateur");
                        _server.Disconnect(this);
                    }
                    else
                    {
                        if (this.DataArriavls(data))
                        {

                        }
                        
                        this.Receive();
                    }
                }                           
            catch (ObjectDisposedException)
            {

                _server.Disconnect(this);
            }
            catch (Exception)
            {

                _server.Disconnect(this);
            }
            finally
            {
                args.Completed -= ReceiveAsyncComplete;
            }
        }

        private void ReceiveAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            ProcessRecieve(args);
        }


        public virtual void send(string Messages)
        {
            if (Sock != null && Sock.Connected)
            {
            Console.WriteLine("Send  : " + Messages);


            var args = new SocketAsyncEventArgs();
            args.Completed += SendAsyncComplete;
            args.UserToken = Messages;

            this.BufferSent = Encoding.UTF8.GetBytes(string.Format("{0}\x00", Messages));
            args.SetBuffer(this.BufferSent, 0, this.BufferSent.Length);
            Array.Clear(this.BufferSent, 0, 0);
     
            
            Sock.SendAsync(args);
            }   
        }

        private  void SendAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
          args.Completed -= SendAsyncComplete;
          args.Dispose();
        }

        public void Connect(string host, int port)
        {
            Connect(IPAddress.Parse(host), port);
        }


        public void Connect(IPAddress addr, int port)
        {
            if (Sock != null)
            {
                if (Sock.Connected)
                {
                    Sock.Disconnect(true);
                }
                Sock.Connect(addr, port);

                Receive();
            }
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
            if (Sock != null && Sock.Connected)
            {
                try
                {
                    Sock.Shutdown(SocketShutdown.Both);
                    Sock.Close();
                    Sock = null;
                }
                catch
                {
                }
            }
        }
           
        }
    }

