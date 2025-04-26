using QuickQuiz.API.Dto;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Network.Game.State;
using QuickQuiz.API.WebSockets.Data;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.WebSockets.Packets
{
    [JsonDerivedType(typeof(GameStateRequestPacket), typeDiscriminator: "gameState")]
    public class GameStateRequestPacket : BasePacketRequest
    {
    }

    [JsonDerivedType(typeof(GameStateResponsePacket), typeDiscriminator: "gameState")]
    public class GameStateResponsePacket : BasePacketResponse
    {
        public LobbyDto Lobby { get; set; }
        public Dictionary<string, GamePlayerDto> GamePlayers { get; set; }
        public GameCategoryVoteDto CategoryVote { get; set; }
        public GamePrepareForQuestionDto PrepareForQuestion { get; set; }
        public GameQuestionAnsweringDto QuestionAnswering { get; set; }
        public GameQuestionAnswerDto QuestionAnswer { get; set; }
        public string StateId { get; set; }
    }
}
