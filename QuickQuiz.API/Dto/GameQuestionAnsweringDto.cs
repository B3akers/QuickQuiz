namespace QuickQuiz.API.Dto
{
    public class GameQuestionAnsweringDto
    {
        public GameQuestionDto Question { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
