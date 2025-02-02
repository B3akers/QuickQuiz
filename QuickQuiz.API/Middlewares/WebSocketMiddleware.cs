using Microsoft.Extensions.Hosting;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.WebSocketPipes;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Text;

namespace QuickQuiz.API.Middlewares
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CancellationTokenSource _applicationShutDownCts;
        private readonly IWebSocketConnectionManager _connectionManager;
        private readonly IWebSocketMessageHandler _messageHandler;

        public WebSocketMiddleware(RequestDelegate next, IWebSocketConnectionManager connectionManager,
            IWebSocketMessageHandler messageHandler, IHostApplicationLifetime hostApplicationLifetime)
        {
            _next = next;
            _connectionManager = connectionManager;
            _messageHandler = messageHandler;
            _applicationShutDownCts = new CancellationTokenSource();

            hostApplicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            _applicationShutDownCts.Cancel();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != "/ws")
            {
                await _next(context);
                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using (var ws = await context.WebSockets.AcceptWebSocketAsync())
            {
                string authorizationToken = null;

                if (context.Request.Query.TryGetValue("token", out var token))
                {
                    if (token.Count != 1)
                    {
                        try
                        {
                            await ws.CloseAsync((WebSocketCloseStatus)3401, "InvalidAuthToken", context.RequestAborted);
                        }
                        catch { }
                        return;
                    }
                    else
                    {
                        authorizationToken = string.IsNullOrEmpty(token[0]) ? string.Empty : token[0];
                    }
                }

                using var pipe = WebSocketPipe.Create(ws, true);
                var connectionContext = _connectionManager.CreateContext(authorizationToken, context, pipe);
                if (connectionContext == null)
                {
                    try
                    {
                        await pipe.CompleteAsync((WebSocketCloseStatus)3401, "InvalidAuthToken");
                    }
                    catch { }
                    return;
                }

                if (await _connectionManager.AddConnection(connectionContext))
                {
                    using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted, _applicationShutDownCts.Token))
                    {
                        try
                        {
                            await Task.WhenAll(
                                ProcessWebSocket(pipe, connectionContext),
                                pipe.RunAsync(linkedCts.Token)
                            );
                        }
                        catch
                        {

                        }
                        finally
                        {
                            _connectionManager.RemoveConnection(connectionContext);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await pipe.CompleteAsync(WebSocketCloseStatus.InternalServerError, "Couldn't establish connection");
                    }
                    catch { }
                }
            }
        }

        async Task ProcessWebSocket(IDuplexPipe pipe, WebSocketConnectionContext connectionContext)
        {
            while (await pipe.Input.ReadAsync() is var result && !result.IsCompleted)
            {
                string received = string.Empty;

                try
                {
                    received = Encoding.UTF8.GetString(result.Buffer);
                }
                catch
                {

                }
                finally
                {
                    pipe.Input.AdvanceTo(result.Buffer.End);
                }

                if (string.IsNullOrEmpty(received))
                    return;

                await _messageHandler.HandleMessagesAsync(received, connectionContext);
            }
        }
    }
}
