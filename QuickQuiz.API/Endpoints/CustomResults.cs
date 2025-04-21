using System.Text.Json;

namespace QuickQuiz.API.Endpoints
{
    public static class CustomResults
    {
        public static IResult InvalidProperty(string name, string message)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { { JsonNamingPolicy.CamelCase.ConvertName(name), [message] } });
        }

        public static IResult GenericError(string detail)
        {
            return Results.Json(new
            {
                Error = detail
            }, statusCode: 400);
        }
    }
}
