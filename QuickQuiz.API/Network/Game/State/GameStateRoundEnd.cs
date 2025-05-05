
using QuickQuiz.API.WebSockets.Packets;

namespace QuickQuiz.API.Network.Game.State
{
    public class GameStateRoundEnd : GameState
    {
        public override GameStateId Id => GameStateId.RoundEnd;
        protected override async Task OnActivateCoreAsync()
        {
            await Game.Players.SendToAllPlayersAsync(new GamePrepareForQuestionResponsePacket()
            {
                PrepareForQuestion = new Dto.GamePrepareForQuestionDto()
                {
                    Category = Game.CurrentQuestionCategory,
                    QuestionCount = Game.CurrentQuestions.Count,
                    QuestionIndex = Game.CurrentQuestionIndex,
                }
            });

            var questionCount = Game.CurrentQuestions.Count;
            double playersCount = Game.Players.Count;

            double[] questionsAnswerTimesAvg = new double[questionCount];
            int[] questionsAnswersCount = new int[questionCount];

            foreach (var player in Game.Players)
            {
                for (var i = 0; i < player.Value.RoundAnswers.Count; i++)
                {
                    if (i >= questionCount) break;
                    if (!player.Value.RoundAnswers[i]) continue;

                    questionsAnswersCount[i]++;
                    questionsAnswerTimesAvg[i] += player.Value.AnswerTimes[i].TotalMilliseconds;
                }
            }

            for (var i = 0; i < questionCount; i++)
            {
                if (questionsAnswersCount[i] == 0)
                    continue;

                questionsAnswerTimesAvg[i] /= questionsAnswersCount[i];
            }

            foreach (var player in Game.Players)
            {
                int goodAnswers = 0;

                for (var i = 0; i < player.Value.RoundAnswers.Count; i++)
                {
                    if (i >= questionCount) break;
                    if (!player.Value.RoundAnswers[i])
                    {
                        if (!Game.Settings.PenaltyPointsForWrongAnswer)
                            continue;

                        if (player.Value.AnswerTimes[i] == TimeSpan.Zero)
                            continue;

                        player.Value.Points = Math.Max(0, player.Value.Points - 50.0);

                        continue;
                    }

                    var timeMulti = questionsAnswerTimesAvg[i] / player.Value.AnswerTimes[i].TotalMilliseconds;
                    var timeBonusPoints = timeMulti > 1 ? (50.0 * Math.Min(timeMulti - 1, 1)) : 0;

                    var questionDifficultyMulti = playersCount / questionsAnswersCount[i];
                    var questionDifficultyPoints = questionDifficultyMulti > 1 ? (50.0 * Math.Min(questionDifficultyMulti - 1, 1)) : 0;

                    if (!Game.Settings.CalculatePointsTimeFactor)
                        timeBonusPoints = 0;

                    if (!Game.Settings.CalculatePointsDifficultyFactor)
                        questionDifficultyPoints = 0;

                    player.Value.Points += (100.0 + timeBonusPoints + questionDifficultyPoints);
                    goodAnswers++;
                }

                if (goodAnswers == player.Value.RoundAnswers.Count && Game.Settings.AddPointsForWinStreak)
                {
                    player.Value.Points += 100.0;
                }
            }
        }

        public override async Task OnUpdateAsync()
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
                await terminateState.OnActivateAsync();
                return;
            }

            var categorySelectionState = new GameStateCategorySelection()
            {
                Game = Game
            };
            await categorySelectionState.OnActivateAsync();
        }
    }
}
