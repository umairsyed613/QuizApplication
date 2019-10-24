using System.Collections.Generic;

namespace QuizApplication.ApiContracts.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IEnumerable<Answer> AvailableAnswers { get; set; }
        public Answer CorrectAnswer { get; set; }
    }
}
