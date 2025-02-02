using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Filters
{
    public class AuthenticationFilter : IEndpointFilter
    {
        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (context.HttpContext.User != null
                && context.HttpContext.User.Identity is ApplicationIdentityJWT)
            {
                return await next(context);
            }

            return Results.Unauthorized();
        }
    }
}
