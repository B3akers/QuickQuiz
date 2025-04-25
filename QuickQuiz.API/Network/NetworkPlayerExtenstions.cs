using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;

namespace QuickQuiz.API.Network
{
    public static class NetworkPlayerExtenstions
    {
        public static T RemovePlayer<T>(this ConcurrentDictionary<string, T> players, string playerId) where T : NetworkPlayer
        {
            return players.TryRemove(playerId, out var networkPlayer) ? networkPlayer : null;
        }

        public static T AddPlayer<T>(this ConcurrentDictionary<string, T> players,
            T player,
            ApplicationIdentityJWT applicationIdentity,
            WebSocketConnectionContext connection) where T : NetworkPlayer
        {
            player.Connection = connection;
            player.Identity = applicationIdentity;
            player.LastConnectionUpdate = DateTimeOffset.UtcNow;

            if (players.TryAdd(applicationIdentity.Id, player)) return player;
            return players.TryGetValue(applicationIdentity.Id, out player) ? player : null;
        }

        public static void UpdatePlayerConnection<T>(this ConcurrentDictionary<string, T> players, string userId, WebSocketConnectionContext connection) where T : NetworkPlayer
        {
            if (!players.TryGetValue(userId, out var player)) return;

            player.Connection = connection;
            player.LastConnectionUpdate = DateTimeOffset.UtcNow;
        }

        public static async Task SendToAllPlayers<T>(this ConcurrentDictionary<string, T> players, BasePacketResponse packet) where T : NetworkPlayer
        {
            List<Task> tasks = new List<Task>(players.Count);

            var message = JsonSerializer.Serialize(packet, IWebSocketConnectionManager.JsonJavascriptOptions);
            var bytes = Encoding.UTF8.GetBytes(message);
            var memory = new ReadOnlyMemory<byte>(bytes);

            foreach (var networkPlayer in players)
            {
                var connection = networkPlayer.Value.Connection;
                if (connection == null) continue;

                tasks.Add(connection.SendAsync(memory));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch { }
        }

        public static async Task SendToPlayers<T>(this ConcurrentDictionary<string, T> players, BasePacketResponse packet, IEnumerable<string> ignore) where T : NetworkPlayer
        {
            List<Task> tasks = new List<Task>(players.Count);

            var message = JsonSerializer.Serialize(packet, IWebSocketConnectionManager.JsonJavascriptOptions);
            var bytes = Encoding.UTF8.GetBytes(message);
            var memory = new ReadOnlyMemory<byte>(bytes);

            foreach (var networkPlayer in players)
            {
                var connection = networkPlayer.Value.Connection;
                if (connection == null) continue;
                if (ignore.Contains(networkPlayer.Key)) continue;

                tasks.Add(connection.SendAsync(memory));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch { }
        }
    }
}
