using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Interfaces
{
    public interface IConnectionTokenProvider
    {
        string CreateConnectionId();
        string CreateConnectionToken(ApplicationIdentityJWT identity, TimeSpan expirationTime);
        ApplicationIdentityJWT Authenticate(string connectionToken);
    }
}
