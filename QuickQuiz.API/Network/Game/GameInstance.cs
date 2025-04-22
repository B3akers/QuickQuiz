using QuickQuiz.API.Database;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Network.Game.State;
using QuickQuiz.API.WebSockets;
using System.Collections.Concurrent;

namespace QuickQuiz.API.Network.Game
{
    public struct GameUpdateStatus
    {
        public string GameId;
        public bool Ended;
    }

    public class GameInstance
    {
        public readonly string Id;
        public GameState State;
        public readonly ConcurrentDictionary<string, GamePlayer> Players;
        public readonly MongoContext MongoContext;
        public DateTimeOffset LastStateSwitch;

        public GameInstance(string id, MongoContext context)
        {
            Id = id;
            Players = new();

            MongoContext = context;
        }

        public Task SwitchToCateogrySelection()
        {
            LastStateSwitch = DateTimeOffset.UtcNow;
            State = new GameStateCategorySelection() { Game = this };
            return State.OnActivate();
        }

        public async Task<GameUpdateStatus> Update()
        {
            if (State == null)
                return new GameUpdateStatus() { GameId = Id, Ended = false };

            await State.OnUpdate();

            return new GameUpdateStatus() { GameId = Id, Ended = State.Id == GameStateId.Terminate };
        }
    }
}