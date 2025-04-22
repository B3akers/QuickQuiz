using QuickQuiz.API.Dto;
using QuickQuiz.API.Network;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;
using QuickQuiz.API.WebSockets.Packets;

namespace QuickQuiz.API.Services
{
    public class GameFlowManagerService : IGameFlowManager
    {
        private readonly ILobbyManager _lobbyManager;
        private readonly IGameManager _gameManager;
        private readonly ILogger<GameFlowManagerService> _logger;
        public GameFlowManagerService(ILobbyManager lobbyManager, IGameManager gameManager, ILogger<GameFlowManagerService> logger)
        {
            _lobbyManager = lobbyManager;
            _gameManager = gameManager;
            _logger = logger;
        }

        public async Task ProcessPacket(WebSocketConnectionContext context, BasePacketRequest packet)
        {
            try
            {
                if (packet is GameStateRequestPacket)
                {
                    var response = new GameStateResponsePacket();

                    var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                    if (lobby != null)
                    {
                        response.Lobby = lobby.MapToDto();
                    }

                    var game = _gameManager.GetGameByPlayer(context.User.Id);
                    if (game != null)
                    {

                    }

                    await context.SendAsync(response);
                }
                else if (packet is LobbyPlayerQuitRequestPacket)
                {
                    await _lobbyManager.TryRemovePlayerFromLobby(context.User);
                }
                else if (packet is LobbyPlayerKickRequestPacket lobbyKickRequest)
                {
                    var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                    if (lobby == null)
                        return;

                    if (lobby.OwnerId != context.User.Id)
                        return;

                    if (lobby.Players.TryGetValue(lobbyKickRequest.PlayerId, out var player))
                        await _lobbyManager.TryKickPlayerFromLobby(lobby, player.Identity);
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
                        await lobby.Players.SendToAllPlayers(new LobbyTransferOwnerResponsePacket() { PlayerId = lobby.OwnerId }, Enumerable.Empty<string>());
                    }
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

                    if (!await _lobbyManager.LobbyStartGame(lobby))
                    {
                        await context.SendAsync(new ShowToastResponsePacket() { Code = "lobby_failed_to_start_game" });
                        return;
                    }
                }
            }
            catch (Exception es) { _logger.LogError(es, "Exception during handle user packet"); }
        }
    }
}
