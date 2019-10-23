using System.Collections.Generic;

namespace QuizApplication.ApiContracts.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IDictionary<string, string> Links { get; set; }
    }
}
