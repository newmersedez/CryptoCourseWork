using System.ComponentModel.Composition;
using GRPC.Server.Model;
using GrpcChatSample.Common;

namespace GRPC.Server.Persistence
{
    [Export(typeof(IChatLogRepository))]
    public class ChatLogRepository : IChatLogRepository
    {
        private readonly List<ChatLog> m_storage = new List<ChatLog>(); // dummy on memory storage

        public void Add(ChatLog chatLog)
        {
            m_storage.Add(chatLog);
        }

        public IEnumerable<ChatLog> GetAll()
        {
            return m_storage.AsReadOnly();
        }
    }
}
