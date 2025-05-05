using QuickQuiz.API.Filters;
using QuickQuiz.API.Identities;

namespace QuickQuiz.API.Extensions
{
    public static class RouteExtensions
    {
        public static RouteGroupBuilder RequireAuthentication(this RouteGroupBuilder builder)
        {
            return builder.AddEndpointFilter<AuthenticationFilter>();
        }
        public static RouteGroupBuilder RequireUnauthenticatedOnly(this RouteGroupBuilder builder)
        {
            return builder.AddEndpointFilter<UnauthenticatedOnlyFilter>();
        }
        public static RouteGroupBuilder RequirePermission(this RouteGroupBuilder builder, string permissions)
        {
            return builder.AddEndpointFilter(new PermissionFilter(permissions));
        }
        public static RouteHandlerBuilder RequireAuthentication(this RouteHandlerBuilder builder)
        {
            return builder.AddEndpointFilter<AuthenticationFilter>();
        }
        public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string permissions)
        {
            return builder.AddEndpointFilter(new PermissionFilter(permissions));
        }
        public static RouteHandlerBuilder RequireUnauthenticatedOnly(this RouteHandlerBuilder builder)
        {
            return builder.AddEndpointFilter<UnauthenticatedOnlyFilter>();
        }
    }
}
