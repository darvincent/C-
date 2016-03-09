//和服务端通信的socket 类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace SupportLogSheet
{
    public class SLSocket
    {
        private MyInfo myInfo;
        // Socket Info
        private Socket Sock;
        private byte[] SocketBuffer;
        private string IP;
        private int Port;
        private int ReconnectTime;
        private StringBuilder WholeMessage;
        private readonly int BufferSize = 8096;
        //msg queues
        private List<string> outMsgQueue = new List<string>();
        private List<Msg_OP> inMsgQueue = new List<Msg_OP>();
        private readonly object inMsgLock = new object();
        private readonly object outMsgLock = new object();

        private Thread ReceiveMsgThread;
        private Thread SendMsgThread;
        private Thread SockAliveThread;

        private bool isSLSConnected;
        private bool isUIRunning;

        public delegate List<Msg_OP> Del_handleMsg(List<Msg_OP> msg_Ops);
        private Del_handleMsg register = new Del_handleMsg(handleMsg);

        public SLSocket()
        {
            if (Config.Customization_INI != null)
            {
                SocketBuffer = new byte[BufferSize];
                IP = Config.Common_INI.readValue("SLS", "ip");
                Port = Int32.Parse(Config.Common_INI.readValue("SLS", "port"));
                ReconnectTime = 3;
            }
            WholeMessage = new StringBuilder();
            isSLSConnected = true;
            isUIRunning = true;
        }

        public bool socketMsg(string msgTpe, message msg, Control toDespose)
        {
            try
            {
                StringBuilder sb = new StringBuilder(myInfo.MyID);
                sb.Append(Config.Msg_Separator1).Append(msgTpe).Append(Config.Msg_Separator1).
                    Append(msg.getMsg()).Append(Config.Msg_Separator6);
                outMsgQueue_add(sb.ToString());
                if (toDespose != null)
                {
                    toDespose.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return false;
            }
        }

        private static List<Msg_OP> handleMsg(List<Msg_OP> msg_Ops)
        {
            return null;
        }

        public void loginToSLS()
        {
            // send a login msg to SLS
            message msg = new message();
            msg.setKeyValuePair("80", myInfo.MyName);
            msg.setKeyValuePair("60", Config.SLS_Version);
            msg.setKeyValuePair("91", myInfo.MyLocation);
            socketMsg("L1", msg, null);
        }

        private void start()
        {
            handleMsg();
            sendMsg();
            checkSockStatus();
        }

        public bool establish(Del_handleMsg del, MyInfo info)
        {
            if (establishSocket(del, info))
            {
                start();
                loginToSLS();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool establishSocket(Del_handleMsg del,MyInfo info)
        {
            try
            {
                myInfo = info;
                register = del;
                if (Sock != null)
                {
                    if (Sock.Connected)
                    {
                        Sock.Shutdown(SocketShutdown.Both);
                        Sock.Close();
                    }
                }
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Sock.Connect(new IPEndPoint(IPAddress.Parse(IP), Port));
                Sock.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketFlags.None, new AsyncCallback(recieve), Sock);
                Config.logWriter.writeLog("establish socket to SLS successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return false;
            }
        }

        private string getSockMsg(string msg)
        {
            if (msg.Contains(Config.Msg_Separator6))
            {
                WholeMessage.Append(msg);
                string temp = Regex.Split(WholeMessage.ToString(), Config.Msg_Separator6)[0];
                WholeMessage.Clear();
                return temp;
            }
            else
            {
                WholeMessage.Append(msg);
            }
            return "";
        }

        private void recieve(IAsyncResult result)
        {
            try
            {
                Socket client = result.AsyncState as Socket;
                string msg = getSockMsg(Encoding.UTF8.GetString(SocketBuffer, 0, client.EndReceive(result)));
                if (!msg.Equals(""))
                {
                    inMsgQueue_add(new Msg_OP(msg));
                    Config.logWriter.writeLog("SLS> " + msg);
                }
                client.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketFlags.None, new AsyncCallback(recieve), client);
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void reconnect()
        {
            int tryTime = ReconnectTime;
            while (--tryTime >= 0)
            {
                Config.logWriter.writeLog("try to reconnect, remian try time:" + tryTime);
                if (establishSocket(register,myInfo))
                {
                    isSLSConnected = true;
                    inMsgQueue_add(new Msg_OP("L3", null));
                    Config.logWriter.writeLog("reconnect to SLS.");
                    break;
                }
                Thread.Sleep(5000);
            }

            if (!isSLSConnected)
            {
                Config.logWriter.writeLog("Disconnected to SLS and failed reconnect");
                inMsgQueue_add(new Msg_OP("L2", null));
            }
            else
            {
                Config.logWriter.writeLog("Restart procedures.");
                checkSockStatus();
                sendMsg();
                loginToSLS();
            }
        }

        public void inMsgQueue_add(Msg_OP msg_Op)
        {
            lock (inMsgLock)
            {
                inMsgQueue.Add(msg_Op);
            }
        }

        public void outMsgQueue_add(string msg)
        {
            if (msg.Equals(""))
            {
                return;
            }
            if (Sock.Connected)
            {
                lock (outMsgLock)
                {
                    outMsgQueue.Add(msg);
                }
            }
            else
            {
                inMsgQueue_add(new Msg_OP("L2", null));
            }
        }

        private void inMsgQueue_handle()
        {
            lock (inMsgLock)
            {
                List<Msg_OP> toRemove = register(inMsgQueue);
                if (toRemove != null)
                {
                    for (int i = 0; i < toRemove.Count; i++)
                    {
                        inMsgQueue.Remove(toRemove[i]);
                    }
                    toRemove.Clear();
                }
            }
        }

        private void outMsgQueue_send()
        {
            lock (outMsgLock)
            {
                if (outMsgQueue.Count > 0)
                {
                    try
                    {
                        send(outMsgQueue[0]);
                        Config.logWriter.writeLog(">SLS :" + outMsgQueue[0]);
                        outMsgQueue.RemoveAt(0);
                    }
                    catch (Exception ex)
                    {
                        Config.logWriter.writeErrorLog(ex);
                    }
                }
            }
        }

        private void send(string msg)
        {
            try
            {
                int length = msg.Length;
                int start = 0, end = 0;
                while (end < length)
                {
                    if (end + BufferSize < length)
                    {
                        end += BufferSize;
                    }
                    else
                    {
                        end = length - 1;
                    }
                    byte[] data = Encoding.UTF8.GetBytes(msg.Substring(start, end - start + 1));
                    Sock.BeginSend(data, 0, data.Length, 0, new AsyncCallback(sendCallback), data);
                    end++;
                    start = end;
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void sendCallback(IAsyncResult ar)
        {
            try
            {
                byte[] msg = (byte[])ar.AsyncState;
                int bytesSend = Sock.EndSend(ar);
                if (bytesSend != msg.Length)
                {
                    Config.logWriter.writeLog("Failed>SLS :" + Encoding.UTF8.GetString(msg));
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void handleMsg()
        {
            try
            {
                ReceiveMsgThread = new Thread(() =>
                {
                    while (isUIRunning)
                    {
                        inMsgQueue_handle();
                        Thread.Sleep(1);
                    }
                });
                ReceiveMsgThread.IsBackground = true;
                ReceiveMsgThread.Start();
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void sendMsg()
        {
            try
            {
                SendMsgThread = new Thread(() =>
                {
                    while (isSLSConnected)
                    {
                        outMsgQueue_send();
                        Thread.Sleep(1);
                    }
                });
                SendMsgThread.IsBackground = true;
                SendMsgThread.Start();
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void checkSockStatus()
        {
            try
            {
                SockAliveThread = new Thread(() =>
                {
                    while (isSLSConnected)
                    {
                        if (Sock.Connected)
                        {
                            Thread.Sleep(5000);
                        }
                        else
                        {
                            isSLSConnected = false;
                        }
                    }
                    reconnect();
                });
                SockAliveThread.IsBackground = true;
                SockAliveThread.Start();
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public void exit()
        {
            try
            {
                message msg = new message();
                msg.setKeyValuePair("80", myInfo.MyName);
                string cmd = new StringBuilder(myInfo.MyID).Append(Config.Msg_Separator1).Append("L0").Append(Config.Msg_Separator1).Append(msg.getMsg()).Append(Config.Msg_Separator6).ToString();
                Config.logWriter.writeLogSycn(cmd);
                Config.logWriter.Close();
                Sock.Send(Encoding.UTF8.GetBytes(cmd));
            }
            catch { }
            close();
        }

        public void close()
        {
            try
            {
                if (Sock.Connected)
                {
                    Sock.Shutdown(SocketShutdown.Both);
                    Sock.Close();
                }
                isSLSConnected = false;
                isUIRunning = false;
            }
            catch { }
        }
    }

}
