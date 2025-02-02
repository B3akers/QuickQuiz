using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Interfaces.WebSocket
{
    public interface IWebSocketMessageHandler
    {
        Task HandleMessagesAsync(string message, WebSocketConnectionContext context);
    }
}
