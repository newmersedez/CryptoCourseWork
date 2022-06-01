using System.Net.Sockets;

namespace Messanger.Server.Net.IO
{
    public sealed class PacketReader : BinaryReader 
    {
        private readonly NetworkStream _ns; 
        
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }
        
        public byte[] ReadMessage() 
        {
            byte[] message;
            var length = ReadInt32();
            message = new byte[length];
            _ns.Read(message, 0, length);
            return message;
        }
    }
}
