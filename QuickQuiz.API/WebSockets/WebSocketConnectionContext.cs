using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets.Data;
using QuickQuiz.API.WebSockets.WebSocketPipes;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace QuickQuiz.API.WebSockets
{
    public class WebSocketConnectionContext
    {
        public WebSocketConnectionContext(string connectionId, ApplicationIdentityJWT user, HttpContext context, IWebSocketPipe pipe)
        {
            ConnectionId = connectionId;
            StartTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            User = user;
            HttpContext = context;
            Pipe = pipe;
        }

        public long StartTimestamp { get; private set; }

        public string ConnectionId { get; private set; }

        public ApplicationIdentityJWT User { get; private set; }

        public HttpContext HttpContext { get; private set; }

        public IWebSocketPipe Pipe { get; private set; }

        public Task SendAsync(BasePacketResponse packet)
        {
            return SendAsync(JsonSerializer.Serialize(packet, IWebSocketConnectionManager.JsonJavascriptOptions));
        }

        public Task SendAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var memory = new ReadOnlyMemory<byte>(bytes);

            return SendAsync(memory);
        }

        public async Task SendAsync(ReadOnlyMemory<byte> data)
        {
            if (!IsValid()) return;
            await Pipe.Output.WriteAsync(data);
        }

        public bool IsValid()
        {
            var state = Pipe.State;
            return !(state == WebSocketState.Aborted ||
                     state == WebSocketState.Closed ||
                     state == WebSocketState.CloseSent);
        }
    }
}
