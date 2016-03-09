using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SLS
{
    public class InputSocketMsg : SocketMessage
    {
        private SocketAsyncEventArgs sockArg { get; set; }
        public SocketAsyncEventArgs SockArg
        {
            get { return sockArg; }
           // set{sock =value;}
        }
        public InputSocketMsg(SocketAsyncEventArgs sockArg, string msg)
        {
            this.sockArg = sockArg;
            this.Message = msg;
        }
    }
}
