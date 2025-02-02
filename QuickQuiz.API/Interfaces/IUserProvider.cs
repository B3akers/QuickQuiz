using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Interfaces
{
    public interface IUserProvider
    {
        ApplicationIdentityJWT GetUser();
    }
}
