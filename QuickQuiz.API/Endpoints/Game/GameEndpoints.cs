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

            group.MapGet("/categories-desc", GetCategoriesDescAsync);
            group.MapGet("/categories", GetCategoriesAsync);
            group.MapGet("/stats", GetStats);
            group.MapGet("/connection-token", GetConnectionToken).RequireAuthentication();
            group.MapPost("/create-lobby", CreateLobby).RequireAuthentication();
            group.MapPost("/join-lobby", JoinLobbyAsync).RequireAuthentication();
        }

        public record CreateLobbyRequest(string LobbyCode);
        private static async Task<IResult> JoinLobbyAsync(IUserProvider userProvider, ILobbyManager lobbyManager, CreateLobbyRequest request)
        {
            var user = userProvider.GetUser();

            if (string.IsNullOrEmpty(request.LobbyCode))
            {
                return CustomResults.InvalidProperty(nameof(request.LobbyCode), "Lobby o takim kodzie nie istnieje");
            }

            if (lobbyManager.PlayerIsInLobby(user.Id))
            {
                return CustomResults.GenericError("Musisz najpierw opuścić lobby");
            }

            if (lobbyManager.IsLobbyCodeAvailable(request.LobbyCode))
            {
                return CustomResults.InvalidProperty(nameof(request.LobbyCode), "Lobby o takim kodzie nie istnieje");
            }

            var result = await lobbyManager.TryAddPlayerToLobbyAsync(user, request.LobbyCode);
            if (!result)
            {
                return CustomResults.GenericError("Wystąpił błąd, nie spełniasz wymagań aby dołączyć do podanego lobby");
            }

            return Results.Json(new { Ok = true });
        }

        private static IResult CreateLobby(IUserProvider userProvider, ILobbyManager lobbyManager, CreateLobbyRequest request)
        {
            var user = userProvider.GetUser();
            var lobbyCode = request.LobbyCode;

            if (string.IsNullOrEmpty(request.LobbyCode) || request.LobbyCode.Length > 24)
                lobbyCode = Randomizer.RandomReadableString(6);

            if (!lobbyManager.IsLobbyCodeAvailable(lobbyCode))
            {
                return CustomResults.InvalidProperty(nameof(request.LobbyCode), "Lobby o takim kodzie już istnieje");
            }

            if (lobbyManager.PlayerIsInLobby(user.Id))
            {
                return CustomResults.GenericError("Musisz najpierw opuścić lobby");
            }

            var lobby = lobbyManager.CreateLobby(user, lobbyCode);
            if (lobby == null)
            {
                return CustomResults.GenericError("Wystąpił błąd podczas tworzenia lobby, spróbuj ponownie");
            }

            return Results.Json(new { Ok = true });
        }

        public record GetStatsResponse(int ActiveLobby, int ActiveLobbyPlayers, int ActiveGames, int ActivePlayers);
        private static IResult GetStats(ILobbyManager lobbyManager, IGameManager gameManager)
        {
            return Results.Json(new GetStatsResponse(
                lobbyManager.GetActiveLobbyCount(),
                lobbyManager.GetActivePlayersCount(),
                gameManager.GetActiveGameCount(),
                gameManager.GetActivePlayersCount()));
        }

        public record ConnectionTokenResponse(string Token);
        private static IResult GetConnectionToken(IUserProvider userProvider, IConnectionTokenProvider connectionTokenProvider, ILobbyManager lobbyManager)
        {
            var user = userProvider.GetUser();
            return Results.Json(new ConnectionTokenResponse(connectionTokenProvider.CreateConnectionToken(user, TimeSpan.FromSeconds(15))));
        }

        private static async Task<IResult> GetCategoriesAsync(MongoContext mongoContext)
        {
            return Results.Json(await (await mongoContext.Categories.FindAsync(Builders<Category>.Filter.Empty)).ToListAsync());
        }

        private static async Task<IResult> GetCategoriesDescAsync(MongoContext mongoContext)
        {
            return Results.Json((await (await mongoContext.Categories.FindAsync(Builders<Category>.Filter.Empty)).ToListAsync()).Select(x => new { x.Id, x.Label }));
        }
    }
}
