using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using Messanger.Crypto.RC6.Classes;

namespace Messanger.ClientWPF.Net.IO
{
    public sealed class PacketReader : BinaryReader
    {
        private readonly MemoryStream _ms;
        private const int Length = 128;
        private readonly byte[] _key = new byte[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        private readonly byte[] _initializationVector = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        private readonly CipherContext _crypto;
        private NetworkStream _ns;
        
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
            _crypto = new CipherContext(EncryptionMode.ECB, _initializationVector, "kekw");
            _crypto.Encrypter = new RC6(_key, Length);
        }

        public string ReadMessage()
        {
            byte[] messageBuffer;
            var length = ReadInt32();
            messageBuffer = new byte[length];
            _ns.Read(messageBuffer, 0, length);
            var message = Encoding.ASCII.GetString(messageBuffer);
            return message;
            
            // byte[] messageBuffer;
            // var length = ReadInt32();
            // messageBuffer = new byte[length];
            // _ns.Read(messageBuffer, 0, length);
            // var decryptedMessage = _crypto.Decrypt(messageBuffer);
            // return Encoding.ASCII.GetString(decryptedMessage);
        }
    }
}
