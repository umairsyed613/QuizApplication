using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using apiModels = QuizApplication.ApiContracts.Models;
using QuizApplication.Common.Extensions;
using QuizApplication.Database;
using QuizApplication.Database.Models;

namespace QuizApplication.Service.Internal
{
    public class QuizInternalWriteService
    {
        private readonly QuizDbContext _dbContext;

        public QuizInternalWriteService(QuizDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task CreateQuiz(apiModels.Quiz quiz)
        {
            if (quiz == null) { throw new ArgumentNullException(nameof(quiz)); }

            var title = quiz.Title.TrimToNull();
            if (title == null) { throw new InvalidOperationException("Quiz With Empty Title cannot be created!"); }

            var dbQuiz = new Quiz { Title = title };

            _dbContext.Quiz.Add(dbQuiz);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateQuizById(apiModels.Quiz quiz)
        {
            if (quiz == null) { throw new ArgumentNullException(nameof(quiz)); }

            var title = quiz.Title.TrimToNull();
            if (title == null) { throw new InvalidOperationException("Quiz With Empty Title cannot be updated!"); }

            var dbQuiz = await _dbContext.Quiz.FirstOrDefaultAsync(q => q.Id == quiz.Id) ?? throw new InvalidOperationException("Wrong Quiz Id Provided.");

            dbQuiz.Title = title;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteQuizById(int quizId)
        {
            var dbQuiz = await _dbContext.Quiz.FirstOrDefaultAsync(q => q.Id == quizId) ?? throw new InvalidOperationException("Wrong Quiz Id Provided.");
            _dbContext.Quiz.Remove(dbQuiz);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateQuestion(int quizId, apiModels.Question question)
        {
            if (question == null) { throw new ArgumentNullException(nameof(question)); }

            var questionText = question.Text.TrimToNull();

            if (questionText == null) { throw new InvalidOperationException("Question Cannot be created with Empty Text!"); }

            var dbQuiz = await _dbContext.Quiz.FirstOrDefaultAsync(q => q.Id == quizId) ?? throw new InvalidOperationException("Wrong Quiz Id Provided.");

            var dbQuestion = new Question { Text = questionText, QuizId = dbQuiz.Id, CorrectAnswerId = question.CorrectAnswer?.Id };

            _dbContext.Question.Add(dbQuestion);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateQuestion(apiModels.Question question)
        {
            if (question == null) { throw new ArgumentNullException(nameof(question)); }

            var text = question.Text.TrimToNull();
            if (text == null) { throw new InvalidOperationException("Question With Empty Text cannot be updated!"); }

            var dbQuestion = await _dbContext.Question.FirstOrDefaultAsync(q => q.Id == question.Id) ?? throw new InvalidOperationException("Wrong Question Id Provided.");

            dbQuestion.Text = text;
            dbQuestion.CorrectAnswerId = question.CorrectAnswer?.Id;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteQuestion(int questionId)
        {
            var dbQuestion = await _dbContext.Question.FirstOrDefaultAsync(q => q.Id == questionId) ?? throw new InvalidOperationException("Wrong Question Id Provided.");
            _dbContext.Question.Remove(dbQuestion);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAnswer(int questionId, apiModels.Answer answer)
        {
            if (answer == null) { throw new ArgumentNullException(nameof(answer)); }

            var text = answer.Text.TrimToNull();
            if (text == null) { throw new InvalidOperationException("Answer With Empty Text cannot be created!"); }

            var dbQuestion = await _dbContext.Question.FirstOrDefaultAsync(q => q.Id == questionId) ?? throw new InvalidOperationException("Wrong Question Id Provided.");

            var dbModel = new Answer { Text = text, QuestionId = dbQuestion.Id };

            _dbContext.Answer.Add(dbModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAnswer(apiModels.Answer answer)
        {
            if (answer == null) { throw new ArgumentNullException(nameof(answer)); }

            var text = answer.Text.TrimToNull();
            if (text == null) { throw new InvalidOperationException("Answer With Empty Text cannot be updated!"); }

            var dbModel = await _dbContext.Answer.FirstOrDefaultAsync(q => q.Id == answer.Id) ?? throw new InvalidOperationException("Wrong Answer Id Provided.");

            dbModel.Text = text;
            _dbContext.Answer.Update(dbModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAnswer(int answerId)
        {
            var dbModel = await _dbContext.Answer.FirstOrDefaultAsync(q => q.Id == answerId) ?? throw new InvalidOperationException("Wrong Answer Id Provided.");
            _dbContext.Answer.Remove(dbModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateQuizResponse(apiModels.QuizResponse response)
        {
            if (response == null) { throw new ArgumentNullException(nameof(response)); }

            if (response.Quiz == null || response.Answer == null || response.Question == null)
            {
                throw new InvalidOperationException("Cannot create response with missing properties");
            }

            var dbModel = new QuizResponse { QuizId = response.Quiz.Id, QuestionId = response.Question.Id, AnswerId = response.Answer.Id, UserId = response.UserId };

            _dbContext.QuizResponse.Add(dbModel);
            await _dbContext.SaveChangesAsync();
        }
    }
}
