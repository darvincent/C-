//SocketAsyncEventArgs 分配池
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLS
{
    internal sealed class IoContextPool
    {
        List<SocketAsyncEventArgs> pool { get; set; }
        public List<SocketAsyncEventArgs> Pool
        {
            get { return pool; }
            //set { Pool = value; }
        }
        int Capacity;
        int Boundary;

        internal IoContextPool(int capacity)
        {
            this.pool = new List<SocketAsyncEventArgs>(capacity);
            this.Boundary = 0;
            this.Capacity = capacity;
        }

        internal bool Add(SocketAsyncEventArgs arg)
        {
            if (arg != null && pool.Count < Capacity)
            {
                pool.Add(arg);
                Boundary++;
                return true;
            }
            else
            {
                return false;
            }
        }

        //get a SocketAsyncEventArgs for using
        internal SocketAsyncEventArgs Pop()
        {
            lock (this.pool)
            {
                if (Boundary > 0)
                {
                    return pool[--Boundary];
                }
                else
                {
                    return null;
                }
            }
        }

        //add back a SocketAsyncEventArgs for re-using
        internal bool Push(SocketAsyncEventArgs arg)
        {
            if (arg != null)
            {
                lock (this.pool)
                {
                    int index = this.pool.IndexOf(arg, Boundary);
                    if (index == Boundary)
                    {
                        Boundary++;
                    }
                    else
                    {
                        this.pool[index] = this.pool[Boundary];
                        this.pool[Boundary++] = arg;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}