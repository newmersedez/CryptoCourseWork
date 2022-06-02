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
            var length = ReadInt32();
            var message = new byte[length];
            _ns.ReadAsync(message, 0, length);
            return message;
        }
    }
}
