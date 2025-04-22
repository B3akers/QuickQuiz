using QuickQuiz.API.WebSockets.Packets;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.WebSockets.Data
{
    [JsonDerivedType(typeof(GameStateRequestPacket), typeDiscriminator: "gameState")]
    [JsonDerivedType(typeof(LobbyPlayerQuitRequestPacket), typeDiscriminator: "lobbyPlayerQuit")]
    [JsonDerivedType(typeof(LobbyPlayerKickRequestPacket), typeDiscriminator: "lobbyPlayerKick")]
    [JsonDerivedType(typeof(LobbyPlayerPromoteRequestPacket), typeDiscriminator: "lobbyPlayerPromote")]
    [JsonDerivedType(typeof(LobbyGameStartRequestPacket), typeDiscriminator: "lobbyGameStart")]
    public class BasePacketRequest
    {

    }

    [JsonDerivedType(typeof(GameStateResponsePacket), typeDiscriminator: "gameState")]
    [JsonDerivedType(typeof(LobbyPlayerJoinResponsePacket), typeDiscriminator: "lobbyPlayerJoin")]
    [JsonDerivedType(typeof(LobbyTransferOwnerResponsePacket), typeDiscriminator: "lobbyTransferOwner")]
    [JsonDerivedType(typeof(LobbyPlayerRemoveResponsePacket), typeDiscriminator: "lobbyPlayerRemove")]
    [JsonDerivedType(typeof(ShowToastResponsePacket), typeDiscriminator: "showToast")]
    public class BasePacketResponse
    {

    }

    [JsonDerivedType(typeof(ShowToastResponsePacket), typeDiscriminator: "showToast")]
    public class ShowToastResponsePacket : BasePacketResponse
    {
        public string Code { get; set; }
    }
}
