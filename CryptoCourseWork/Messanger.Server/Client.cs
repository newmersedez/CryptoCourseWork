using System.Net;
using System.Net.Sockets;
using Messanger.Server.Net.IO;

namespace Messanger.Server
{
    public sealed class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        private PacketReader _packetReader;
        
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            
            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();
            
            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username {Username}\n");

            Task.Run(() => Process());
        }
        
        private void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                        {
                            var message = _packetReader.ReadMessage();
                            Console.Write($"[{DateTime.Now}] : Message received {message}\n");
                            Program.BroadcastMessage($"{Username}: {message}");
                            break;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Disconnected\n");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}