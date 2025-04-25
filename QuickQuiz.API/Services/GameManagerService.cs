using QuickQuiz.API.Database;
using QuickQuiz.API.Network;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.Network.Game;
using QuickQuiz.API.WebSockets;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static QuickQuiz.API.Interfaces.IGameManager;
using static QuickQuiz.API.Interfaces.WebSocket.IWebSocketConnectionManager;

namespace QuickQuiz.API.Services
{
    public class GameManagerService : IGameManager
    {
        private readonly ConcurrentDictionary<string, GameInstance> _games;
        private readonly ConcurrentDictionary<string, string> _playerToGame;
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;
        private readonly ILogger<GameManagerService> _logger;
        private readonly IQuizProvider _quizProvider;

        public event GameTerminateHandler OnGameTerminate;

        public GameManagerService(IWebSocketConnectionManager webSocketConnectionManager, ILogger<GameManagerService> logger, IQuizProvider quizProvider)
        {
            _games = new();
            _playerToGame = new();

            _quizProvider = quizProvider;
            _webSocketConnectionManager = webSocketConnectionManager;
            _webSocketConnectionManager.OnConnectionUpdate += OnConnectionUpdate;
            _logger = logger;
        }

        private void UpdatePlayerConnection(string userId, WebSocketConnectionContext connection)
        {
            var game = GetGameByPlayer(userId);
            if (game == null) return;

            game.Players.UpdatePlayerConnection(userId, connection);
        }

        private void OnConnectionUpdate(object sender, ConnectionUpdateArgs args)
        {
            UpdatePlayerConnection(args.Context.User.Id, args.IsRemove ? null : args.Context);
        }

        public int GetActiveGameCount()
        {
            return _games.Count;
        }

        public int GetActivePlayersCount()
        {
            return _playerToGame.Count;
        }

        public bool TryTerminateGame(string gameId)
        {
            if (!_games.TryRemove(gameId, out var game)) return false;

            foreach (var player in game.Players)
                _playerToGame.TryRemove(new KeyValuePair<string, string>(player.Key, game.Id));

            OnGameTerminate(this, new GameTerminateArgs(gameId));

            return true;
        }

        public async Task<GameInstance> TryToCreateNewGame(List<ApplicationIdentityJWT> players)
        {
            if (players.Count == 0)
                return null;

            var instance = new GameInstance(Guid.NewGuid().ToString(), _quizProvider);

            if (!_games.TryAdd(instance.Id, instance))
            {
                return null;
            }

            List<KeyValuePair<string, string>> playersToGame = new List<KeyValuePair<string, string>>(players.Count);
            foreach (var player in players)
            {
                var pair = new KeyValuePair<string, string>(player.Id, instance.Id);

                if (!_playerToGame.TryAdd(player.Id, instance.Id))
                {
                    instance = null;
                    break;
                }

                instance.Players.AddPlayer(new GamePlayer(), player, _webSocketConnectionManager.GetConnectionByUserId(player.Id));
                playersToGame.Add(pair);
            }

            if (instance == null)
            {
                _games.Remove(instance.Id, out _);

                foreach (var pair in playersToGame)
                    _playerToGame.TryRemove(pair);

                return null;
            }

            await instance.SwitchToCateogrySelection();

            return instance;
        }

        public bool TryGetGamePlayerPairByPlayer(string playerId, out GamePlayerPair pair)
        {
            pair = default;

            if (!_playerToGame.TryGetValue(playerId, out var gameId)) return false;
            if (!_games.TryGetValue(gameId, out var game)) return false;
            if (game.State == null) return false;
            if (!game.Players.TryGetValue(playerId, out var player)) return false;

            pair.Game = game;
            pair.Player = player;

            return true;
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

        public async Task OnUpdate()
        {
            List<Task<GameUpdateStatus>> gamesTick = new(_games.Count);
            foreach (var game in _games)
                gamesTick.Add(game.Value.Update());

            await foreach (var data in Task.WhenEach(gamesTick))
            {
                var result = await data;
                if (result.Ended)
                    TryTerminateGame(result.GameId);
            }
        }

        public bool IsGameActive(string gameId)
        {
            return _games.ContainsKey(gameId);
        }
    }
}
