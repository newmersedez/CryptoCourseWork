using System.Diagnostics.CodeAnalysis;
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
            Task.Run(Process);
        }
        
        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: System.Byte[]; size: 1820MB")]
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
                            ServerProgram.BroadcastMessage(Encoding.Default.GetBytes(Username), message);
                            break;
                        }

                        case 10:
                        {
                            var filename = _packetReader.ReadMessage();
                            var text = _packetReader.ReadMessage();
                            
                            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
                            while (directory != null && !directory.GetFiles("*.sln").Any())
                            {
                                directory = directory.Parent;
                            }

                            var fullPath = directory + ServerProgram.ServerFiles + Encoding.Default.GetString(filename);
                            File.WriteAllBytes(fullPath, text);
                            break;
                        }
                        case 20:
                        {
                            var filename = _packetReader.ReadMessage();
                            ServerProgram.BroadcastFile(ClientSocket, filename);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    ServerProgram.BroadcastDisconnect(Uid.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}