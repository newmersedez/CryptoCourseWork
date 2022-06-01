using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using Messanger.Crypto.RC6.Classes;
using Messanger.Server.Net.IO;

namespace Messanger.Server
{
    public sealed class ServerProgram
    {
        private static TcpListener _listener;
        private static List<Connection> _users;

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        private static void Main(string[] args)
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
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(Encoding.Default.GetBytes(usr.Username));
                    broadcastPacket.WriteMessage(Encoding.Default.GetBytes(usr.UID.ToString()));
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string username,  byte[] message)
        {
            Console.WriteLine($"Broadcasting {message}");
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
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(15);
                broadcastPacket.WriteMessage(Encoding.Default.GetBytes(uid));
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            
            BroadcastMessage(disconnectedUser.Username, Encoding.Default.GetBytes($"Disconnected"));
        }
    }
}
