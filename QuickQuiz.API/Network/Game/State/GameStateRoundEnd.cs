
using QuickQuiz.API.WebSockets.Packets;

namespace QuickQuiz.API.Network.Game.State
{
    public class GameStateRoundEnd : GameState
    {
        public override GameStateId Id => GameStateId.RoundEnd;
        protected override async Task OnActivateCore()
        {
            await Game.Players.SendToAllPlayers(new GamePrepareForQuestionResponsePacket()
            {
                PrepareForQuestion = new Dto.GamePrepareForQuestionDto()
                {
                    Category = Game.CurrentQuestionCategory,
                    QuestionCount = Game.CurrentQuestions.Count,
                    QuestionIndex = Game.CurrentQuestionIndex,
                }
            });
        }

        public override async Task OnUpdate()
        {
            var delta = DateTimeOffset.UtcNow - Game.LastStateSwitch;
            if (delta < TimeSpan.FromSeconds(2.5))
                return;

            if (Game.CurrentCategoryRoundIndex == Game.Settings.MaxCategoryVotesCount)
            {
                var terminateState = new GameStateTerminate()
                {
                    Game = Game
                };
                await terminateState.OnActivate();
                return;
            }

            var categorySelectionState = new GameStateCategorySelection()
            {
                Game = Game
            };
            await categorySelectionState.OnActivate();
        }
    }
}
