using QuickQuiz.API.WebSockets.WebSocketPipes;
using QuickQuiz.API.WebSockets;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace QuickQuiz.API.Interfaces.WebSocket
{
    public interface IWebSocketConnectionManager
    {
        record ConnectionUpdateArgs(WebSocketConnectionContext Context, bool IsRemove);
        delegate void ConnectionUpdateHandler(object sender, ConnectionUpdateArgs e);
        event ConnectionUpdateHandler OnConnectionUpdate;

        public static readonly JsonSerializerOptions JsonJavascriptOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        WebSocketConnectionContext CreateContext(string connectionToken, HttpContext context, IWebSocketPipe pipe);
        WebSocketConnectionContext GetConnectionByUserId(string userId);
        Task<bool> AddConnectionAsync(WebSocketConnectionContext context);
        bool RemoveConnection(WebSocketConnectionContext context);
        bool IsClientConnected(string userId);
    }
}
