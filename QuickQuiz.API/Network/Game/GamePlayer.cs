using QuickQuiz.API.Network;
using QuickQuiz.API.Identities;
using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Network.Game
{
    public class GamePlayer : NetworkPlayer
    {
        public string CategoryVoteId;
        public float Points;

        public List<bool> RoundAnswers = new List<bool>(5);
        public List<TimeSpan> AnswerTimes = new List<TimeSpan>(5);

        public int AnswerId;
        public TimeSpan AnswerTime;
    }
}
