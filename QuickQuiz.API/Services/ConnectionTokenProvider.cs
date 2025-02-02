using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using QuickQuiz.API.Identities;
using QuickQuiz.API.Interfaces;
using System.Security.Cryptography;

namespace QuickQuiz.API.Services
{
    public class ConnectionTokenProvider : IConnectionTokenProvider
    {
        class TokenValue
        {
            public ApplicationIdentityJWT Identity { get; set; }
            public DateTimeOffset IssuedAt { get; set; }
        }

        private readonly IMemoryCache _memoryCache;

        public ConnectionTokenProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string CreateConnectionToken(ApplicationIdentityJWT identity, TimeSpan expirationTime)
        {
            var token = CreateConnectionId();

            _memoryCache.Set(token, new TokenValue
            {
                Identity = identity,
                IssuedAt = DateTimeOffset.UtcNow
            }, expirationTime);

            return token;
        }

        public ApplicationIdentityJWT Authenticate(string connectionToken)
        {
            if (_memoryCache.TryGetValue(connectionToken, out var cachedValue) && cachedValue is TokenValue tokenValue)
            {
                _memoryCache.Remove(connectionToken);
                return tokenValue.Identity;
            }

            return null;
        }

        public string CreateConnectionId()
        {
            Span<byte> buffer = stackalloc byte[32];
            RandomNumberGenerator.Fill(buffer);
            return WebEncoders.Base64UrlEncode(buffer);
        }
    }
}
