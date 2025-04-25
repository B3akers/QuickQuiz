
namespace QuickQuiz.API.Network.Game.State
{
    public class GameStateTerminate : GameState
    {
        public override GameStateId Id => GameStateId.Terminate;

        public override Task OnUpdate()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActivateCore()
        {
            return Task.CompletedTask;
        }
    }
}
