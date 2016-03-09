using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS
{
    public abstract class SocketMessage : IDisposable
    {
        private string message { get; set; }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        private bool IsDisposed = false;
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void dispose(bool Disposing)
        {
            if (!IsDisposed)
            {
                if (Disposing)
                {
                    //Clean Up managed resources  
                }
                //Clean up unmanaged resources  
            }
            IsDisposed = true;
        }
        ~SocketMessage()
        {
            dispose(false);
        }
    }
}
