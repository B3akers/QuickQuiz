using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;

namespace QuickQuiz.API.Interfaces
{
    public interface IGameFlowManager
    {
        Task ProcessPacket(WebSocketConnectionContext context, BasePacketRequest packet);
    }
}
