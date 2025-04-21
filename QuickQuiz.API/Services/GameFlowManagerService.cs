using QuickQuiz.API.Dto;
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
        public GameFlowManagerService(ILobbyManager lobbyManager, IGameManager gameManager)
        {
            _lobbyManager = lobbyManager;
            _gameManager = gameManager;
        }

        public async Task ProcessPacket(WebSocketConnectionContext context, BasePacketRequest packet)
        {
            if (packet is GameStateRequestPacket)
            {
                var response = new GameStateResponsePacket();
                var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                if (lobby != null)
                {
                    response.Lobby = lobby.MapToDto();
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
                if (lobby != null
                    && lobby.OwnerId == context.User.Id)
                {
                    if (lobby.Players.TryGetValue(lobbyKickRequest.PlayerId, out var player))
                        await _lobbyManager.TryRemovePlayerFromLobby(player.Identity);
                }
            }
            else if (packet is LobbyPlayerPromoteRequestPacket lobbyPromoteRequest)
            {
                var lobby = _lobbyManager.GetLobbyByPlayer(context.User.Id);
                if (lobby != null
                    && lobby.OwnerId == context.User.Id
                    && lobby.OwnerId != lobbyPromoteRequest.PlayerId)
                {
                    if (lobby.Players.TryGetValue(lobbyPromoteRequest.PlayerId, out var player))
                    {
                        lobby.OwnerId = player.Identity.Id;
                        await lobby.SendToAllPlayers(new LobbyTransferOwnerResponsePacket() { PlayerId = lobby.OwnerId });
                    }
                }
            }
            else if (packet is LobbyGameStartRequestPacket)
            {

            }
        }
    }
}
