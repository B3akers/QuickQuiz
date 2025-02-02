namespace QuickQuiz.API.Identities
{
    public class UnauthenticatedOnlyFilter : IEndpointFilter
    {
        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (context.HttpContext != null
                && context.HttpContext.User != null
                && context.HttpContext.User.Identity != null
                && context.HttpContext.User.Identity.IsAuthenticated)
            {
                return Results.BadRequest();
            }

            return await next(context);
        }
    }
}
