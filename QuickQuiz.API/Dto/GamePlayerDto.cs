using QuickQuiz.API.Network.Game;
using QuickQuiz.API.Network.Lobby;

namespace QuickQuiz.API.Dto
{
    public class GamePlayerDto : PlayerDto
    {
        public List<bool> RoundAnswers { get; set; }
        public float Points { get; set; }
        public static GamePlayerDto Map(GamePlayer gamePlayer)
        {
            var player = new GamePlayerDto();

            player.Id = gamePlayer.Identity.Id;
            player.Name = gamePlayer.Identity.Name;
            player.Twitch = gamePlayer.Identity.Twitch;
            player.RoundAnswers = gamePlayer.RoundAnswers;
            player.Points = gamePlayer.Points;

            return player;
        }
    }
}
