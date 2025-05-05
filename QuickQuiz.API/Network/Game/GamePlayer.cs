using QuickQuiz.API.Network;
using QuickQuiz.API.Identities;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.Dto;

namespace QuickQuiz.API.Network.Game
{
    public class GamePlayer : NetworkPlayer
    {
        public string CategoryVoteId;
        public double Points;

        public List<bool> RoundAnswers = new List<bool>(5);
        public List<TimeSpan> AnswerTimes = new List<TimeSpan>(5);

        public int AnswerId;
        public TimeSpan AnswerTime;

        public GamePlayerDto MapToGamePlayerDto()
        {
            var player = new GamePlayerDto();

            player.Id = Identity.Id;
            player.Name = Identity.Name;
            player.AuthSource = Identity.AuthSource;
            player.RoundAnswers = RoundAnswers;

            return player;
        }
    }
}
