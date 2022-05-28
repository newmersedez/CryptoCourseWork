using System.Net.Sockets;
using System.Text;

namespace Messanger.Server.Net.IO
{    public sealed class PacketReader : BinaryReader
     {
         private NetworkStream _ns;
         
         public PacketReader(NetworkStream ns) : base(ns)
         {
             _ns = ns;
         }
 
         public string ReadMessage()
         {
             byte[] messageBuffer;
             var length = ReadInt32();
             messageBuffer = new byte[length];
             _ns.Read(messageBuffer, 0, length);
 
             var message = Encoding.ASCII.GetString(messageBuffer);
 
             return message;
         }
     }

}
