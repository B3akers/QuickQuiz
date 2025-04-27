using QuickQuiz.API.Dto;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.Network.Game;
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
        public GameSettings LobbyGameSettings;

        public Lobby(string id)
        {
            Id = id;
            MaxPlayers = 100;
            Access = new AllLobbyAccess();
            LobbyGameSettings = new GameSettings();
            Players = new();
        }

        public void UpdateSettings(LobbySettingsDto settings)
        {
            settings.MaxPlayers = Math.Clamp(settings.MaxPlayers, Players.Count, 1000);

            MaxPlayers = settings.MaxPlayers;
            Access = settings.TwitchMode ? new TwitchLobbyAccess() : new AllLobbyAccess();
        }

        public void UpdateGameSettings(GameSettings settings)
        {
            settings.CategoryCountInVote = Math.Clamp(settings.CategoryCountInVote, 1, 50);
            settings.MaxCategoryVotesCount = Math.Clamp(settings.MaxCategoryVotesCount, 1, 15);
            settings.QuestionCountPerRound = Math.Clamp(settings.QuestionCountPerRound, 1, 20);
            settings.CategoryVoteTimeInSeconds = Math.Clamp(settings.CategoryVoteTimeInSeconds, 1, 90);
            settings.QuestionAnswerTimeInSeconds = Math.Clamp(settings.QuestionAnswerTimeInSeconds, 1, 120);

            if (settings.IncludeCategories != null)
            {
                settings.IncludeCategories = settings.IncludeCategories.Where(x => MongoDB.Bson.ObjectId.TryParse(x, out _)).ToList();
            }

            if (settings.ExcludeCategories != null)
            {
                settings.ExcludeCategories = settings.ExcludeCategories.Where(x => MongoDB.Bson.ObjectId.TryParse(x, out _)).ToList();
            }

            LobbyGameSettings = settings;
        }

        public LobbySettingsDto MapToSettingsDto()
        {
            var settings = new LobbySettingsDto();

            settings.MaxPlayers = MaxPlayers;
            settings.TwitchMode = Access is TwitchLobbyAccess;

            return settings;
        }

        public LobbyDto MapToDto()
        {
            var result = new LobbyDto();

            result.Id = Id;
            result.OwnerId = OwnerId;
            result.ActiveGameId = ActiveGameId;
            result.Players = new List<PlayerDto>(Players.Count);
            result.Settings = MapToSettingsDto();
            foreach (var player in Players)
                result.Players.Add(player.Value.MapToPlayerDto());

            return result;
        }
    }
}
