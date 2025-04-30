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

        public event IWebSocketConnectionManager.ConnectionUpdateHandler OnConnectionUpdate;

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

        public async Task<bool> AddConnectionAsync(WebSocketConnectionContext context)
        {
            if (_connections.TryRemove(context.User.Id, out var activeContext))
            {
                await activeContext.Pipe.CompleteAsync((WebSocketCloseStatus)3402, "AnotherConnection");
            }

            if (!_connections.TryAdd(context.User.Id, context)) return false;

            OnConnectionUpdate(this, new IWebSocketConnectionManager.ConnectionUpdateArgs(context, false));

            return true;
        }

        public bool RemoveConnection(WebSocketConnectionContext context)
        {
            if (_connections.TryRemove(new KeyValuePair<string, WebSocketConnectionContext>(context.User.Id, context)))
            {
                OnConnectionUpdate(this, new IWebSocketConnectionManager.ConnectionUpdateArgs(context, true));
            }

            return true;
        }

        public bool IsClientConnected(string userId)
        {
            return _connections.ContainsKey(userId);
        }

        public WebSocketConnectionContext GetConnectionByUserId(string userId)
        {
            if (_connections.TryGetValue(userId, out var connection))
                return connection;

            return null;
        }
    }
}
