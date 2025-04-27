using QuickQuiz.API.Network.Game;
using QuickQuiz.API.Network.Lobby;

namespace QuickQuiz.API.Dto
{
    public class GamePlayerDto : PlayerDto
    {
        public List<bool> RoundAnswers { get; set; }
    }
}
