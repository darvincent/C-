using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SLS
{
    public class OutputSocketMsg : SocketMessage
    {
        // 0 for broadCase, 1 for sending to specified clients
        private int type{get;set;}
        public int Type
        {
            get { return type; }
        }
        private List<SocketAsyncEventArgs> socketArgs { get; set; }
        public List<SocketAsyncEventArgs> SocketArgs
        {
            get { return socketArgs; }
        }
        public bool ForceDisconnect = false;
        public OutputSocketMsg(List<SocketAsyncEventArgs> socketArgs, int type, string msg)
        {
            this.socketArgs = socketArgs;
            this.type = type;
            this.Message = msg;
        }
    }
}
