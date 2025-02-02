using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets.WebSocketPipes;
using QuickQuiz.API.WebSockets;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace QuickQuiz.API.Services.WebSocket
{
    public class WebSocketConnectionManager : IWebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocketConnectionContext> _connections;
        private readonly IConnectionTokenProvider _connectionTokenProvider;

        public WebSocketConnectionManager(IConnectionTokenProvider connectionTokenProvider)
        {
            _connections = new ConcurrentDictionary<string, WebSocketConnectionContext>();
            _connectionTokenProvider = connectionTokenProvider;
        }

        public WebSocketConnectionContext CreateContext(string connectionToken, HttpContext context, IWebSocketPipe pipe)
        {
            if (connectionToken == null)
            {
                return new WebSocketConnectionContext(_connectionTokenProvider.CreateConnectionId(), null, context, pipe);
            }

            var identity = _connectionTokenProvider.Authenticate(connectionToken);
            if (identity == null)
                return null;

            return new WebSocketConnectionContext(connectionToken, identity, context, pipe);
        }

        public async Task<bool> AddConnection(WebSocketConnectionContext context)
        {
            if (_connections.TryRemove(context.User.Id, out var activeContext))
            {
                await activeContext.Pipe.CompleteAsync((WebSocketCloseStatus)3402, "AnotherConnection");
            }

            //Add to lobby manager

            return _connections.TryAdd(context.User.Id, context);
        }

        public bool RemoveConnection(WebSocketConnectionContext context)
        {
            _connections.TryRemove(new KeyValuePair<string, WebSocketConnectionContext>(context.User.Id, context));

            //Todo remove from lobby manager

            return true;
        }

        public bool IsClientConnected(string userId)
        {
            return _connections.ContainsKey(userId);
        }
    }
}
