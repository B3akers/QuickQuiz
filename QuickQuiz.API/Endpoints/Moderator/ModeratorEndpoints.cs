using MongoDB.Driver;
using QuickQuiz.API.Claims;
using QuickQuiz.API.Database;
using QuickQuiz.API.Database.Structures;
using QuickQuiz.API.Extensions;
using QuickQuiz.API.Interfaces;

namespace QuickQuiz.API.Endpoints.Moderator
{
    public static class ModeratorEndpoints
    {
        public static void MapModeratorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/moderator")
                .DisableAntiforgery()
                .WithOpenApi();

            group.MapGet("/active-lobbies", GetActiveLobbies).RequirePermission(Permissions.MANAGE_ACTIVE_LOBBIES);
            group.MapGet("/lobby/{id}", GetActiveLobby).RequirePermission(Permissions.MANAGE_ACTIVE_LOBBIES);
            group.MapGet("/question-reports", GetQuestionReportsAsync).RequirePermission(Permissions.MANAGE_QUESTION_REPORTS);
            group.MapDelete("/question-report/{id}", DiscardQuestionReportAsync).RequirePermission(Permissions.MANAGE_QUESTION_REPORTS);
        }

        private static async Task<IResult> DiscardQuestionReportAsync(string id, MongoContext mongoContext)
        {
            if (MongoDB.Bson.ObjectId.TryParse(id, out var _))
                await mongoContext.QuestionReports.DeleteOneAsync(x => x.Id == id);

            return Results.Ok();
        }
        private static IResult GetActiveLobby(string id, ILobbyManager lobbyManager)
        {
            var lobby = lobbyManager.GetLobbyById(id);
            if (lobby == null)
                return Results.NotFound();

            return Results.Json(new
            {
                lobby = lobby.MapToDto(),
                gameSettings = lobby.LobbyGameSettings
            });
        }

        private static IResult GetActiveLobbies(ILobbyManager lobbyManager )
        {
            return Results.Json(lobbyManager.GetAllLobbies());
        }

        private static async Task<IResult> GetQuestionReportsAsync(MongoContext mongoContext)
        {
            var reports = await (await mongoContext.QuestionReports.FindAsync(Builders<QuestionReport>.Filter.Empty)).ToListAsync();
            var questions = await (await mongoContext.Questions.FindAsync(Builders<Question>.Filter.In(x => x.Id, reports.Select(x => x.QuestionId)))).ToListAsync();
            var categories = await (await mongoContext.Categories.FindAsync(Builders<Category>.Filter.In(x => x.Id, reports.Select(x => x.CategoryId)))).ToListAsync();

            return Results.Json(new
            {
                reports,
                questions,
                categories
            });
        }
    }
}
