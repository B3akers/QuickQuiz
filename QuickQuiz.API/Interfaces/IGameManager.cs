using QuickQuiz.API.Identities;
using QuickQuiz.API.Network.Game;
using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Interfaces
{
    public struct GamePlayerPair
    {
        public GameInstance Game;
        public GamePlayer Player;
    }

    public interface IGameManager
    {
        record GameTerminateArgs(string GameId);
        delegate void GameTerminateHandler(object sender, GameTerminateArgs e);
        event GameTerminateHandler OnGameTerminate;

        int GetActiveGameCount();
        int GetActivePlayersCount();
        GameInstance GetGameByPlayer(string playerId);
        bool TryGetGamePlayerPairByPlayer(string playerId, out GamePlayerPair pair);
        bool PlayerIsInGame(string playerId);
        bool IsGameActive(string gameId);
        bool TryTerminateGame(string gameId);
        Task<GameInstance> TryToCreateNewGameAsync(List<ApplicationIdentityJWT> players, GameSettings settings = null);
        Task OnUpdateAsync();
    }
}
