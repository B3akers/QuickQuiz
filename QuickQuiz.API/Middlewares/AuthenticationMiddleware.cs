using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Identities;
using System.Security.Principal;

namespace QuickQuiz.API.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJWTTokenProvider _tokenProvider;
        public AuthenticationMiddleware(RequestDelegate next, IJWTTokenProvider tokenProvider)
        {
            _next = next;
            _tokenProvider = tokenProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string token = string.Empty;
            if (context.Request.Headers.TryGetValue("Authorization", out var authorizationToken))
            {
                const string tokenPrefix = "Bearer ";

                if (authorizationToken.Count == 1 && !string.IsNullOrEmpty(authorizationToken[0]) && authorizationToken[0].StartsWith(tokenPrefix))
                {
                    token = authorizationToken[0].Substring(tokenPrefix.Length);
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            var tokenClaims = _tokenProvider.Authorize(token);
            if (tokenClaims != null)
            {
                var identity = new ApplicationIdentityJWT(tokenClaims.UserId, tokenClaims.Username, tokenClaims.AuthorizedTwitch);
                context.User = new GenericPrincipal(identity, null);
            }

            await _next(context);
        }
    }
}
