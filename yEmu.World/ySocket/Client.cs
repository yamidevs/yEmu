using Ngot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yEmu.World.Core;
using yEmu.World.Core.Classes.Characters;

namespace yEmu.Network
{
    public abstract class Client : IDisposable
    {


        public abstract bool DataArriavls(BufferSegment data);

        public  Server _server
        {
            get;
            set;
        }
        public int sizeBuffer = 1024;

        private object m_lock = new object();

        public Socket Sock 
        {
            get;
            private set;
        }
 
        public Characters Characters
        {
            get;
            set;
        }

        private uint _bytesReceived;

        private static readonly BufferManager Buffers = BufferManager.Default;

        private uint _bytesSent;

        private static long _totalBytesReceived;

        private static long _totalBytesSent;

        public static long TotalBytesSent
        {
            get { return _totalBytesSent; }
        }

        public static long TotalBytesReceived
        {
            get { return _totalBytesReceived; }
        }

        protected BufferSegment _bufferSegment;

        protected int _offset, _remainingLength;

        protected Client(Socket _sock, Server server)
        {
            Characters = new Characters();
            this.Sock = _sock;

            _server = server;
            _bufferSegment = Buffers.CheckOut();

        }

        public void Receive()
        {
                   
            if (Sock != null || Sock.Connected)
            {

                var args = SocketHelpers.AcquireSocketArg();
                var offset = _offset + _remainingLength;

                args.SetBuffer(_bufferSegment.Buffer.Array, _bufferSegment.Offset + offset, sizeBuffer - offset);
                                   
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
                var bytesReceived = args.BytesTransferred;

                if (args.BytesTransferred == 0)
                {
                    Console.WriteLine("Deconnexion utilisateur");
                    _server.Disconnect(this);
                }
                else
                {
                    unchecked
                    {
                        _bytesReceived += (uint)bytesReceived;
                    }

                    Interlocked.Add(ref _totalBytesReceived, bytesReceived);

                    _remainingLength += bytesReceived;

                       if (this.DataArriavls(_bufferSegment))
                        {
                            _offset = 0;
                            _bufferSegment.DecrementUsage();
                            _bufferSegment = Buffers.CheckOut();
                        }
                       else
                       {
                           EnsureBuffer();
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
                    SocketHelpers.ReleaseSocketArg(args);
            }
         }
        

        private void ReceiveAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            ProcessRecieve(args);
        }

        protected void EnsureBuffer() //(int size)
        {
            //if (size > BufferSize - _offset)
            {
                // not enough space left in buffer: Copy to new buffer
                var newSegment = Buffers.CheckOut();
                Array.Copy(_bufferSegment.Buffer.Array,
                    _bufferSegment.Offset + _offset,
                    newSegment.Buffer.Array,
                    newSegment.Offset,
                    _remainingLength);
                _bufferSegment.DecrementUsage();
                _bufferSegment = newSegment;
                _offset = 0;
            }
        }

        public virtual void send(string Messages)
        {
            
                if (Sock != null && Sock.Connected)
                {
                    Console.WriteLine("Send  : " + Messages);


                    var args = SocketHelpers.AcquireSocketArg();
                    if (args != null)
                    {
                        args.Completed += SendAsyncComplete;
                        args.UserToken = this;

                        var packet = Encoding.UTF8.GetBytes(string.Format("{0}\x00", Messages));
                        args.SetBuffer(packet, 0, packet.Length);
                        
                            Sock.SendAsync(args);
                            unchecked
                            {
                                _bytesSent += (uint)packet.Length;
                            }

                            Interlocked.Add(ref _totalBytesSent, packet.Length);
                    }
                    else
                    {
                    }
                }
           
        }

        private  void SendAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            args.Completed -= SendAsyncComplete;
            SocketHelpers.ReleaseSocketArg(args);
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

