using GrpcChatSample.Common;

namespace GRPC.Server.Model
{
    public interface IChatLogRepository
    {
        void Add(ChatLog chatLog);
        IEnumerable<ChatLog> GetAll();
    }
}
