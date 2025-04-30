
using QuickQuiz.API.WebSockets.Packets;

namespace QuickQuiz.API.Network.Game.State
{
    public class GameStatePrepareForQuestion : GameState
    {
        public override GameStateId Id => GameStateId.PrepareForQuestion;

        protected override async Task OnActivateCoreAsync()
        {
            await Game.Players.SendToAllPlayersAsync(new GamePrepareForQuestionResponsePacket()
            {
                PrepareForQuestion = new Dto.GamePrepareForQuestionDto()
                {
                    Category = Game.CurrentQuestionCategory,
                    PreloadImage = Game.CurrentQuestions[Game.CurrentQuestionIndex].Image,
                    QuestionCount = Game.CurrentQuestions.Count,
                    QuestionIndex = Game.CurrentQuestionIndex,
                }
            });
        }

        public override async Task OnUpdateAsync()
        {
            var delta = DateTimeOffset.UtcNow - Game.LastStateSwitch;
            if (delta < TimeSpan.FromSeconds(1.5))
                return;

            var nextState = new GameStateQuestionAnswering() { Game = Game };
            await nextState.OnActivateAsync();
        }
    }
}
