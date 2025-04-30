
using QuickQuiz.API.WebSockets.Packets;

namespace QuickQuiz.API.Network.Game.State
{
    public class GameStateTerminate : GameState
    {
        public override GameStateId Id => GameStateId.Terminate;
        protected override async Task OnActivateCoreAsync()
        {
            var points = new List<KeyValuePair<string, double>>(Game.Players.Count);
            foreach (var player in Game.Players)
                points.Add(new KeyValuePair<string, double>(player.Key, player.Value.Points));

            await Game.Players.SendToAllPlayersAsync(new GameFinishedResponsePacket()
            {
                PlayerPoints = points
            });
        }

        public override Task OnUpdateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
