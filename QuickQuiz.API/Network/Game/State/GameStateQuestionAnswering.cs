
using QuickQuiz.API.Dto;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets.Packets;
using System.Text.Json;
using System.Text;

namespace QuickQuiz.API.Network.Game.State
{
    public class GameStateQuestionAnswering : GameState
    {
        public override GameStateId Id => GameStateId.QuestionAnswering;

        protected override async Task OnActivateCore()
        {
            var question = Game.CurrentQuestions[Game.CurrentQuestionIndex];

            foreach (var player in Game.Players)
            {
                player.Value.AnswerId = -1;
            }

            var currentTime = DateTimeOffset.UtcNow;
            await Game.Players.SendToAllPlayers(new GameQuestionAnsweringResponsePacket()
            {
                QuestionAnswering = new GameQuestionAnsweringDto()
                {
                    Question = GameQuestionDto.Map(question),
                    StartTime = currentTime,
                    EndTime = currentTime.AddSeconds(Game.Settings.QuestionAnswerTimeInSeconds),
                }
            });
        }

        public override async Task OnUpdate()
        {
            bool anyTimeoutPlayer = true;

            var delta = DateTimeOffset.UtcNow - Game.LastStateSwitch;
            if (delta < TimeSpan.FromSeconds(Game.Settings.QuestionAnswerTimeInSeconds))
            {
                if (Game.Players.Any(x => x.Value.AnswerId == -1))
                    return;

                anyTimeoutPlayer = false;
            }

            var question = Game.CurrentQuestions[Game.CurrentQuestionIndex];

            byte[] answersPacketData = null;
            List<Task> timeoutTasks = null;
            List<string> timeoutPlayers = null;
            if (anyTimeoutPlayer)
            {
                timeoutTasks = new List<Task>(Game.Players.Count / 4 + 1);
                timeoutPlayers = new List<string>(Game.Players.Count / 4 + 1);
                answersPacketData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new GameAsnwerResultResponsePacket()
                {
                    QuestionAnswer = new GameQuestionAnswerDto()
                    {
                        PlayerAnswers = Game.Players.GetPlayerAnswers(question.Answers.Count),
                        CorrectAnswerId = question.CorrectAnswer
                    }
                }, IWebSocketConnectionManager.JsonJavascriptOptions));
            }

            foreach (var player in Game.Players)
            {
                if (player.Value.AnswerId != question.CorrectAnswer)
                {
                    player.Value.RoundAnswers.Add(false);
                    player.Value.AnswerTimes.Add(TimeSpan.Zero);
                }
                else
                {
                    player.Value.RoundAnswers.Add(true);
                    player.Value.AnswerTimes.Add(player.Value.AnswerTime);
                }

                if (player.Value.AnswerId == -1)
                {
                    timeoutPlayers.Add(player.Key);

                    var connection = player.Value.Connection;
                    if (connection != null)
                        timeoutTasks.Add(connection.SendAsync(answersPacketData));
                }
            }

            if (timeoutTasks != null)
                await Task.WhenAll(timeoutTasks);

            await Game.Players.SendToAllPlayers(new GameAsnwerTimeoutResponsePacket()
            {
                PlayerIds = timeoutPlayers
            });

            var state = new GameStateQuestionAnswered()
            {
                Game = Game
            };
            await state.OnActivate();
        }
    }
}
