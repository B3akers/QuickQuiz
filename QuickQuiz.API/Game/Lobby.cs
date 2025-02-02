using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Game
{
    public class Lobby
    {
        public readonly string Id;
        public required string OwnerId;
        public required ILobbyAccess Access;
    }
}
