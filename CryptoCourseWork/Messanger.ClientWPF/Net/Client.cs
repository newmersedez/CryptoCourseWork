using System;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Messanger.Crypto.RC6.Classes;
using Messanger.Server.Net.IO;

namespace Messanger.ClientWPF.Net
{
    public sealed class Client
    {
        private readonly EncryptionMode _mode = EncryptionMode.ECB;
        private readonly byte[] _initVector = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private readonly string _param = "";
        private readonly byte[] _key;
        internal CipherContext Algorithm { get; set; }

        private readonly TcpClient _client;
        public PacketReader PacketReader;

        public event Action ConnectedEvent;
        public event Action MessageReceivedEvent;
        public event Action UserDisconnectedEvent;
        
        public Client()
        {
            _client = new TcpClient();
        }
        
        public void ConnectToServer(string username, string key)
        {
            if (_client.Connected)
                return;
            
            try
            {
                var byteKey = BigInteger.Parse(key).ToByteArray();
                if (byteKey.Length * 8 != 128)
                    throw new ArgumentException("Incorrect session key");

                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(Encoding.Default.GetBytes(username));
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }

                Algorithm = new CipherContext(_mode, _initVector, _param)
                {
                    Encrypter = new RC6(BigInteger.Parse(key).ToByteArray(), 128)
                };
                
                ReadPackets();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to connect to the server ({e})");
            }
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
                        case 15:
                            UserDisconnectedEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("ah yes...");
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
    }   
}