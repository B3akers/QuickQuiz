using MongoDB.Driver;
using QuickQuiz.API.Database;
using QuickQuiz.API.Database.Structures;
using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Filters
{
    public class PermissionFilter : IEndpointFilter
    {
        private readonly string _requiredPermission;
        public PermissionFilter(string requiredPermission)
        {
            _requiredPermission = requiredPermission;
        }

        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (context.HttpContext.User != null
                && context.HttpContext.User.Identity is ApplicationIdentityJWT jwt)
            {
                var mongoContext = context.HttpContext.RequestServices.GetRequiredService<MongoContext>();
                var hasPermission = await mongoContext.Permissions.Find(Builders<Permission>.Filter.And(
                                                        Builders<Permission>.Filter.Eq(p => p.UserId, jwt.Id),
                                                        Builders<Permission>.Filter.AnyEq(p => p.Permissions, _requiredPermission)
                                                    )).AnyAsync();

                if (!hasPermission)
                {
                    return Results.StatusCode(403);
                }

                return await next(context);
            }

            return Results.Unauthorized();
        }
    }
}
