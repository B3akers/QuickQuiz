namespace QuickQuiz.API.Network.Game
{
    public class GameSettings
    {
        public int CategoryCountInVote { get; set; } = 12;
        public int MaxCategoryVotesCount { get; set; } = 5;
        public int CategoryVoteTimeInSeconds { get; set; } = 15;
        public int QuestionCountPerRound { get; set; } = 5;
        public int QuestionAnswerTimeInSeconds { get; set; } = 15;
        public List<string> IncludeCategories { get; set; }
        public List<string> ExcludeCategories { get; set; }
        public bool CalculatePointsTimeFactor { get; set; } = true;
        public bool CalculatePointsDifficultyFactor { get; set; } = true;
    }
}
