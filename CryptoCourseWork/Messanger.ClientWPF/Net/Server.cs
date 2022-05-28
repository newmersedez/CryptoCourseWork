using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messanger.ClientWPF.Net.IO;

namespace Messanger.ClientWPF.Net
{
    public sealed class Server
    {
        private readonly TcpClient _client;
        public PacketReader PacketReader;

        public event Action ConnectedEvent;
        public event Action MessageReceivedEvent;
        public event Action UserDisconnectedEvent;
        
        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());   
                }

                ReadPackets();
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
                        case 10:
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
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }   
}