using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using yEmu.Core.BufferPool;
using yEmu.Realm;

namespace yEmu.Network
{
    public abstract class Client : IDisposable
    {

        BufferPool<byte[]> myPool;

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

        public Processor Processors
        {
            get;
            set;
        }

 
        protected Client(Socket _sock, Server server)
        {
            myPool = new BufferPool<byte[]>(() => new byte[sizeBuffer]);
            myPool.Allocate(1);

            this.Sock = _sock;

            _server = server;
        }

        public void Receive()
        {
            if (Sock != null || Sock.Connected)
            {
     
                var args = new SocketAsyncEventArgs();


                    var buffer = myPool.Dequeue();
                    args.SetBuffer(buffer, args.Offset, buffer.Length);

                    Array.Clear(buffer, 0, 0);
                    myPool.Enqueue(buffer);

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

            var buffer = myPool.Dequeue();

            buffer = Encoding.UTF8.GetBytes(Messages + "\0");
            args.SetBuffer(buffer, 0, buffer.Length);
            Array.Clear(buffer, 0, 0);

            myPool.Enqueue(buffer);
     
            
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

