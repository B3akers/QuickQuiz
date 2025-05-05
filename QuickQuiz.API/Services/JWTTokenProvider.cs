using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickQuiz.API.Claims;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuickQuiz.API.Services
{
    public class JWTTokenProvider : IJWTTokenProvider
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSettings _jwtSettings;

        public JWTTokenProvider(IOptions<JwtSettings> settings)
        {
            _jwtSettings = settings.Value;
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Key)), SecurityAlgorithms.HmacSha256Signature);
        }

        public TokenClaims Authorize(string token)
        {
            var claimsPrincipal = VerifyToken(token);
            if (claimsPrincipal == null)
                return null;

            var claims = new TokenClaims();

            foreach (var claim in claimsPrincipal.Claims)
            {
                switch (claim.Type)
                {
                    case JwtClaimTypes.UserId:
                        claims.UserId = claim.Value;
                        break;
                    case JwtClaimTypes.Username:
                        claims.Username = claim.Value;
                        break;
                    case JwtClaimTypes.AuthorizedTwitch:
                        claims.AuthSource = bool.Parse(claim.Value) ? "twitch" : string.Empty;
                        break;
                    case JwtClaimTypes.AuthSource:
                        claims.AuthSource = claim.Value;
                        break;
                }
            }

            if (string.IsNullOrEmpty(claims.UserId) || string.IsNullOrEmpty(claims.Username))
                return null;

            return claims;
        }

        public string GenerateToken(List<Claim> claims, DateTime expiresAt)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = _signingCredentials,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingCredentials.Key,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                if (claimsPrincipal != null && validatedToken != null)
                {
                    return claimsPrincipal;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
