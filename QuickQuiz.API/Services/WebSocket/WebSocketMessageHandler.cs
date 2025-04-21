using QuickQuiz.API.Interfaces;
using QuickQuiz.API.Interfaces.WebSocket;
using QuickQuiz.API.WebSockets;
using QuickQuiz.API.WebSockets.Data;
using System.Text.Json;

namespace QuickQuiz.API.Services.WebSocket
{
    public class WebSocketMessageHandler : IWebSocketMessageHandler
    {
        private readonly IGameFlowManager _gameFlowManager;
        public WebSocketMessageHandler(IGameFlowManager gameFlowManager)
        {
            _gameFlowManager = gameFlowManager;
        }

        public async Task HandleMessagesAsync(string message, WebSocketConnectionContext context)
        {
            try
            {
                var packet = JsonSerializer.Deserialize<BasePacketRequest>(message, IWebSocketConnectionManager.JsonJavascriptOptions);
                await _gameFlowManager.ProcessPacket(context, packet);
            }
            catch { }
        }
    }
}
