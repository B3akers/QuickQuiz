using QuickQuiz.API.Dto;
using QuickQuiz.API.WebSockets.Data;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.WebSockets.Packets
{
    [JsonDerivedType(typeof(LobbyPlayerQuitRequestPacket), typeDiscriminator: "lobbyPlayerQuit")]
    public class LobbyPlayerQuitRequestPacket : BasePacketRequest
    {
    }

    [JsonDerivedType(typeof(LobbyGameStartRequestPacket), typeDiscriminator: "lobbyGameStart")]
    public class LobbyGameStartRequestPacket : BasePacketRequest
    {
    }

    [JsonDerivedType(typeof(LobbyPlayerJoinResponsePacket), typeDiscriminator: "lobbyPlayerJoin")]
    public class LobbyPlayerJoinResponsePacket : BasePacketResponse
    {
        public PlayerDto Player { get; set; }
    }

    [JsonDerivedType(typeof(LobbyTransferOwnerResponsePacket), typeDiscriminator: "lobbyTransferOwner")]
    public class LobbyTransferOwnerResponsePacket : BasePacketResponse
    {
        public string PlayerId { get; set; }
    }

    [JsonDerivedType(typeof(LobbyPlayerKickRequestPacket), typeDiscriminator: "lobbyPlayerKick")]
    public class LobbyPlayerKickRequestPacket : BasePacketRequest
    {
        public string PlayerId { get; set; }
    }

    [JsonDerivedType(typeof(LobbyPlayerPromoteRequestPacket), typeDiscriminator: "lobbyPlayerPromote")]
    public class LobbyPlayerPromoteRequestPacket : BasePacketRequest
    {
        public string PlayerId { get; set; }
    }

    [JsonDerivedType(typeof(LobbyPlayerRemoveResponsePacket), typeDiscriminator: "lobbyPlayerRemove")]
    public class LobbyPlayerRemoveResponsePacket : BasePacketResponse
    {
        public string PlayerId { get; set; }
        public string Reason { get; set; }
    }
}
