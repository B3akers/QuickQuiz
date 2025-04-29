
namespace QuickQuiz.API.Network.Game.State
{
    public class GameStateQuestionAnswered : GameState
    {
        public override GameStateId Id => GameStateId.QuestionAnswered;

        public override async Task OnUpdate()
        {
            var delta = DateTimeOffset.UtcNow - Game.LastStateSwitch;
            if (delta < TimeSpan.FromSeconds(2))
                return;

            Game.CurrentQuestionIndex++;

            if (Game.CurrentQuestionIndex < Game.CurrentQuestions.Count)
            {
                var state = new GameStatePrepareForQuestion()
                {
                    Game = Game
                };
                await state.OnActivate();
                return;
            }

            var roundEndState = new GameStateRoundEnd()
            {
                Game = Game
            };
            await roundEndState.OnActivate();
        }

        protected override Task OnActivateCore()
        {
            return Task.CompletedTask;
        }
    }
}
