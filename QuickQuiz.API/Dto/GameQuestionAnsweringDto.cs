namespace QuickQuiz.API.Dto
{
    public class GameQuestionAnsweringDto
    {
        public GameQuestionDto Question { get; set; }
        public List<string>[] PlayerAnswers { get; set; }
        public int? CorrectAnswerId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
