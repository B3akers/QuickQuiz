using QuickQuiz.API.Game;
using QuickQuiz.API.Interfaces;
using System.Collections.Concurrent;

namespace QuickQuiz.API.Services
{
    public class LobbyManagerService : ILobbyManager
    {
        private readonly ConcurrentDictionary<string, Lobby> _lobbies;
        private readonly ConcurrentDictionary<string, string> _playerToLobby;

        public LobbyManagerService()
        {
            _lobbies = new();
            _playerToLobby = new();
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

        public Lobby CreateLobby(string ownerId, string lobbyCode)
        {
            var lobby = new Lobby()
            {
                OwnerId = ownerId,
                Access = new AllLobbyAccess()
            };

            if (!_lobbies.TryAdd(lobbyCode, lobby))
                return null;

            if (!_playerToLobby.TryAdd(ownerId, lobbyCode))
            {
                _lobbies.TryRemove(lobbyCode, out _);
                return null;
            }

            return lobby;
        }
    }
}
