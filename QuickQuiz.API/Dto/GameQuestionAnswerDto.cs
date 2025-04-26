namespace QuickQuiz.API.Dto
{
    public class GameQuestionAnswerDto
    {
        public List<string>[] PlayerAnswers { get; set; }
        public int CorrectAnswerId { get; set; }
    }
}
