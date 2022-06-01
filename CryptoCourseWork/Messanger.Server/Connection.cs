using System.Net.Sockets;
using System.Text;
using Messanger.Server.Net.IO;

namespace Messanger.Server
{
    internal sealed class Connection
    {
        public string Username { get; }
        public Guid Uid { get; }
        
        public TcpClient ClientSocket { get;  }
        private readonly PacketReader _packetReader;
        
        public Connection(TcpClient client)
        {
            ClientSocket = client;
            Uid = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            
            _packetReader.ReadByte();
            Username = Encoding.Default.GetString(_packetReader.ReadMessage());

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username {Username}\n");

            Task.Run(Process);
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
                catch (Exception)
                {
                    Console.WriteLine($"[{Uid.ToString()}]: Disconnected\n");
                    ServerProgram.BroadcastDisconnect(Uid.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}