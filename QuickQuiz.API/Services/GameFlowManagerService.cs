using QuickQuiz.API.Dto;
using QuickQuiz.API.Network;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;
using QuickQuiz.API.WebSockets.Packets;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using QuickQuiz.API.Network.Game;
using QuickQuiz.API.Interfaces.WebSocket;
using System.Text.Json;
using System.Text;
using QuickQuiz.API.Database;
using MongoDB.Driver;

namespace QuickQuiz.API.Services
{
    public class GameFlowManagerService : IGameFlowManager
    {
        private readonly ILobbyManager _lobbyManager;
        private readonly IGameManager _gameManager;
        private readonly MongoContext _mongoContext;
        private readonly ILogger<GameFlowManagerService> _logger;
        private readonly GameGlobalAsyncLock _globalGameLock;

        public GameFlowManagerService(ILobbyManager lobbyManager, IGameManager gameManager, ILogger<GameFlowManagerService> logger, GameGlobalAsyncLock globalGameLock, MongoContext mongoContext)
        {
            _lobbyManager = lobbyManager;
            _gameManager = gameManager;
            _logger = logger;
            _globalGameLock = globalGameLock;
            _mongoContext = mongoContext;
        }

        //TODO: move to handlers
        //
        public async Task ProcessPacketAsync(WebSocketConnectionContext context, BasePacketRequest packet)
        {
            try
            {
                await using (var readLock = await _globalGameLock.ReadLockAsync())
                {
                    if (packet is GameStateRequestPacket)
                    {
                        var response = new GameStateResponsePacket()
                        {
                            StateId = "None",
                        };

                        var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                        if (lobby != null)
                        {
                            response.Lobby = lobby.MapToDto();
                            response.LobbyGameSettings = lobby.LobbyGameSettings;
                        }

                        if (_gameManager.TryGetGamePlayerPairByPlayer(context.User.Id, out var pair))
                        {
                            if (pair.Game.State.Id == Network.Game.State.GameStateId.CategorySelection)
                            {
                                response.CategoryVote = new GameCategoryVoteDto()
                                {
                                    Categories = pair.Game.CurrentCategories,
                                    CategoryIndex = pair.Game.CurrentCategoryRoundIndex,
                                    MaxCategoryIndex = pair.Game.Settings.MaxCategoryVotesCount,
                                    StartTime = pair.Game.LastStateSwitch,
                                    EndTime = pair.Game.LastStateSwitch.AddSeconds(pair.Game.Settings.CategoryVoteTimeInSeconds),
                                    SelectedCategory = pair.Player.CategoryVoteId
                                };
                            }

                            if (pair.Game.State.Id == Network.Game.State.GameStateId.PrepareForQuestion)
                            {
                                response.PrepareForQuestion = new GamePrepareForQuestionDto()
                                {
                                    Category = pair.Game.CurrentQuestionCategory,
                                    PreloadImage = pair.Game.CurrentQuestions[pair.Game.CurrentQuestionIndex].Image,
                                    QuestionCount = pair.Game.CurrentQuestions.Count,
                                    QuestionIndex = pair.Game.CurrentQuestionIndex
                                };
                            }

                            if (pair.Game.State.Id == Network.Game.State.GameStateId.RoundEnd)
                            {
                                response.PrepareForQuestion = new GamePrepareForQuestionDto()
                                {
                                    Category = pair.Game.CurrentQuestionCategory,
                                    QuestionCount = pair.Game.CurrentQuestions.Count,
                                    QuestionIndex = pair.Game.CurrentQuestionIndex
                                };
                            }

                            if (pair.Game.State.Id == Network.Game.State.GameStateId.QuestionAnswering
                            || pair.Game.State.Id == Network.Game.State.GameStateId.QuestionAnswered)
                            {
                                var question = pair.Game.CurrentQuestions[pair.Game.CurrentQuestionIndex];

                                response.QuestionAnswering = new GameQuestionAnsweringDto()
                                {
                                    StartTime = pair.Game.LastStateSwitch,
                                    EndTime = pair.Game.LastStateSwitch.AddSeconds(pair.Game.Settings.QuestionAnswerTimeInSeconds),
                                    Question = GameQuestionDto.Map(question),
                                };

                                if (pair.Player.AnswerId != -1
                                    || pair.Game.State.Id == Network.Game.State.GameStateId.QuestionAnswered)
                                {
                                    response.QuestionAnswer = new GameQuestionAnswerDto()
                                    {
                                        PlayerAnswers = pair.Game.Players.GetPlayerAnswers(question.Answers.Count),
                                        CorrectAnswerId = question.CorrectAnswer
                                    };
                                }
                            }

                            response.GamePlayers = pair.Game.Players.ToPlayersDto();
                            response.StateId = pair.Game.State.Id.ToString();
                        }

                        await context.SendAsync(response);
                    }
                    else if (packet is LobbyPlayerQuitRequestPacket)
                    {
                        await _lobbyManager.TryRemovePlayerFromLobbyAsync(context.User);
                    }
                    else if (packet is LobbyPlayerKickRequestPacket lobbyKickRequest)
                    {
                        var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                        if (lobby == null)
                            return;

                        if (lobby.OwnerId != context.User.Id)
                            return;

                        if (lobby.Players.TryGetValue(lobbyKickRequest.PlayerId, out var player))
                            await _lobbyManager.TryKickPlayerFromLobbyAsync(lobby, player.Identity);
                    }
                    else if (packet is LobbyPlayerPromoteRequestPacket lobbyPromoteRequest)
                    {
                        var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                        if (lobby == null)
                            return;

                        if (lobby.OwnerId != context.User.Id)
                            return;

                        if (lobby.OwnerId == lobbyPromoteRequest.PlayerId)
                            return;

                        if (lobby.Players.TryGetValue(lobbyPromoteRequest.PlayerId, out var player))
                        {
                            lobby.OwnerId = player.Identity.Id;
                            await lobby.Players.SendToAllPlayersAsync(new LobbyTransferOwnerResponsePacket() { PlayerId = lobby.OwnerId });
                        }
                    }
                    else if (packet is LobbyUpdateSettingsRequestPacket updateLobbySettingsPacket)
                    {
                        var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                        if (lobby == null)
                            return;

                        if (lobby.OwnerId != context.User.Id)
                            return;

                        lobby.UpdateSettings(updateLobbySettingsPacket.Settings);

                        await lobby.Players.SendToAllPlayersAsync(new LobbyUpdateSettingsResponsePacket() { Settings = updateLobbySettingsPacket.Settings });
                    }
                    else if (packet is LobbyGameUpdateSettingsRequestPacket updateLobbyGameSettingsPacket)
                    {
                        var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                        if (lobby == null)
                            return;

                        if (lobby.OwnerId != context.User.Id)
                            return;

                        lobby.UpdateGameSettings(updateLobbyGameSettingsPacket.Settings);

                        await lobby.Players.SendToAllPlayersAsync(new LobbyGameUpdateSettingsResponsePacket() { Settings = updateLobbyGameSettingsPacket.Settings });
                    }
                    else if (packet is LobbyGameStartRequestPacket)
                    {
                        var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                        if (lobby == null)
                            return;

                        if (lobby.OwnerId != context.User.Id)
                            return;

                        if (!string.IsNullOrEmpty(lobby.ActiveGameId))
                        {
                            await context.SendAsync(new ShowToastResponsePacket() { Code = "lobby_game_active" });
                            return;
                        }

                        if (!await _lobbyManager.LobbyStartGameAsync(lobby))
                        {
                            await context.SendAsync(new ShowToastResponsePacket() { Code = "lobby_failed_to_start_game" });
                            return;
                        }
                    }
                    else if (packet is QuestionReportRequestPacket reportQuestionPacket)
                    {
                        if (!_gameManager.TryGetGamePlayerPairByPlayer(context.User.Id, out var pair))
                            return;

                        if (pair.Game.State.Id != Network.Game.State.GameStateId.QuestionAnswering
                            && pair.Game.State.Id != Network.Game.State.GameStateId.QuestionAnswered)
                            return;

                        if (reportQuestionPacket.QuestionId != pair.Game.CurrentQuestions[pair.Game.CurrentQuestionIndex].Id)
                            return;

                        await _mongoContext.QuestionReports.UpdateOneAsync(
                            x => x.QuestionId == reportQuestionPacket.QuestionId,
                            Builders<Database.Structures.QuestionReport>.Update
                            .Set(x => x.Reason, Math.Clamp(reportQuestionPacket.Reason, 0, 4))
                            .Set(x => x.SenderIp, context?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty), new UpdateOptions()
                            {
                                IsUpsert = true,
                            });
                    }
                    else if (packet is GameCategoryVoteRequestPacket categoryVotePacket)
                    {
                        if (!_gameManager.TryGetGamePlayerPairByPlayer(context.User.Id, out var pair))
                            return;

                        if (pair.Game.State.Id != Network.Game.State.GameStateId.CategorySelection)
                            return;

                        ref var voteCount = ref CollectionsMarshal.GetValueRefOrNullRef(pair.Game.CurrentVoteCategories, categoryVotePacket.CategoryId);
                        if (Unsafe.IsNullRef(ref voteCount))
                            return;

                        if (!string.IsNullOrEmpty(pair.Player.CategoryVoteId))
                        {
                            ref var prevCatVoteCount = ref CollectionsMarshal.GetValueRefOrNullRef(pair.Game.CurrentVoteCategories, pair.Player.CategoryVoteId);
                            Interlocked.Decrement(ref prevCatVoteCount);
                        }

                        pair.Player.CategoryVoteId = categoryVotePacket.CategoryId;
                        Interlocked.Increment(ref voteCount);

                        await context.SendAsync(new GameCategoryVoteResponsePacket()
                        {
                            CategoryId = categoryVotePacket.CategoryId
                        });
                    }
                    else if (packet is GameQuestionAnswerRequestPacket questionAnswerPacket)
                    {
                        if (!_gameManager.TryGetGamePlayerPairByPlayer(context.User.Id, out var pair))
                            return;

                        if (pair.Game.State.Id != Network.Game.State.GameStateId.QuestionAnswering)
                            return;

                        var question = pair.Game.CurrentQuestions[pair.Game.CurrentQuestionIndex];
                        if (question.Id != questionAnswerPacket.QuestionId) return;
                        if (questionAnswerPacket.AnswerId < 0 || questionAnswerPacket.AnswerId >= question.Answers.Count) return;
                        if (Interlocked.CompareExchange(ref pair.Player.AnswerId, questionAnswerPacket.AnswerId, -1) != -1) return;

                        pair.Player.AnswerTime = DateTimeOffset.UtcNow.Subtract(pair.Game.LastStateSwitch);

                        List<string>[] playerAnswers = new List<string>[question.Answers.Count];
                        List<Task> taskAnswers = new List<Task>(pair.Game.Players.Count / 2 + 1);

                        var packetToPlayers = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new GamePlayerAnsweredResponsePacket()
                        {
                            AnswerId = questionAnswerPacket.AnswerId,
                            PlayerId = context.User.Id
                        }, IWebSocketConnectionManager.JsonJavascriptOptions));
                        var packetToPlayersSpan = new ReadOnlyMemory<byte>(packetToPlayers);

                        foreach (var player in pair.Game.Players)
                        {
                            if (player.Value.AnswerId < 0
                                || player.Value.AnswerId >= question.Answers.Count)
                                continue;

                            List<string> players = playerAnswers[player.Value.AnswerId];
                            if (players == null)
                            {
                                players = new List<string>(pair.Game.Players.Count / question.Answers.Count + 1);
                                playerAnswers[player.Value.AnswerId] = players;
                            }

                            players.Add(player.Key);

                            if (player.Value == pair.Player)
                                continue;

                            var connection = player.Value.Connection;
                            if (connection == null) continue;

                            taskAnswers.Add(connection.SendAsync(packetToPlayersSpan));
                        }

                        taskAnswers.Add(context.SendAsync(new GameAsnwerResultResponsePacket()
                        {
                            QuestionAnswer = new GameQuestionAnswerDto()
                            {
                                CorrectAnswerId = question.CorrectAnswer,
                                PlayerAnswers = playerAnswers
                            }
                        }));

                        await Task.WhenAll(taskAnswers);
                    }
                }
            }
            catch (Exception es) { _logger.LogError(es, "Exception during handle user packet"); }
        }
    }
}
