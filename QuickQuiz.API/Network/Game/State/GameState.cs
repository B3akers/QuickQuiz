using QuickQuiz.API.Dto;
using QuickQuiz.API.WebSockets.Packets;

namespace QuickQuiz.API.Network.Game.State
{
    public enum GameStateId
    {
        None,
        CategorySelection,
        PrepareForQuestion,
        QuestionAnswering,
        QuestionAnswered,
        RoundEnd,
        Terminate
    };

    public abstract class GameState
    {
        public GameInstance Game { get; set; }

        public abstract GameStateId Id { get; }
        public async Task OnActivateAsync()
        {
            //Game start
            //
            if (Game.State == null)
            {
                await Game.Players.SendToAllPlayersAsync(new GamePlayersResponsePacket()
                {
                    Players = Game.Players.ToPlayersDto(),
                });
            }

            await OnActivateCoreAsync();

            Game.LastStateSwitch = DateTimeOffset.UtcNow;
            Game.State = this;
        }

        protected abstract Task OnActivateCoreAsync();
        public abstract Task OnUpdateAsync();
    }
}
