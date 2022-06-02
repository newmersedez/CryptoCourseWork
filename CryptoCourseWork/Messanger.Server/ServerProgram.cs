using System.Net;
using System.Net.Sockets;
using System.Text;
using Messanger.Server.Net.IO;

namespace Messanger.Server
{
    internal static class ServerProgram
    {
        internal const string ServerFiles = "\\Messanger.Server\\Files\\";
        private static TcpListener? _listener;
        private static List<Connection>? _users;
        
        private static void Main()
        {
            _users = new List<Connection>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();

            while (true)
            {
                var client = new Connection(_listener.AcceptTcpClient());
                _users.Add(client);
                BroadcastConnection();
            }
        }

        internal static void BroadcastConnection()
        {
            if (_users == null) 
                return;
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(Encoding.Default.GetBytes(usr.Username));
                    broadcastPacket.WriteMessage(Encoding.Default.GetBytes(usr.Uid.ToString()));
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(byte[] username, byte[] message)
        {
            if (_users == null)
            {
                return;
            }
            foreach (var user in _users)
            {
                var messagePacket = new PacketBuilder();
                messagePacket.WriteOpCode(5);
                messagePacket.WriteMessage(username);
                messagePacket.WriteMessage(Encoding.Default.GetBytes(": "));
                messagePacket.WriteMessage(message);
                user.ClientSocket.Client.Send(messagePacket.GetPacketBytes());
            }
        }

        internal static void BroadcastFile(TcpClient client, byte[] filename)
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            
            var fullPath = directory + ServerFiles + Encoding.Default.GetString(filename);
            var text = File.ReadAllBytes(fullPath);
            
            var filePacket = new PacketBuilder();
            filePacket.WriteOpCode(20);
            filePacket.WriteMessage(filename);
            filePacket.WriteMessage(text);
            client.Client.Send(filePacket.GetPacketBytes());
        }
        
        internal static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users?.FirstOrDefault(
                x => x.Uid.ToString() == uid);
            if (disconnectedUser == null)
            {
                return;
            }

            _users?.Remove(disconnectedUser);
            if (_users == null) 
                return;
            
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(15);
                broadcastPacket.WriteMessage(Encoding.Default.GetBytes(uid));
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
    }
}
