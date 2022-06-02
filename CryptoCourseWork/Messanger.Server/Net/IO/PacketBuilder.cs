namespace Messanger.Server.Net.IO
{
    public sealed class PacketBuilder
    {
        private readonly MemoryStream _ms;

        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }
        
        public void WriteMessage(byte[] message)
        {
            var messageLength = message.Length;
            _ms.WriteAsync(BitConverter.GetBytes(messageLength));
            _ms.WriteAsync(message);
        }
        
        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}