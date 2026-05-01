using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace _001_PortScaner
{
    internal class info
    {
        public Socket ClientSocket { get; set; }

        public string RemoteEndPoint { get; set; }

        public override string ToString()
        {
            return $"Client {RemoteEndPoint} connected";
        }
    }
}
