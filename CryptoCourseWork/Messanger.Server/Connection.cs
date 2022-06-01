using System.Net.Sockets;
using System.Numerics;
using System.Text;
using Messanger.Crypto.RC6.Classes;
using Messanger.Server;
using Messanger.Server.Net.IO;

namespace Messanger.Server
{
    public sealed class Connection
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        private readonly PacketReader _packetReader;
        
        public Connection(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            
            var opcode = _packetReader.ReadByte();
            Username = Encoding.Default.GetString(_packetReader.ReadMessage());

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
                            Console.Write($"[{DateTime.Now}] : Message received {Encoding.Default.GetString(message)}\n");
                            ServerProgram.BroadcastMessage(Username, message);
                            break;
                        }
}
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Disconnected\n");
                    ServerProgram.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}