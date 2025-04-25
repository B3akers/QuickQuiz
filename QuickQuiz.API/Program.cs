using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using QuickQuiz.API;
using QuickQuiz.API.Database;
using QuickQuiz.API.Endpoints.Game;
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllAllowedOrigins");

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
    KeepAliveTimeout = TimeSpan.FromMinutes(2)
});
app.UseMiddleware<WebSocketMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

app.MapGameEndpoints();
app.MapUserEndpoints();

app.Run();