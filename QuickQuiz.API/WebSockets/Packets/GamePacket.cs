using QuickQuiz.API.Database.Structures;
using QuickQuiz.API.Dto;
using QuickQuiz.API.Identities;
using QuickQuiz.API.WebSockets.Data;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.WebSockets.Packets
{
    [JsonDerivedType(typeof(GameCategoryVoteStartResponsePacket), typeDiscriminator: "gameCategoryVoteStart")]
    public class GameCategoryVoteStartResponsePacket : BasePacketResponse
    {
        public GameCategoryVoteDto CategoryVote { get; set; }
    }

    [JsonDerivedType(typeof(GameCategoryVoteRequestPacket), typeDiscriminator: "gameCategoryVote")]
    public class GameCategoryVoteRequestPacket : BasePacketRequest
    {
        public string CategoryId { get; set; }
    }

    [JsonDerivedType(typeof(GameCategoryVoteResponsePacket), typeDiscriminator: "gameCategoryVote")]
    public class GameCategoryVoteResponsePacket : BasePacketResponse
    {
        public string CategoryId { get; set; }
    }

    [JsonDerivedType(typeof(GamePlayersResponsePacket), typeDiscriminator: "gamePlayers")]
    public class GamePlayersResponsePacket : BasePacketResponse
    {
        public Dictionary<string, GamePlayerDto> Players { get; set; }
    }

    [JsonDerivedType(typeof(GamePrepareForQuestionResponsePacket), typeDiscriminator: "gamePrepareForQuestion")]
    public class GamePrepareForQuestionResponsePacket : BasePacketResponse
    {
        public GamePrepareForQuestionDto PrepareForQuestion { get; set; }
    }

    [JsonDerivedType(typeof(GameQuestionAnsweringResponsePacket), typeDiscriminator: "gameQuestionAnswering")]
    public class GameQuestionAnsweringResponsePacket : BasePacketResponse
    {
        public GameQuestionAnsweringDto QuestionAnswering { get; set; }
    }

    [JsonDerivedType(typeof(GameClearPlayerAnswersResponsePacket), typeDiscriminator: "gameClearPlayerAnswers")]
    public class GameClearPlayerAnswersResponsePacket : BasePacketResponse
    {
    }

    [JsonDerivedType(typeof(GameAsnwerResultResponsePacket), typeDiscriminator: "gameAnswerResult")]
    public class GameAsnwerResultResponsePacket : BasePacketResponse
    {
        public GameQuestionAnswerDto QuestionAnswer { get; set; }
    }

    [JsonDerivedType(typeof(GameAsnwerTimeoutResponsePacket), typeDiscriminator: "gameAnswerTimeout")]
    public class GameAsnwerTimeoutResponsePacket : BasePacketResponse
    {
        public List<string> PlayerIds { get; set; }
    }

    [JsonDerivedType(typeof(GamePlayerAnsweredResponsePacket), typeDiscriminator: "gamePlayerAnswered")]
    public class GamePlayerAnsweredResponsePacket : BasePacketResponse
    {
        public string PlayerId { get; set; }
        public int AnswerId { get; set; }
    }

    [JsonDerivedType(typeof(GameFinishedResponsePacket), typeDiscriminator: "gameFinished")]
    public class GameFinishedResponsePacket : BasePacketResponse
    {
        public List<KeyValuePair<string, double>> PlayerPoints { get; set; }
    }

    [JsonDerivedType(typeof(GameQuestionAnswerRequestPacket), typeDiscriminator: "gameQuestionAnswer")]
    public class GameQuestionAnswerRequestPacket : BasePacketRequest
    {
        public string QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}
