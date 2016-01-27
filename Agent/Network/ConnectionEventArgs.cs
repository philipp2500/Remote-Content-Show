using System;
using System.Net;

namespace Agent.Network
{
    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionEventArgs(EndPoint remoteEndPoint)
        {
            this.RemoteEndPoint = remoteEndPoint;
        }

        public EndPoint RemoteEndPoint
        {
            get;
            private set;
        }
    }
}