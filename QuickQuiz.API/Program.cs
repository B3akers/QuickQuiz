using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using QuickQuiz.API;
using QuickQuiz.API.Database;
using QuickQuiz.API.Endpoints.Game;
using QuickQuiz.API.Endpoints.Moderator;
using QuickQuiz.API.Endpoints.User;
using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.Middlewares;
using QuickQuiz.API.Services;
using QuickQuiz.API.Services.WebSocket;
using QuickQuiz.API.Settings;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllAllowedOrigins",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.OperationFilter<SwaggerAuthOperationFilter>();
});

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<TwitchSettings>(builder.Configuration.GetSection("Twitch"));

builder.Services.AddSingleton<GameGlobalAsyncLock>();
builder.Services.AddSingleton<IJWTTokenProvider, JWTTokenProvider>();
builder.Services.AddSingleton<IUserProvider, UserProviderService>();
builder.Services.AddSingleton<ILobbyManager, LobbyManagerService>();
builder.Services.AddSingleton<IGameManager, GameManagerService>();
builder.Services.AddSingleton<IGameFlowManager, GameFlowManagerService>();
builder.Services.AddSingleton<IQuizProvider, QuizProviderService>();
builder.Services.AddSingleton<IConnectionTokenProvider, ConnectionTokenProvider>();
builder.Services.AddSingleton<IWebSocketMessageHandler, WebSocketMessageHandler>();
builder.Services.AddSingleton<IWebSocketConnectionManager, WebSocketConnectionManager>();

builder.Services.AddHostedService<GameTickService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

ValidatorOptions.Global.PropertyNameResolver = (type, memberInfo, expression) =>
{
    if (memberInfo != null)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(memberInfo.Name);
    }
    return null;
};

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers.TryGetValue("X-Real-IP", out var ip))
    {
        if (ip.Count > 1)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        ctx.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip[0]);
    }

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllAllowedOrigins");

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

    if (path.StartsWith("/v1"))
    {
        await next();
        return;
    }

    var try1 = Path.Combine(env.WebRootPath, path.TrimStart('/'));
    var try2 = try1 + ".html";
    var try3 = Path.Combine(env.WebRootPath, path.TrimStart('/'), "index.html");

    if (File.Exists(try1))
    {
        await next(); 
        return;
    }

    if (File.Exists(try2))
    {
        context.Request.Path = path + ".html";
        await next();
        return;
    }

    if (File.Exists(try3))
    {
        context.Request.Path = path + "/index.html";
        await next();
        return;
    }

    context.Response.StatusCode = StatusCodes.Status404NotFound;
    await context.Response.WriteAsync("404 Not Found");
});

// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
    KeepAliveTimeout = TimeSpan.FromMinutes(2)
});
app.UseMiddleware<WebSocketMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

app.MapGroup("/v1")
    .MapGameEndpoints()
    .MapUserEndpoints()
    .MapModeratorEndpoints();

app.Run();