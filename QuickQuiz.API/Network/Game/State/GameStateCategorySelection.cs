
using QuickQuiz.API.Utility;
using QuickQuiz.API.WebSockets.Packets;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace QuickQuiz.API.Network.Game.State
{
    public class GameStateCategorySelection : GameState
    {
        public override GameStateId Id => GameStateId.CategorySelection;

        protected override async Task OnActivateCoreAsync()
        {
            foreach (var player in Game.Players)
            {
                player.Value.CategoryVoteId = null;

                player.Value.RoundAnswers.Clear();
                player.Value.RoundAnswers.EnsureCapacity(Game.Settings.QuestionCountPerRound);

                player.Value.AnswerTimes.Clear();
                player.Value.AnswerTimes.EnsureCapacity(Game.Settings.QuestionCountPerRound);
            }

            await Game.Players.SendToAllPlayersAsync(new GameClearPlayerAnswersResponsePacket());

            List<Database.Structures.Category> categories = new List<Database.Structures.Category>(Game.Settings.CategoryCountInVote);

            var skipCategories = new List<string>(Game.AcknowledgedCategories.Count + (Game.Settings.ExcludeCategories?.Count ?? 0) + (Game.Settings.IncludeCategories?.Count ?? 0));
            skipCategories.AddRange(Game.AcknowledgedCategories);
            skipCategories.AddRange(Game.Settings.ExcludeCategories ?? Enumerable.Empty<string>());

            if (Game.Settings.IncludeCategories != null)
            {
                var includeCategories = await Game.QuizProvider.GetCategoriesAsync(Game.Settings.IncludeCategories, skipCategories);
                skipCategories.AddRange(Game.Settings.IncludeCategories ?? Enumerable.Empty<string>());
                categories.AddRange(includeCategories.Take(Game.Settings.CategoryCountInVote));
            }

            var left = Game.Settings.CategoryCountInVote - categories.Count;
            if (left > 0)
                categories.AddRange(await Game.QuizProvider.GetRandomCategoriesAsync(left, 25, skipCategories));

            if (categories.Count == 0) //Sanity check
                categories = await Game.QuizProvider.GetRandomCategoriesAsync(Game.Settings.CategoryCountInVote, 25, Enumerable.Empty<string>());

            Game.CurrentCategories = categories;
            Game.CurrentCategoryRoundIndex++;
            Game.CurrentVoteCategories.Clear();

            foreach (var category in categories)
                Game.CurrentVoteCategories.TryAdd(category.Id, 0);

            var currentTime = DateTimeOffset.UtcNow;

            await Game.Players.SendToAllPlayersAsync(new GameCategoryVoteStartResponsePacket()
            {
                CategoryVote = new Dto.GameCategoryVoteDto()
                {
                    Categories = categories,
                    CategoryIndex = Game.CurrentCategoryRoundIndex,
                    MaxCategoryIndex = Game.Settings.MaxCategoryVotesCount,
                    StartTime = currentTime,
                    SelectedCategory = string.Empty,
                    EndTime = currentTime.AddSeconds(Game.Settings.CategoryVoteTimeInSeconds)
                }
            });
        }

        public override async Task OnUpdateAsync()
        {
            var delta = DateTimeOffset.UtcNow - Game.LastStateSwitch;
            if (delta < TimeSpan.FromSeconds(Game.Settings.CategoryVoteTimeInSeconds))
            {
                if (Game.Players.Any(x => string.IsNullOrEmpty(x.Value.CategoryVoteId)))
                    return;
            }

            List<string> categories = new List<string>();
            int bestVoteCount = -1;

            foreach (var category in Game.CurrentVoteCategories)
            {
                if (category.Value < bestVoteCount)
                    continue;

                if (category.Value == bestVoteCount)
                {
                    categories.Add(category.Key);
                    continue;
                }

                categories.Clear();
                categories.Add(category.Key);
                bestVoteCount = category.Value;
            }

            var winnerId = categories[RandomNumberGenerator.GetInt32(categories.Count)];
            var winner = Game.CurrentCategories.FirstOrDefault(x => x.Id == winnerId);
            var questions = await Game.QuizProvider.GetRandomQuestionsFromCategoryAsync(winnerId, Game.Settings.QuestionCountPerRound, Game.AcknowledgedQuestions);

            await Game.QuizProvider.IncreasePopularityAsync(winnerId, bestVoteCount);

            Game.CurrentQuestionCategory = winner;
            Game.CurrentQuestions = questions;
            Game.CurrentQuestionIndex = 0;

            Game.AcknowledgedCategories.Add(winnerId);
            Game.AcknowledgedQuestions.AddRange(questions.Select(x => x.Id));

            var nextState = new GameStatePrepareForQuestion()
            {
                Game = Game
            };

            await nextState.OnActivateAsync();
        }
    }
}
