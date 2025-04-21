using QuickQuiz.API.Game;
using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Dto
{
    public class PlayerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Twitch { get; set; }

        public static PlayerDto Map(LobbyPlayer lobbyPlayer)
        {
            var player = new PlayerDto();

            player.Id = lobbyPlayer.Identity.Id;
            player.Name = lobbyPlayer.Identity.Name;
            player.Twitch = lobbyPlayer.Identity.Twitch;

            return player;
        }
    }
}
