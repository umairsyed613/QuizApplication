using System.Linq;
using apiModels = QuizApplication.ApiContracts.Models;
using dbModels = QuizApplication.Database.Models;

namespace QuizApplication.Service.Mappers
{
    public static class QuizMapper
    {
        public static apiModels.Quiz ToApiModel(dbModels.Quiz quiz)
        {
            return quiz == null ? null : new apiModels.Quiz { Id = quiz.Id, Title = quiz.Title, Questions = quiz.Question.Select(ToApiModel).ToList(), Links = null };
        }

        public static apiModels.Question ToApiModel(dbModels.Question question)
        {
            if (question == null) { return null; }

            var apiModelQuestion = new apiModels.Question { Id = question.Id, Text = question.Text };

            if (!question.Answer.Any()) { return apiModelQuestion; }

            apiModelQuestion.AvailableAnswers = question.Answer.Select(ToApiModel).ToList();
            apiModelQuestion.CorrectAnswer = question.CorrectAnswerId.HasValue ?
                ToApiModel(question.Answer.First(f => f.Id == question.CorrectAnswerId)) : null;

            return apiModelQuestion;
        }

        public static apiModels.Answer ToApiModel(dbModels.Answer answer)
        {
            return answer == null ? null : new apiModels.Answer { Id = answer.Id, Text = answer.Text };
        }
    }
}
