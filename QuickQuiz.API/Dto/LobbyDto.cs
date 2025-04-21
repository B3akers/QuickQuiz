using QuickQuiz.API.Game;

namespace QuickQuiz.API.Dto
{
    public class LobbyDto
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public List<PlayerDto> Players { get; set; }
        public int MaxPlayers { get; set; }
    }
}
