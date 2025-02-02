using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Services.WebSocket
{
    public class WebSocketMessageHandler : IWebSocketMessageHandler
    {
        public Task HandleMessagesAsync(string message, WebSocketConnectionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
