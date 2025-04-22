using QuickQuiz.API.Dto;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QuickQuiz.API.Network.Lobby
{
    public class Lobby
    {
        public readonly string Id;
        public required string OwnerId;
        public readonly ConcurrentDictionary<string, LobbyPlayer> Players;
        public string ActiveGameId;
        public int MaxPlayers;
        public ILobbyAccess Access;

        public Lobby(string id)
        {
            Id = id;
            MaxPlayers = 100;
            Access = new AllLobbyAccess();
            Players = new();
        }

        public LobbyDto MapToDto()
        {
            var result = new LobbyDto();

            result.Id = Id;
            result.OwnerId = OwnerId;
            result.MaxPlayers = MaxPlayers;
            result.ActiveGameId = ActiveGameId;
            result.Players = new List<PlayerDto>(Players.Count);
            foreach (var player in Players)
                result.Players.Add(PlayerDto.Map(player.Value));

            return result;
        }
    }
}
