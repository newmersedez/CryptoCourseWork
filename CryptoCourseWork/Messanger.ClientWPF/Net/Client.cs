using System;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Messanger.Crypto.RC6.Classes;
using Messanger.Server.Net.IO;

namespace Messanger.ClientWPF.Net
{
    public sealed class Client
    {
        private const EncryptionMode Mode = EncryptionMode.ECB;
        private readonly byte[] _initVector = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private const string Param = "";
        internal CipherContext Algorithm;

        private readonly TcpClient _client;
        public PacketReader PacketReader;

        public event Action ConnectedEvent;
        public event Action MessageReceivedEvent;
        public event Action UserDisconnectedEvent;
        public event Action FileReceivedEvent;
        
        public Client()
        {
            _client = new TcpClient();
        }
        
        public void ConnectToServer(string username, string key)
        {
            if (_client.Connected)
                return;
            
            var byteKey = BigInteger.Parse(key).ToByteArray();
            if (byteKey.Length * 8 != 128)
                throw new ArgumentException("Incorrect session key");

            _client.ConnectAsync("127.0.0.1", 7891);
            PacketReader = new PacketReader(_client.GetStream());

            if (!string.IsNullOrEmpty(username))
            {
                var connectPacket = new PacketBuilder();
                connectPacket.WriteOpCode(0);
                connectPacket.WriteMessage(Encoding.Default.GetBytes(username));
                _client.Client.Send(connectPacket.GetPacketBytes());
            }

            Algorithm = new CipherContext(Mode, _initVector, Param)
            {
                Encrypter = new RC6(BigInteger.Parse(key).ToByteArray(), 128)
            };
            
            ReadPackets();
        }

        public bool IsConnectedToServer()
        {
            return _client.Connected;
        }
        
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke();
                            break;
                        case 5:
                            MessageReceivedEvent?.Invoke();
                            break;
                        case 10:
                            break;
                        case 15:
                            UserDisconnectedEvent?.Invoke();
                            break;
                        case 20:
                            FileReceivedEvent?.Invoke();
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(Algorithm.Encrypt(Encoding.Default.GetBytes(message)));
            _client.Client.Send(messagePacket.GetPacketBytes());
        }

        public void SendFileToServer(string fullPath)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(10);

            var index = fullPath.LastIndexOf('\\');
            var filename = fullPath.Substring(index + 1, fullPath.Length - index - 1);
            messagePacket.WriteMessage(Encoding.Default.GetBytes(filename));

            var text = File.ReadAllBytes(fullPath);
            messagePacket.WriteMessage(Algorithm.Encrypt(text));
            _client.Client.Send(messagePacket.GetPacketBytes());
        }

        public void GetFileFromServer(string filename)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(20);
            messagePacket.WriteMessage(Encoding.Default.GetBytes(filename));
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }   
}