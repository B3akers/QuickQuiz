using QuickQuiz.API.Dto;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QuickQuiz.API.Game
{
    public class Lobby
    {
        public readonly string Id;
        public required string OwnerId;
        public readonly ConcurrentDictionary<string, LobbyPlayer> Players;
        public int MaxPlayers;
        public ILobbyAccess Access;

        public Lobby(string id)
        {
            Id = id;
            MaxPlayers = 100;
            Access = new AllLobbyAccess();
            Players = new();
        }

        public LobbyPlayer RemovePlayer(string playerId)
        {
            return Players.TryRemove(playerId, out var lobbyPlayer) ? lobbyPlayer : null;
        }

        public LobbyPlayer AddPlayer(ApplicationIdentityJWT applicationIdentity,
            WebSocketConnectionContext connection)
        {
            var result = new LobbyPlayer()
            {
                Connection = connection,
                Identity = applicationIdentity,
                LastConnectionUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            if (Players.TryAdd(applicationIdentity.Id, result)) return result;
            return Players.TryGetValue(applicationIdentity.Id, out result) ? result : null;
        }

        public void UpdatePlayerConnection(string userId, WebSocketConnectionContext connection)
        {
            if (!Players.TryGetValue(userId, out var player)) return;

            player.Connection = connection;
            player.LastConnectionUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public async Task SendToAllPlayers(BasePacketResponse packet, HashSet<string> ignore = null)
        {
            List<Task> tasks = new List<Task>(Players.Count);

            var message = JsonSerializer.Serialize(packet, IWebSocketConnectionManager.JsonJavascriptOptions);
            var bytes = Encoding.UTF8.GetBytes(message);
            var memory = new ReadOnlyMemory<byte>(bytes);

            foreach (var lobbyPlayer in Players)
            {
                var connection = lobbyPlayer.Value.Connection;
                if (connection == null) continue;
                if (ignore != null && ignore.Contains(lobbyPlayer.Key)) continue;

                tasks.Add(connection.SendAsync(memory));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch { }
        }

        public LobbyDto MapToDto()
        {
            var result = new LobbyDto();

            result.Id = Id;
            result.OwnerId = OwnerId;
            result.MaxPlayers = MaxPlayers;
            result.Players = new List<PlayerDto>(Players.Count);
            foreach (var player in Players)
                result.Players.Add(PlayerDto.Map(player.Value));

            return result;
        }
    }
}
