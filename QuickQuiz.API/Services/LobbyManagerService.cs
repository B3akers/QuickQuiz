using QuickQuiz.API.Dto;
using QuickQuiz.API.Game;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Packets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static QuickQuiz.API.Interfaces.WebSocket.IWebSocketConnectionManager;

namespace QuickQuiz.API.Services
{
    public class LobbyManagerService : ILobbyManager
    {
        private readonly ConcurrentDictionary<string, Lobby> _lobbies;
        private readonly ConcurrentDictionary<string, string> _playerToLobby;
        private readonly ILogger<LobbyManagerService> _logger;
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;

        public LobbyManagerService(IWebSocketConnectionManager webSocketConnectionManager, ILogger<LobbyManagerService> logger)
        {
            _lobbies = new();
            _playerToLobby = new();
            _webSocketConnectionManager = webSocketConnectionManager;
            _webSocketConnectionManager.OnConnectionUpdate += OnConnectionUpdate;
            _logger = logger;
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

            lobby.UpdatePlayerConnection(userId, connection);
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

            lobby.AddPlayer(owner, _webSocketConnectionManager.GetConnectionByUserId(owner.Id));

            return lobby;
        }

        public async Task<bool> TryRemovePlayerFromLobby(ApplicationIdentityJWT player)
        {
            var lobby = GetLobbyByPlayer(player.Id);
            if (lobby == null) return false;

            var moveOwner = lobby.OwnerId == player.Id;
            var deleteLobby = moveOwner && lobby.Players.Count == 1;

            if (!_playerToLobby.TryRemove(new KeyValuePair<string, string>(player.Id, lobby.Id)))
                return false;

            var lobbyPlayer = lobby.RemovePlayer(player.Id);
            if (lobbyPlayer == null)
                return false;

            if (lobbyPlayer.Connection != null)
            {
                await lobbyPlayer.Connection.SendAsync(new LobbyPlayerRemoveResponsePacket() { PlayerId = player.Id, Reason = "kick" });
            }

            if (deleteLobby)
                return _lobbies.TryRemove(new KeyValuePair<string, Lobby>(lobby.Id, lobby));

            if (moveOwner)
            {
                lobby.OwnerId = lobby.Players.First().Key;
                await lobby.SendToAllPlayers(new LobbyTransferOwnerResponsePacket() { PlayerId = lobby.OwnerId });
            }

            await lobby.SendToAllPlayers(new LobbyPlayerRemoveResponsePacket() { PlayerId = player.Id });

            return true;
        }

        public async Task<bool> TryAddPlayerToLobby(ApplicationIdentityJWT player, string lobbyCode)
        {
            if (!_lobbies.TryGetValue(lobbyCode, out var lobby))
                return false;

            if (!lobby.Access.CanJoin(player))
                return false;

            if (lobby.Players.Count >= lobby.MaxPlayers)
                return false;

            if (!_playerToLobby.TryAdd(player.Id, lobbyCode))
                return false;

            var lobbyPlayer = lobby.AddPlayer(player, _webSocketConnectionManager.GetConnectionByUserId(player.Id));
            if (lobbyPlayer == null)
            {
                _playerToLobby.TryRemove(new KeyValuePair<string, string>(player.Id, lobbyCode));
                return false;
            }

            await lobby.SendToAllPlayers(new LobbyPlayerJoinResponsePacket() { Player = PlayerDto.Map(lobbyPlayer) }, [player.Id]);

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

        public Task OnUpdate()
        {
            return Task.CompletedTask;
        }
    }
}
