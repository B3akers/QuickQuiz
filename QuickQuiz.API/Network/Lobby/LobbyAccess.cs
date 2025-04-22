using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Network.Lobby
{
    public interface ILobbyAccess
    {
        public bool CanJoin(ApplicationIdentityJWT user);
    };

    public class AllLobbyAccess : ILobbyAccess
    {
        public bool CanJoin(ApplicationIdentityJWT user)
        {
            return true;
        }
    }

    public class TwitchLobbyAccess : ILobbyAccess
    {
        public bool CanJoin(ApplicationIdentityJWT user)
        {
            return user.Twitch;
        }
    }
}
