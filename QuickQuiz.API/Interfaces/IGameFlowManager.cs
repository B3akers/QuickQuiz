using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;

namespace QuickQuiz.API.Interfaces
{
    public interface IGameFlowManager
    {
        Task ProcessPacketAsync(WebSocketConnectionContext context, BasePacketRequest packet);
    }
}
