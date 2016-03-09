// socket msg handle server class, using IOCP
using System.Net;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SLS
{
    internal sealed class IoServer
    {
        private Thread sendThread;
        public static bool isSLSRunning = true;
        //用于将socket buffer里的数据拼接成一条完整的信息
        private Dictionary<Socket, StringBuilder> msgBuilders = new Dictionary<Socket, StringBuilder>();
        private List<OutputSocketMsg> socketMsgOutputList = new List<OutputSocketMsg>();
        public delegate void Del_handleInMsg(InputSocketMsg input_msg);
        private Del_handleInMsg inMsgHandler = new Del_handleInMsg(HandleInMsg);
        private Socket listenSocket;
        private static Mutex mutex = new Mutex();
        private int bufferSize;
        private int connectedSocketsNum;
        private int connectionCapacity;
        private IoContextPool ioContextPool;
        internal IoServer(int ConnectionCapacity, int BufferSize, Del_handleInMsg Del)
        {
            this.inMsgHandler = Del;
            this.connectedSocketsNum = 0;
            this.connectionCapacity = ConnectionCapacity;
            this.bufferSize = BufferSize;

            // initial SocketAsyncEventArgs pool
            this.ioContextPool = new IoContextPool(ConnectionCapacity);

            for (int i = 0; i < this.connectionCapacity; i++)
            {
                SocketAsyncEventArgs ioContext = new SocketAsyncEventArgs();
                ioContext.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                ioContext.SetBuffer(new Byte[this.bufferSize], 0, this.bufferSize);
                this.ioContextPool.Add(ioContext);
            }
        }

        private static void HandleInMsg(InputSocketMsg input_msg)
        {
            Server.logger.Info("MsgHandler handle-in-socket-message function failed!!!");
        }

        public bool IsSocketExist(Socket sock)
        {
            for (int i = 0; i < connectedSocketsNum; i++)
            {
                SocketInfo s = ioContextPool.Pool[i].UserToken as SocketInfo;
                if (s.ClientSocket== sock)
                {
                    return true;
                }
            }
                return false;
        }

        //receive or send done
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessReceive(e);
        }

        //receive done
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    SocketInfo s = (SocketInfo)e.UserToken;
                    string msg = GetInSockMsg(s.ClientSocket, Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred));
                    if (!msg.Equals(""))
                    {
                        inMsgHandler(new InputSocketMsg(e, msg));
                    }
                    if (!s.ClientSocket.ReceiveAsync(e))  
                    {
                        this.ProcessReceive(e);
                    }
                }
                else
                {
                    this.ProcessError(e);
                }
            }
            else
            {
                // remote client closed connection
                this.CloseClientSocket(e);
            }
        }

        private void ProcessError(SocketAsyncEventArgs e)
        {
            SocketInfo s = e.UserToken as SocketInfo;
            this.CloseClientSocket(s.ClientSocket, e);
        }

        //close connection
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            SocketInfo s = e.UserToken as SocketInfo;
            this.CloseClientSocket(s.ClientSocket, e);
        }

        private void CloseClientSocket(Socket s, SocketAsyncEventArgs e)
        {
            HandleAfterDisconnect(e);
            Interlocked.Decrement(ref this.connectedSocketsNum);
            // SocketAsyncEventArg released, push in for reuse
            try
            {
                s.Shutdown(SocketShutdown.Send);
                this.ioContextPool.Push(e);
                Server.logger.Info(new StringBuilder("Closed a connection: ").Append(s.RemoteEndPoint).Append(",").Append(s.Handle).Append("\r\n").ToString());
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
            }
            finally
            {
                s.Close();
            }
        }

        private void HandleAfterDisconnect(SocketAsyncEventArgs e)
        {
            SocketInfo s = e.UserToken as SocketInfo;
            Message msg = new Message();
            msg.SetKeyValuePair("80", s.userName);
            inMsgHandler(new InputSocketMsg(e, GenInSocketMsg(s.userID, "L0", msg.GetMsg())));
        }

        //accept done
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        //accept new connection
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Socket s = e.AcceptSocket;
            if (s.Connected)
            {
                try
                {
                    SocketAsyncEventArgs ioContext = this.ioContextPool.Pop();
                    if (ioContext != null)
                    {
                        ioContext.UserToken = new SocketInfo(s);
                        msgBuilders.Add(s, new StringBuilder());
                        byte[] buffer = new byte[1024];
                        Interlocked.Increment(ref this.connectedSocketsNum);
                        Server.logger.Info(new StringBuilder("Accept a connection: ").Append(s.RemoteEndPoint).Append(",").Append(s.Handle).Append("\r\n").ToString());
                        if (!s.ReceiveAsync(ioContext))
                        {
                            this.ProcessReceive(ioContext);
                        }
                    }
                    else  
                    {
                        s.Close();
                    }
                }
                catch (Exception ex)
                {
                    Server.logger.Error(ex);
                }
                // next request
                this.StartAccept(e);
            }
        }

        /// <param name="acceptEventArg">The context object to use when issuing 
        /// the accept operation on the server's listening socket.</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                // reuse
                acceptEventArg.AcceptSocket = null;
            }

            if (!this.listenSocket.AcceptAsync(acceptEventArg))
            {
                this.ProcessAccept(acceptEventArg);
            }
        }

        //start binding
        internal void Start(int port)
        {
            // get host information
            IPAddress[] addressList = Dns.GetHostEntry(Environment.MachineName).AddressList;
            //IPEndPoint localEndPoint = new IPEndPoint(addressList[addressList.Length - 1], port);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any,port);
            // create binding socket
            this.listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.ReceiveBufferSize = this.bufferSize;
            this.listenSocket.SendBufferSize = this.bufferSize;

            if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // binding dual-mode (IPv4 & IPv6) 
                // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
                this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                this.listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
            }
            else
            {
                this.listenSocket.Bind(localEndPoint);
            }

            // start binding
            this.listenSocket.Listen(this.connectionCapacity);

            // place a null accept request
            this.StartAccept(null);

            Send();
            // Blocks the current thread to receive incoming messages.
            mutex.WaitOne();
        }

        internal void Stop()
        {
            this.listenSocket.Close();
            mutex.ReleaseMutex();
        }

        private string GetInSockMsg(Socket sock, string msg)
        {
            if (msg.Contains(Config.Msg_Separator6))
            {
                msgBuilders[sock].Append(msg);
                string temp = Regex.Split(msgBuilders[sock].ToString(), Config.Msg_Separator6)[0];
                msgBuilders[sock].Clear();
                return temp;
            }
            else
            {
                if (!msg.Trim().Equals(""))
                {
                    msgBuilders[sock].Append(msg);
                }
                return "";
            }
        }

        public static string GenOutSocketMsg(string userID, string msgType, string msgContent)
        {
            try
            {
                return new StringBuilder(userID).Append(Config.Msg_Separator1).Append(msgType).Append(Config.Msg_Separator1).Append(msgContent).Append(Config.Msg_Separator6).ToString();
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
                return new StringBuilder(userID).Append(Config.Msg_Separator1).Append(msgType).Append(Config.Msg_Separator1).Append(Config.Msg_Separator6).ToString();
            }
        }

        public static string GenOutSocketMsg(string toAddMsgSeperator6)
        {
            int i = toAddMsgSeperator6.Length;
            try
            {
                return toAddMsgSeperator6 + Config.Msg_Separator6;
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
                return "";
            }
        }

        public static string GenInSocketMsg(string userID, string msgType, string msgContent)
        {
            try
            {
                return new StringBuilder(userID).Append(Config.Msg_Separator1).Append(msgType).Append(Config.Msg_Separator1).Append(msgContent).ToString();
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
                return new StringBuilder(userID).Append(Config.Msg_Separator1).Append(msgType).Append(Config.Msg_Separator1).ToString();
            }
        }

        public void AddOutSockMsg(OutputSocketMsg OSM)
        {
            socketMsgOutputList.Add(OSM);
        }

        private void DoSend(string msg,SocketInfo s)
        {
            int length = msg.Length;
            int start = 0, end = 0;
            while (end < length)
            {
                if (end + bufferSize < length)
                {
                    end += bufferSize;
                }
                else
                {
                    end = length - 1;
                }
                s.TempData  = Encoding.UTF8.GetBytes(msg.Substring(start, end - start + 1));
                s.ClientSocket.BeginSend(s.TempData, 0, s.TempData.Length, 0, new AsyncCallback(SendCallback), s);
                end++;
                start = end;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                SocketInfo s = ar.AsyncState as SocketInfo;
                int bytesSend = s.ClientSocket.EndSend(ar);
                if (bytesSend != s.TempData.Length)
                {
                    Server.logger.Info("Failed>UI :" + Encoding.UTF8.GetString(s.TempData));
                }
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
            }
        }

        private void Send()
        {
            sendThread = new Thread(() =>
            {
                while (isSLSRunning)
                {
                    try
                    {
                        if (socketMsgOutputList.Count > 0)
                        {
                            OutputSocketMsg tempMsg = socketMsgOutputList[0];
                            switch (tempMsg.Type)
                            {
                                // broadCast
                                case 0:
                                    {
                                        SocketInfo s;
                                        Server.logger.Info("Broadcast<" + tempMsg.Message + ", total " + connectedSocketsNum+" connections.");
                                        for (int i = 1; i <= connectedSocketsNum;i++ )
                                        {
                                            try
                                            {
                                                s = (SocketInfo)ioContextPool.Pool[connectionCapacity - i].UserToken;
                                                if (s.ClientSocket.Connected)
                                                {
                                                    DoSend(tempMsg.Message, s);
                                                    Server.logger.Info("Succeed broadcast to " + s.ClientSocket.RemoteEndPoint + " " + s.ClientSocket.Handle);
                                                }
                                            }
                                            catch(Exception ex)
                                            {
                                                Server.logger.Error(ex);
                                            }
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        // to specified clients
                                        SocketInfo s;
                                        for (int i = 0; i < tempMsg.SocketArgs.Count; i++)
                                        {
                                            try
                                            {
                                                s = (SocketInfo)tempMsg.SocketArgs[i].UserToken;
                                                if (s.ClientSocket.Connected)
                                                {
                                                    DoSend(tempMsg.Message, s);
                                                    Server.logger.Info(s.ClientSocket.RemoteEndPoint + "<" + tempMsg.Message);
                                                    if (tempMsg.ForceDisconnect)
                                                    {
                                                        Server.logger.Info(new StringBuilder("Removed a connected client due to duplicate login: ").Append(s.ClientSocket.RemoteEndPoint).Append(" ").Append(s.ClientSocket.Handle).ToString());
                                                        CloseClientSocket(tempMsg.SocketArgs[i]);
                                                    }
                                                }
                                            }
                                            catch(Exception ex)
                                            {
                                                Server.logger.Error(ex);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            socketMsgOutputList.Remove(tempMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Server.logger.Error(ex);
                    }
                    Thread.Sleep(3);
                }
            });
            sendThread.IsBackground = true;
            sendThread.Start();
        }
    }
}