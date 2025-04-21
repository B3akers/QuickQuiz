using QuickQuiz.API.Game;

namespace QuickQuiz.API.Interfaces
{
    public interface IGameManager
    {
        int GetActiveGameCount();
        int GetActivePlayersCount();
        GameInstance GetGameByPlayer(string playerId);
        bool PlayerIsInGame(string playerId);
        Task OnUpdate();
    }
}
