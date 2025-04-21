using QuickQuiz.API.Identities;
using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Game
{
    public class LobbyPlayer
    {
        public ApplicationIdentityJWT Identity;
        public WebSocketConnectionContext Connection;
        public long LastConnectionUpdate;
    }
}
