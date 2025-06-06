﻿using QuickQuiz.API.WebSockets.Packets;
using System.Text.Json.Serialization;

namespace QuickQuiz.API.WebSockets.Data
{
    [JsonDerivedType(typeof(GameStateRequestPacket), typeDiscriminator: "gameState")]
    [JsonDerivedType(typeof(LobbyPlayerQuitRequestPacket), typeDiscriminator: "lobbyPlayerQuit")]
    [JsonDerivedType(typeof(LobbyPlayerKickRequestPacket), typeDiscriminator: "lobbyPlayerKick")]
    [JsonDerivedType(typeof(LobbyPlayerPromoteRequestPacket), typeDiscriminator: "lobbyPlayerPromote")]
    [JsonDerivedType(typeof(LobbyGameStartRequestPacket), typeDiscriminator: "lobbyGameStart")]
    [JsonDerivedType(typeof(GameCategoryVoteRequestPacket), typeDiscriminator: "gameCategoryVote")]
    [JsonDerivedType(typeof(GameQuestionAnswerRequestPacket), typeDiscriminator: "gameQuestionAnswer")]
    [JsonDerivedType(typeof(LobbyUpdateSettingsRequestPacket), typeDiscriminator: "lobbyUpdateSettings")]
    [JsonDerivedType(typeof(LobbyGameUpdateSettingsRequestPacket), typeDiscriminator: "lobbyGameUpdateSettings")]
    [JsonDerivedType(typeof(QuestionReportRequestPacket), typeDiscriminator: "reportQuestion")]
    public class BasePacketRequest
    {

    }

    [JsonDerivedType(typeof(GameStateResponsePacket), typeDiscriminator: "gameState")]
    [JsonDerivedType(typeof(LobbyPlayerJoinResponsePacket), typeDiscriminator: "lobbyPlayerJoin")]
    [JsonDerivedType(typeof(GameCategoryVoteResponsePacket), typeDiscriminator: "gameCategoryVote")]
    [JsonDerivedType(typeof(LobbyTransferOwnerResponsePacket), typeDiscriminator: "lobbyTransferOwner")]
    [JsonDerivedType(typeof(LobbyPlayerRemoveResponsePacket), typeDiscriminator: "lobbyPlayerRemove")]
    [JsonDerivedType(typeof(LobbyActiveGameUpdateResponsePacket), typeDiscriminator: "lobbyActiveGameUpdate")]
    [JsonDerivedType(typeof(ShowToastResponsePacket), typeDiscriminator: "showToast")]
    [JsonDerivedType(typeof(GameCategoryVoteStartResponsePacket), typeDiscriminator: "gameCategoryVoteStart")]
    [JsonDerivedType(typeof(GamePrepareForQuestionResponsePacket), typeDiscriminator: "gamePrepareForQuestion")]
    [JsonDerivedType(typeof(GamePlayersResponsePacket), typeDiscriminator: "gamePlayers")]
    [JsonDerivedType(typeof(GameClearPlayerAnswersResponsePacket), typeDiscriminator: "gameClearPlayerAnswers")]
    [JsonDerivedType(typeof(GameQuestionAnsweringResponsePacket), typeDiscriminator: "gameQuestionAnswering")]
    [JsonDerivedType(typeof(GameAsnwerResultResponsePacket), typeDiscriminator: "gameAnswerResult")]
    [JsonDerivedType(typeof(GameAsnwerTimeoutResponsePacket), typeDiscriminator: "gameAnswerTimeout")]
    [JsonDerivedType(typeof(GamePlayerAnsweredResponsePacket), typeDiscriminator: "gamePlayerAnswered")]
    [JsonDerivedType(typeof(GameFinishedResponsePacket), typeDiscriminator: "gameFinished")]
    [JsonDerivedType(typeof(LobbyUpdateSettingsResponsePacket), typeDiscriminator: "lobbyUpdateSettings")]
    [JsonDerivedType(typeof(LobbyGameUpdateSettingsResponsePacket), typeDiscriminator: "lobbyGameUpdateSettings")]
    public class BasePacketResponse
    {

    }

    [JsonDerivedType(typeof(ShowToastResponsePacket), typeDiscriminator: "showToast")]
    public class ShowToastResponsePacket : BasePacketResponse
    {
        public string Code { get; set; }
    }
}
