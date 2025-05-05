using QuickQuiz.API.Dto;
using QuickQuiz.API.Identities;
using QuickQuiz.API.WebSockets;

namespace QuickQuiz.API.Network
{
    public class NetworkPlayer
    {
        public ApplicationIdentityJWT Identity;
        public WebSocketConnectionContext Connection;
        public DateTimeOffset LastConnectionUpdate;

        public PlayerDto MapToPlayerDto()
        {
            var player = new PlayerDto();

            player.Id = Identity.Id;
            player.Name = Identity.Name;
            player.AuthSource = Identity.AuthSource;

            return player;
        }

    }
}
