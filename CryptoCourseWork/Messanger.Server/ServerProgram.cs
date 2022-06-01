using System.Net;
using System.Net.Sockets;
using System.Text;
using Messanger.Server.Net.IO;

namespace Messanger.Server
{
    internal static class ServerProgram
    {
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

        private static void BroadcastConnection()
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

        public static void BroadcastMessage(string username,  byte[] message)
        {
            Console.WriteLine($"Broadcasting {message}");
            if (_users == null) 
                return;
            foreach (var user in _users)
            {
                var messagePacket = new PacketBuilder();
                messagePacket.WriteOpCode(5);
                messagePacket.WriteMessage(Encoding.Default.GetBytes(username));
                messagePacket.WriteMessage(Encoding.Default.GetBytes(": "));
                messagePacket.WriteMessage(message);
                user.ClientSocket.Client.Send(messagePacket.GetPacketBytes());
            }
        }
        
        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users?.FirstOrDefault(x => x.Uid.ToString() == uid);
            if (disconnectedUser == null) 
                return;
            
            _users?.Remove(disconnectedUser);
            if (_users != null)
            {
                foreach (var user in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(15);
                    broadcastPacket.WriteMessage(Encoding.Default.GetBytes(uid));
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
            BroadcastMessage(disconnectedUser.Username, Encoding.Default.GetBytes($"Disconnected"));
        }
    }
}
