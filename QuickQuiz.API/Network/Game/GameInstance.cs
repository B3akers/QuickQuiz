using QuickQuiz.API.Database;
using QuickQuiz.API.Dto;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Network.Game.State;
using QuickQuiz.API.WebSockets;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
        public readonly IQuizProvider QuizProvider;
        public GameSettings Settings;

        //CategorySelection stage
        //
        public readonly Dictionary<string, int> CurrentVoteCategories;
        public int CurrentCategoryRoundIndex;
        public List<string> AcknowledgedCategories;
        public List<Database.Structures.Category> CurrentCategories;

        //Questions stage
        //
        public List<Database.Structures.Question> CurrentQuestions;
        public Database.Structures.Category CurrentQuestionCategory;
        public int CurrentQuestionIndex;

        public List<string> AcknowledgedQuestions;
        public DateTimeOffset LastStateSwitch;

        public GameInstance(string id, IQuizProvider quizProvider)
        {
            Id = id;
            Players = new();
            CurrentVoteCategories = new();
            AcknowledgedQuestions = new();
            AcknowledgedCategories = new();
            Settings = new();

            QuizProvider = quizProvider;
        }

        public async Task SwitchToCateogrySelection()
        {
            var state = new GameStateCategorySelection() { Game = this };
            await state.OnActivate();
        }

        public async Task<GameUpdateStatus> Update()
        {
            if (State == null)
                return new GameUpdateStatus() { GameId = Id, Ended = false };

            bool terminateGameNoPlayers = true;

            var currentTime = DateTimeOffset.UtcNow;
            foreach (var player in Players)
            {
                if (player.Value.Connection != null)
                {
                    terminateGameNoPlayers = false;
                    break;
                }

                if (currentTime - player.Value.LastConnectionUpdate < TimeSpan.FromMinutes(1))
                {
                    terminateGameNoPlayers = false;
                    break;
                }
            }

            if (terminateGameNoPlayers)
                return new GameUpdateStatus() { GameId = Id, Ended = true };

            await State.OnUpdate();

            return new GameUpdateStatus() { GameId = Id, Ended = State.Id == GameStateId.Terminate };
        }
    }

    public static class GamePlayersExtension
    {
        public static Dictionary<string, GamePlayerDto> ToPlayersDto(this ConcurrentDictionary<string, GamePlayer> players)
        {
            var dict = new Dictionary<string, GamePlayerDto>(players.Count);
            foreach (var pair in players)
                dict.Add(pair.Key, pair.Value.MapToGamePlayerDto());

            return dict;
        }

        public static List<string>[] GetPlayerAnswers(this ConcurrentDictionary<string, GamePlayer> players, int answerCount)
        {
            List<string>[] playerAnswers = new List<string>[answerCount];
            foreach (var player in players)
            {
                if (player.Value.AnswerId < 0
                    || player.Value.AnswerId >= answerCount)
                    continue;

                List<string> playerIdList = playerAnswers[player.Value.AnswerId];
                if (playerIdList == null)
                {
                    playerIdList = new List<string>();
                    playerAnswers[player.Value.AnswerId] = playerIdList;
                }

                playerIdList.Add(player.Key);
            }

            return playerAnswers;
        }
    }
}