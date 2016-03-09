//socket Info 类， 封装了连接的socket，连接用户ID，用户名，以及tempData
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SLS
{
    public class SocketInfo
    {
        public string userID;
        public string userName;
        private Socket socket { get; set; }
        private byte[] tempData { get; set; }
        public Socket ClientSocket
        {
            get { return socket; }
            set { }
        }
        public byte[] TempData
        {
            get { return tempData; }
            set { tempData = value; }
        }
        public SocketInfo(Socket sock)
        {
            this.socket = sock;
            userID = "";
            userName = "";
        }

        public string ToString()
        {
            return new StringBuilder("SocketInfo userID: ").Append(userID).Append(" , userName: ").Append(userName).Append(" ,socket remote end point: ").Append(socket.RemoteEndPoint).ToString();
        }
    }
}
