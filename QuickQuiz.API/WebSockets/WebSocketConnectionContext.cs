using QuickQuiz.API.Identities;
using QuickQuiz.API.WebSockets.WebSocketPipes;

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
    }
}
