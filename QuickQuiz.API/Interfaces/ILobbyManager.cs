using QuickQuiz.API.Game;
using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Interfaces
{
    public interface ILobbyManager
    {
        bool IsLobbyCodeAvailable(string lobbyCode);
        int GetActiveLobbyCount();
        int GetActivePlayersCount();
        Lobby GetLobbyByPlayer(string playerId);
        bool PlayerIsInLobby(string playerId);
        Lobby CreateLobby(ApplicationIdentityJWT owner, string lobbyCode);
        Task<bool> TryAddPlayerToLobby(ApplicationIdentityJWT player, string lobbyCode);
        Task<bool> TryRemovePlayerFromLobby(ApplicationIdentityJWT player);
        Task OnUpdate();
    }
}
