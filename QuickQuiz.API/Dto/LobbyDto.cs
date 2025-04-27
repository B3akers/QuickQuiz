using QuickQuiz.API.Network;

namespace QuickQuiz.API.Dto
{
    public class LobbyDto
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string ActiveGameId { get; set; }
        public List<PlayerDto> Players { get; set; }
        public LobbySettingsDto Settings { get; set; }
    }
}
