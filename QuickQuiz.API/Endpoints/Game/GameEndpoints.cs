using MongoDB.Driver;
using QuickQuiz.API.Database;
using QuickQuiz.API.Database.Structures;
using QuickQuiz.API.Extensions;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Utility;

namespace QuickQuiz.API.Endpoints.Game
{
    public static class GameEndpoints
    {
        public static void MapGameEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/game")
                .DisableAntiforgery()
                .WithOpenApi();

            group.MapGet("/categories", GetCategories);
            group.MapGet("/stats", GetStats);
            group.MapGet("/connection-token", GetConnectionToken).RequireAuthentication();
            group.MapPost("/create-lobby", CreateLobby).RequireAuthentication();
        }

        public record CreateLobbyRequest(string LobbyCode);
        private static IResult CreateLobby(IUserProvider userProvider, IConnectionTokenProvider connectionTokenProvider, ILobbyManager lobbyManager, CreateLobbyRequest request)
        {
            var user = userProvider.GetUser();
            var lobbyCode = request.LobbyCode;

            if (string.IsNullOrEmpty(request.LobbyCode) || request.LobbyCode.Length > 24)
                lobbyCode = Randomizer.RandomReadableString(6);

            if (!lobbyManager.IsLobbyCodeAvailable(lobbyCode))
            {
                return CustomResults.InvalidProperty(nameof(request.LobbyCode), "Lobby o takim kodzie już istnieje");
            }

            var lobby = lobbyManager.CreateLobby(user.Id, lobbyCode);
            if (lobby == null)
            {
                return CustomResults.GenericError("Wystąpił błąd podczas tworzenia lobby, spróbuj ponownie");
            }

            return Results.Json(new { Ok = true });
        }

        public record GetStatsResponse(int ActiveLobby, int ActivePlayers);
        private static IResult GetStats(ILobbyManager lobbyManager)
        {
            return Results.Json(new GetStatsResponse(
                lobbyManager.GetActiveLobbyCount(),
                lobbyManager.GetActivePlayersCount()));
        }

        public record ConnectionTokenResponse(string Token);
        private static IResult GetConnectionToken(IUserProvider userProvider, IConnectionTokenProvider connectionTokenProvider, ILobbyManager lobbyManager)
        {
            var user = userProvider.GetUser();
            return Results.Json(new ConnectionTokenResponse(connectionTokenProvider.CreateConnectionToken(user, TimeSpan.FromSeconds(15))));
        }

        private static async Task<IResult> GetCategories(MongoContext mongoContext)
        {
            return Results.Json(await (await mongoContext.Categories.FindAsync(Builders<Category>.Filter.Empty)).ToListAsync());
        }
    }
}
