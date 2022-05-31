using System;
using System.IO;
using System.Text;

namespace Messanger.Server.Net.IO
{
    public sealed class PacketBuilder
    {
        private readonly MemoryStream _ms;

        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }

        // public void WriteMessage(string message)
        // {
        //     var messageLength = message.Length;
        //     _ms.Write(BitConverter.GetBytes(messageLength));
        //     _ms.Write(Encoding.Default.GetBytes(message));
        // }
        
        public void WriteMessage(byte[] message)
        {
            var messageLength = message.Length;
            _ms.Write(BitConverter.GetBytes(messageLength));
            _ms.Write(message);
        }
        
        // public void WriteByteMessage(byte[] byteMessage)
        // {
        //     var messageLength = byteMessage.Length;
        //     _ms.Write(BitConverter.GetBytes(messageLength));
        //     _ms.Write(byteMessage);
        // }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}