using QuickQuiz.API.Database.Structures;

namespace QuickQuiz.API.Dto
{
    public class GameCategoryVoteDto
    {
        public List<Category> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public int CategoryIndex { get; set; }
        public int MaxCategoryIndex { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
