
using QuickQuiz.API.Interfaces;

namespace QuickQuiz.API.Services
{
    public class GameTickService : BackgroundService
    {
        private readonly ILogger<GameTickService> _logger;
        private readonly ILobbyManager _lobbyManager;
        private readonly IGameManager _gameManager;
        private readonly GameGlobalAsyncLock _globalGameLock;

        public GameTickService(ILogger<GameTickService> logger, ILobbyManager lobbyManager, IGameManager gameManager, GameGlobalAsyncLock globalGameLock)
        {
            _lobbyManager = lobbyManager;
            _gameManager = gameManager;
            _logger = logger;
            _globalGameLock = globalGameLock;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using (var writeLock = await _globalGameLock.WriterLockAsync())
                    {
                        await Task.WhenAll([_gameManager.OnUpdate(), _lobbyManager.OnUpdate()]);
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation($"Got stopping request, shutting down {nameof(GameTickService)}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured");
                }
            }
        }
    }
}
