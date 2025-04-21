using QuickQuiz.API.Dto;
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
    }
}
