using QuickQuiz.API.Identities;
using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Network
{
    public class NetworkPlayer
    {
        public ApplicationIdentityJWT Identity;
        public WebSocketConnectionContext Connection;
        public DateTimeOffset LastConnectionUpdate;
    }
}
