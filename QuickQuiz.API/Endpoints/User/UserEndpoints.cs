using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QuickQuiz.API.Claims;
using QuickQuiz.API.Extensions;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Settings;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace QuickQuiz.API.Endpoints.User
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/user")
                .DisableAntiforgery()
                .WithOpenApi();

            group.MapGet("/session", GetSession).RequireAuthentication();
            group.MapGet("/twitch-client-id", TwitchClientId).RequireUnauthenticatedOnly();
            group.MapPost("/twitch-login", TwitchLoginAsync).RequireUnauthenticatedOnly();
            group.MapPost("/create", CreateUserAsync).RequireUnauthenticatedOnly();
        }

        public record TwitchClientIdResponse(string ClientId);
        private static IResult TwitchClientId(IOptions<TwitchSettings> settings)
        {
            return Results.Json(new TwitchClientIdResponse(settings.Value.ClientId));
        }

        public record TwitchTokenRequest(string Code, string RedirectUrl);
        public record TwitchTokenResponse(string access_token, int expires_in, string refresh_token, string[] scope, string token_type);
        private static async Task<IResult> TwitchLoginAsync(IJWTTokenProvider tokenProvider, IHttpClientFactory httpClientFactory, IOptions<TwitchSettings> settings, TwitchTokenRequest twitchTokenRequest)
        {
            if (string.IsNullOrEmpty(twitchTokenRequest.Code) || string.IsNullOrEmpty(twitchTokenRequest.RedirectUrl)) return Results.BadRequest();

            var httpClient = httpClientFactory.CreateClient();
            var currentSettings = settings.Value;

            TwitchTokenResponse tokenResponse = null;
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", currentSettings.ClientId },
                    { "client_secret", currentSettings.ClientSecret },
                    { "code", twitchTokenRequest.Code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", twitchTokenRequest.RedirectUrl },
                };

                var content = new FormUrlEncodedContent(values);

                var message = new HttpRequestMessage();
                message.RequestUri = new Uri("https://id.twitch.tv/oauth2/token");
                message.Method = HttpMethod.Post;
                message.Content = content;

                var response = await httpClient.SendAsync(message);
                response.EnsureSuccessStatusCode();

                tokenResponse = await response.Content.ReadFromJsonAsync<TwitchTokenResponse>();
            }
            if (tokenResponse == null) return Results.BadRequest();

            string login = null;
            string id = null;
            {
                var message = new HttpRequestMessage();
                message.RequestUri = new Uri("https://api.twitch.tv/helix/users");
                message.Method = HttpMethod.Get;
                message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {tokenResponse.access_token}");
                message.Headers.TryAddWithoutValidation("Client-Id", currentSettings.ClientId);

                var response = await httpClient.SendAsync(message);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<JsonObject>();
                var user = result["data"][0];

                login = user["login"].ToString();
                id = user["id"].ToString();
            }

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(id)) return Results.BadRequest();

            return Results.Json(new CreateUserResponse(tokenProvider.GenerateToken([
                new Claim(JwtClaimTypes.UserId, id),
                new Claim(JwtClaimTypes.Username, login),
                new Claim(JwtClaimTypes.AuthorizedTwitch, true.ToString())
                ], DateTime.UtcNow.AddYears(1))));
        }

        public record CreateUserRequest(string Username);
        public record CreateUserResponse(string Token);
        public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
        {
            public CreateUserRequestValidator()
            {
                RuleFor(x => x.Username)
                    .NotEmpty().WithMessage("To pole jest wymagane")
                    .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("Dozwolone są tylko znaki alfanumeryczne")
                    .Length(3, 24).WithMessage("Musi mieć od 3 do 24 znaków długości");
            }
        }
        private static async Task<IResult> CreateUserAsync(CreateUserRequest request, IJWTTokenProvider tokenProvider, IValidator<CreateUserRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return Results.Json(new CreateUserResponse(tokenProvider.GenerateToken([
                new Claim(JwtClaimTypes.UserId, Guid.NewGuid().ToString()),
                new Claim(JwtClaimTypes.Username, request.Username)
                ], DateTime.UtcNow.AddYears(1))));
        }

        private static IResult GetSession(IUserProvider userProvider)
        {
            var user = userProvider.GetUser();
            return Results.Json(user);
        }
    }
}
