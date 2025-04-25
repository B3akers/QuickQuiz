namespace QuickQuiz.API.Dto
{
    public class GameQuestionDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public List<string> Answers { get; set; }

        public static GameQuestionDto Map(Database.Structures.Question question)
        {
            return new GameQuestionDto()
            {
                Id = question.Id,
                Text = question.Text,
                Image = question.Image,
                Answers = question.Answers,
            };
        }
    }
}
