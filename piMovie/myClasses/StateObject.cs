using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace piMovie.myClasses
{
    class StateObject
    {
        public Socket workSocket;
        public const int BufferSize = 256;

        public byte[] buffer = new byte[BufferSize];

        // Don't need string buffer
        // We do receive strings, server is going to respond in strings
        public StringBuilder sb = new StringBuilder();
    }
}
