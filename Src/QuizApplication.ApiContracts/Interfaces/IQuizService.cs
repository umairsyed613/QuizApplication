using System.Collections.Generic;
using System.Threading.Tasks;
using QuizApplication.ApiContracts.Models;

namespace QuizApplication.ApiContracts.Interfaces
{
    public interface IQuizReadService
    {
        // read part of service
        Task<IEnumerable<Quiz>> GetAllQuiz();
        Task<Quiz> GetQuizById(int id);
        Task<IEnumerable<QuizResponse>> GetQuizResponse(int quizId);
    }

    public interface IQuizWriteService
    {
        // CRUD Quiz
        Task CreateQuiz(Quiz quiz);
        Task UpdateQuiz(Quiz quiz);
        Task DeleteQuiz(int quizId);

        // CRUD Questions
        // TODO: create Question with Answer
        Task CreateQuestion(int quizId, Question question);
        Task CreateQuestions(int quizId, List<Question> questions);
        Task UpdateQuestion(Question question);
        Task DeleteQuestion(int questionId);

        // CRUD Answers

        Task CreateAnswer(int questionId, Answer answer);
        Task UpdateAnswer(Answer answer);
        Task DeleteAnswer(int answerId);

        // Quiz Response

        Task CreateQuizResponse(QuizResponse response);
    }
}
