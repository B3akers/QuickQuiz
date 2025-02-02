using QuickQuiz.API.Game;

namespace QuickQuiz.API.Interfaces
{
    public interface ILobbyManager
    {
        bool IsLobbyCodeAvailable(string lobbyCode);
        int GetActiveLobbyCount();
        int GetActivePlayersCount();
        Lobby GetLobbyByPlayer(string playerId);
        Lobby CreateLobby(string ownerId, string lobbyCode);
    }
}
