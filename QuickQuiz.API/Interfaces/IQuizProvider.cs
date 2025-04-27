using QuickQuiz.API.Database.Structures;

namespace QuickQuiz.API.Interfaces
{
    public interface IQuizProvider
    {
        Task<List<Category>> GetCategoriesAsync(IEnumerable<string> include, IEnumerable<string> skip);
        Task<List<Category>> GetRandomCategoriesAsync(int number, int minimumQuestionCount, IEnumerable<string> skip);
        Task<List<Question>> GetRandomQuestionsFromCategoryAsync(string categoryId, int number, IEnumerable<string> skip);
        Task IncreasePopularity(string categoryId, long count);
    }
}
