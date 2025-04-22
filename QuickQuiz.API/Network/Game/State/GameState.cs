namespace QuickQuiz.API.Network.Game.State
{
    public enum GameStateId
    {
        None,
        CateogrySelection,
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
        public abstract Task OnActivate();
        public abstract Task OnUpdate();
    }
}
