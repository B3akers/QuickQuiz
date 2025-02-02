using QuickQuiz.API.WebSockets.WebSocketPipes;
using QuickQuiz.API.WebSockets;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace QuickQuiz.API.Interfaces.WebSocket
{
    public interface IWebSocketConnectionManager
    {
        public static readonly JsonSerializerOptions JsonJavascriptOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        WebSocketConnectionContext CreateContext(string connectionToken, HttpContext context, IWebSocketPipe pipe);
        Task<bool> AddConnection(WebSocketConnectionContext context);
        bool RemoveConnection(WebSocketConnectionContext context);
        bool IsClientConnected(string userId);
    }
}
