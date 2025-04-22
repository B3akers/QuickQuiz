using QuickQuiz.API.Network.Lobby;
using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Interfaces
{
    public interface ILobbyManager
    {
        bool IsLobbyCodeAvailable(string lobbyCode);
        int GetActiveLobbyCount();
        int GetActivePlayersCount();
        Lobby GetLobbyByPlayer(string playerId);
        Lobby GetLobbyById(string lobbyId);
        bool PlayerIsInLobby(string playerId);
        bool LobbyIsInGame(Lobby lobby);
        Task<bool> LobbyStartGame(Lobby lobby);
        Lobby CreateLobby(ApplicationIdentityJWT owner, string lobbyCode);
        Task<bool> TryAddPlayerToLobby(ApplicationIdentityJWT player, string lobbyCode);
        Task<bool> TryRemovePlayerFromLobby(ApplicationIdentityJWT player);
        Task<bool> TryKickPlayerFromLobby(Lobby lobby, ApplicationIdentityJWT player);
        Task OnUpdate();
    }
}
