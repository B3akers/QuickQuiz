using System.Text.Json.Serialization;

namespace QuickQuiz.API.Dto
{
    public class GamePrepareForQuestionDto
    {
        public Database.Structures.Category Category { get; set; }
        public string PreloadImage { get; set; }
        public int QuestionCount { get; set; }
        public int QuestionIndex { get; set; }
    }
}
