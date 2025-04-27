using QuickQuiz.API.Dto;
using QuickQuiz.API.Network.Game;
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

    [JsonDerivedType(typeof(LobbyUpdateSettingsRequestPacket), typeDiscriminator: "lobbyUpdateSettings")]
    public class LobbyUpdateSettingsRequestPacket : BasePacketRequest
    {
        public LobbySettingsDto Settings { get; set; }
    }

    [JsonDerivedType(typeof(LobbyUpdateSettingsResponsePacket), typeDiscriminator: "lobbyUpdateSettings")]
    public class LobbyUpdateSettingsResponsePacket : BasePacketResponse
    {
        public LobbySettingsDto Settings { get; set; }
    }

    [JsonDerivedType(typeof(LobbyGameUpdateSettingsRequestPacket), typeDiscriminator: "lobbyGameUpdateSettings")]
    public class LobbyGameUpdateSettingsRequestPacket : BasePacketRequest
    {
        public GameSettings Settings { get; set; }
    }

    [JsonDerivedType(typeof(LobbyGameUpdateSettingsResponsePacket), typeDiscriminator: "lobbyGameUpdateSettings")]
    public class LobbyGameUpdateSettingsResponsePacket : BasePacketResponse
    {
        public GameSettings Settings { get; set; }
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

    [JsonDerivedType(typeof(LobbyActiveGameUpdateResponsePacket), typeDiscriminator: "lobbyActiveGameUpdate")]
    public class LobbyActiveGameUpdateResponsePacket : BasePacketResponse
    {
        public string GameId { get; set; }
    }
}
