using System.Security.Claims;

namespace QuickQuiz.API.Interfaces
{
    public class TokenClaims
    {
        public string UserId;
        public string Username;
        public bool AuthorizedTwitch;
    }

    public interface IJWTTokenProvider
    {
        string GenerateToken(List<Claim> claims, DateTime expiresAt);
        ClaimsPrincipal VerifyToken(string token);
        TokenClaims Authorize(string token);
    }
}
