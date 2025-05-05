using QuickQuiz.API.Dto;
using QuickQuiz.API.Network;
using QuickQuiz.API.Network.Lobby;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Packets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static QuickQuiz.API.Interfaces.IGameManager;
using static QuickQuiz.API.Interfaces.WebSocket.IWebSocketConnectionManager;

namespace QuickQuiz.API.Services
{
    public class LobbyManagerService : ILobbyManager
    {
        private readonly ConcurrentDictionary<string, Lobby> _lobbies;
        private readonly ConcurrentDictionary<string, string> _playerToLobby;
        private readonly ILogger<LobbyManagerService> _logger;
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;

        private readonly IGameManager _gameManager;

        public LobbyManagerService(IWebSocketConnectionManager webSocketConnectionManager, ILogger<LobbyManagerService> logger, IGameManager gameManager)
        {
            _lobbies = new();
            _playerToLobby = new();
            _logger = logger;
            _webSocketConnectionManager = webSocketConnectionManager;
            _webSocketConnectionManager.OnConnectionUpdate += OnConnectionUpdate;
            _gameManager = gameManager;
            _gameManager.OnGameTerminate += OnGameTerminate;
        }

        private void OnGameTerminate(object sender, GameTerminateArgs e)
        {
            foreach (var lobby in _lobbies)
            {
                if (lobby.Value.ActiveGameId == e.GameId)
                {
                    _ = lobby.Value.Players.SendToAllPlayersAsync(new LobbyActiveGameUpdateResponsePacket() { GameId = null });
                    lobby.Value.ActiveGameId = null;
                    break;
                }
            }
        }

        public bool IsLobbyCodeAvailable(string lobbyCode)
        {
            return _lobbies.ContainsKey(lobbyCode) == false;
        }

        public int GetActiveLobbyCount()
        {
            return _lobbies.Count;
        }

        public int GetActivePlayersCount()
        {
            return _playerToLobby.Count;
        }

        public Lobby GetLobbyByPlayer(string playerId)
        {
            if (!_playerToLobby.TryGetValue(playerId, out var lobbyCode)) return null;
            if (!_lobbies.TryGetValue(lobbyCode, out var lobby)) return null;

            return lobby;
        }

        public void UpdatePlayerConnection(string userId, WebSocketConnectionContext connection)
        {
            var lobby = GetLobbyByPlayer(userId);
            if (lobby == null) return;

            lobby.Players.UpdatePlayerConnection(userId, connection);
        }

        public bool LobbyIsInGame(Lobby lobby)
        {
            if (string.IsNullOrEmpty(lobby.ActiveGameId)) return false;
            if (!_gameManager.IsGameActive(lobby.ActiveGameId))
            {
                lobby.ActiveGameId = null;

                _logger.LogWarning($"Lobby {lobby.Id} ActiveGameId is not null but game is not active!");

                return false;
            }

            return true;
        }

        public async Task<bool> LobbyStartGameAsync(Lobby lobby)
        {
            if (LobbyIsInGame(lobby)) return false;

            var game = await _gameManager.TryToCreateNewGameAsync(lobby.Players.Values.Select(x => x.Identity).ToList(), lobby.LobbyGameSettings);
            if (game == null) return false;

            lobby.ActiveGameId = game.Id;

            await lobby.Players.SendToAllPlayersAsync(new LobbyActiveGameUpdateResponsePacket() { GameId = game.Id });

            return true;
        }

        public Lobby CreateLobby(ApplicationIdentityJWT owner, string lobbyCode)
        {
            var lobby = new Lobby(lobbyCode)
            {
                OwnerId = owner.Id
            };

            if (!_lobbies.TryAdd(lobbyCode, lobby))
                return null;

            if (!_playerToLobby.TryAdd(owner.Id, lobbyCode))
            {
                _lobbies.TryRemove(lobbyCode, out _);
                return null;
            }

            lobby.Players.AddPlayer(new LobbyPlayer(), owner, _webSocketConnectionManager.GetConnectionByUserId(owner.Id));

            return lobby;
        }

        private async Task<bool> TryRemovePlayerFromLobbyInternalAsync(Lobby lobby, ApplicationIdentityJWT player, string reason)
        {
            if (!_playerToLobby.TryRemove(new KeyValuePair<string, string>(player.Id, lobby.Id)))
                return false;

            var lobbyPlayer = lobby.Players.RemovePlayer(player.Id);
            if (lobbyPlayer == null)
                return false;

            if (lobbyPlayer.Connection != null)
            {
                await lobbyPlayer.Connection.SendAsync(new LobbyPlayerRemoveResponsePacket() { PlayerId = player.Id, Reason = reason });
            }

            if (lobby.Players.Count == 0)
            {
                return _lobbies.TryRemove(new KeyValuePair<string, Lobby>(lobby.Id, lobby));
            }

            if (lobby.OwnerId == player.Id)
            {
                lobby.OwnerId = lobby.Players.First().Key;
                await lobby.Players.SendToAllPlayersAsync(new LobbyTransferOwnerResponsePacket() { PlayerId = lobby.OwnerId });
            }

            await lobby.Players.SendToAllPlayersAsync(new LobbyPlayerRemoveResponsePacket() { PlayerId = player.Id });

            return true;
        }

        public async Task<bool> TryRemovePlayerFromLobbyAsync(ApplicationIdentityJWT player)
        {
            var lobby = GetLobbyByPlayer(player.Id);
            if (lobby == null) return false;

            return await TryRemovePlayerFromLobbyInternalAsync(lobby, player, "remove");
        }

        public async Task<bool> TryAddPlayerToLobbyAsync(ApplicationIdentityJWT player, string lobbyCode)
        {
            if (!_lobbies.TryGetValue(lobbyCode, out var lobby))
                return false;

            if (!lobby.Access.CanJoin(player))
                return false;

            if (lobby.Players.Count >= lobby.MaxPlayers)
                return false;

            if (!_playerToLobby.TryAdd(player.Id, lobbyCode))
                return false;

            var lobbyPlayer = lobby.Players.AddPlayer(new LobbyPlayer(), player, _webSocketConnectionManager.GetConnectionByUserId(player.Id));
            if (lobbyPlayer == null)
            {
                _playerToLobby.TryRemove(new KeyValuePair<string, string>(player.Id, lobbyCode));
                return false;
            }

            await lobby.Players.SendToPlayersAsync(new LobbyPlayerJoinResponsePacket() { Player = lobbyPlayer.MapToPlayerDto() }, [player.Id]);

            return true;
        }

        private void OnConnectionUpdate(object sender, ConnectionUpdateArgs args)
        {
            UpdatePlayerConnection(args.Context.User.Id, args.IsRemove ? null : args.Context);
        }

        public bool PlayerIsInLobby(string playerId)
        {
            if (_playerToLobby.TryGetValue(playerId, out var lobbyCode))
            {
                if (_lobbies.ContainsKey(lobbyCode)) return true;

                _playerToLobby.TryRemove(playerId, out _);

                _logger.LogWarning("PlayerIsInLobby player is in list but lobby dosent exists anymore");

                return false;
            }

            return false;
        }

        private async Task OnLobbyUpdateAsync(Lobby lobby)
        {
            List<ApplicationIdentityJWT> disconnectedPlayers = new List<ApplicationIdentityJWT>();

            var currentTime = DateTimeOffset.UtcNow;
            foreach (var player in lobby.Players.Values)
            {
                if (currentTime - player.LastConnectionUpdate >= TimeSpan.FromMinutes(1) && player.Connection == null)
                {
                    disconnectedPlayers.Add(player.Identity);
                }
            }

            foreach (var player in disconnectedPlayers)
            {
                await TryRemovePlayerFromLobbyInternalAsync(lobby, player, "timeout");
            }
        }

        public async Task OnUpdateAsync()
        {
            List<Task> tasks = new List<Task>(_lobbies.Count);
            foreach (var lobby in _lobbies.Values)
            {
                tasks.Add(OnLobbyUpdateAsync(lobby));
            }

            await Task.WhenAll(tasks);
        }

        public Lobby GetLobbyById(string lobbyId)
        {
            if (_lobbies.TryGetValue(lobbyId, out var lobby))
                return lobby;

            _logger.LogWarning($"Lobby: {lobbyId} not found!");

            return null;
        }

        public List<LobbySimpleDto> GetAllLobbies()
        {
            return _lobbies.Values.Select(x => x.MapToSimpleDto()).ToList();
        }

        public Task<bool> TryKickPlayerFromLobbyAsync(Lobby lobby, ApplicationIdentityJWT player)
        {
            return TryRemovePlayerFromLobbyInternalAsync(lobby, player, "kick");
        }
    }
}
