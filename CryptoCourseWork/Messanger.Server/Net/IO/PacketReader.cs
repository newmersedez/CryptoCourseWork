using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Messanger.Server.Net.IO
{
    public sealed class PacketReader : BinaryReader 
    {
        private readonly NetworkStream _ns; 
        
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }
        
        public string ReadMessage() 
        {
            byte[] messageBuffer;
            var length = ReadInt32();
            messageBuffer = new byte[length];
            _ns.Read(messageBuffer, 0, length);
            var message = Encoding.Default.GetString(messageBuffer);
            return message;
        }
     }
}
