using QuickQuiz.API.Identities;
using QuickQuiz.API.Network.Game;
using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Interfaces
{
    public interface IGameManager
    {
        record GameTerminateArgs(string GameId);
        delegate void GameTerminateHandler(object sender, GameTerminateArgs e);
        event GameTerminateHandler OnGameTerminate;

        int GetActiveGameCount();
        int GetActivePlayersCount();
        GameInstance GetGameByPlayer(string playerId);
        bool PlayerIsInGame(string playerId);
        bool IsGameActive(string gameId);
        bool TryTerminateGame(string gameId);
        Task<GameInstance> TryToCreateNewGame(List<ApplicationIdentityJWT> players);
        Task OnUpdate();
    }
}
