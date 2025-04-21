using QuickQuiz.API.Game;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Interfaces.WebSocket;
using System.Collections.Concurrent;
using static QuickQuiz.API.Interfaces.WebSocket.IWebSocketConnectionManager;

namespace QuickQuiz.API.Services
{
    public class GameManagerService : IGameManager
    {
        private readonly ConcurrentDictionary<string, GameInstance> _games;
        private readonly ConcurrentDictionary<string, string> _playerToGame;
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;
        private readonly ILogger<GameManagerService> _logger;

        public GameManagerService(IWebSocketConnectionManager webSocketConnectionManager, ILogger<GameManagerService> logger)
        {
            _games = new();
            _playerToGame = new();

            _webSocketConnectionManager = webSocketConnectionManager;
            _webSocketConnectionManager.OnConnectionUpdate += OnConnectionUpdate;
            _logger = logger;
        }

        private void OnConnectionUpdate(object sender, ConnectionUpdateArgs args)
        {

        }

        public int GetActiveGameCount()
        {
            return _games.Count;
        }

        public int GetActivePlayersCount()
        {
            return _playerToGame.Count;
        }

        public GameInstance GetGameByPlayer(string playerId)
        {
            if (!_playerToGame.TryGetValue(playerId, out var gameId)) return null;
            if (!_games.TryGetValue(gameId, out var game)) return null;

            return game;
        }

        public bool PlayerIsInGame(string playerId)
        {
            if (_playerToGame.TryGetValue(playerId, out var gameId))
            {
                if (_games.ContainsKey(gameId)) return true;

                _playerToGame.TryRemove(playerId, out _);

                _logger.LogWarning("PlayerIsInGame player is in list but game dosent exists anymore");

                return false;
            }

            return false;
        }

        public Task OnUpdate()
        {
            return Task.CompletedTask;
        }
    }
}
