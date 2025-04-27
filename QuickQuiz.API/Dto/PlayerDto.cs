using QuickQuiz.API.Network.Lobby;
using QuickQuiz.API.Network;

namespace QuickQuiz.API.Dto
{
    public class PlayerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Twitch { get; set; }
    }
}
