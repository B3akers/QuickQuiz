using QuickQuiz.API.Exceptions;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces;

namespace QuickQuiz.API.Services
{
    public class UserProviderService : IUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserProviderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ApplicationIdentityJWT GetUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null || httpContext.User == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                throw new UserNotAuthenticatedException();

            if (httpContext.User.Identity is not ApplicationIdentityJWT user)
                throw new UserNotAuthenticatedException();

            return user;
        }
    }
}
