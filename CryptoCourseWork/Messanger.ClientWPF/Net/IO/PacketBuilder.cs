using System;
using System.IO;
using System.Text;
using Messanger.Crypto.RC6.Classes;

namespace Messanger.ClientWPF.Net.IO
{
    public sealed class PacketBuilder
    {
        private readonly MemoryStream _ms;
        private const int Length = 128;
        private readonly byte[] _key = new byte[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        private readonly byte[] _initializationVector = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        private readonly CipherContext _crypto;

        public PacketBuilder()
        {
            _ms = new MemoryStream(); 
            _crypto = new CipherContext(EncryptionMode.ECB, _initializationVector, "kekw");
            _crypto.Encrypter = new RC6(_key, Length);
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }

        public void WriteMessage(string message)
        {
            // var messageLength = message.Length;
            // _ms.Write(BitConverter.GetBytes(messageLength));
            // _ms.Write(_crypto.Encrypt(Encoding.ASCII.GetBytes(message)));
            
            var messageLength = message.Length;
            _ms.Write(BitConverter.GetBytes(messageLength));
            _ms.Write(Encoding.ASCII.GetBytes(message));
            
        }
        
        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}