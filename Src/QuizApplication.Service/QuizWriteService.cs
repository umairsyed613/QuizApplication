using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuizApplication.ApiContracts.Interfaces;
using QuizApplication.ApiContracts.Models;
using QuizApplication.Database;
using QuizApplication.Service.Internal;

namespace QuizApplication.Service
{
    public class QuizWriteService : IQuizWriteService
    {
        private readonly IDbWithTransactionFactory _dbConnFactory;

        public QuizWriteService(IDbWithTransactionFactory dbConnFactory)
        {
            _dbConnFactory = dbConnFactory ?? throw new ArgumentNullException(nameof(dbConnFactory));
        }

        public async Task CreateQuiz(Quiz quiz)
        {
            await WrapReadWrite(service => service.CreateQuiz(quiz));
        }

        public async Task UpdateQuiz(Quiz quiz)
        {
            await WrapReadWrite(service => service.UpdateQuizById(quiz));
        }

        public async Task DeleteQuiz(int quizId)
        {
            await WrapReadWrite(service => service.DeleteQuizById(quizId));
        }

        public async Task CreateQuestion(int quizId, Question question)
        {
            await WrapReadWrite(service => service.CreateQuestion(quizId, question));
        }

        // TODO Create in Batch
        public async Task CreateQuestions(int quizId, List<Question> questions) => throw new NotImplementedException();

        public async Task UpdateQuestion(Question question)
        {
            await WrapReadWrite(service => service.UpdateQuestion(question));
        }

        public async Task DeleteQuestion(int questionId)
        {
            await WrapReadWrite(service => service.DeleteQuestion(questionId));
        }

        public async Task CreateAnswer(int questionId, Answer answer)
        {
            await WrapReadWrite(service => service.CreateAnswer(questionId, answer));
        }

        public async Task UpdateAnswer(Answer answer) => await WrapReadWrite(service => service.UpdateAnswer(answer));

        public async Task DeleteAnswer(int answerId) => await WrapReadWrite(service => service.DeleteAnswer(answerId));

        public async Task CreateQuizResponse(QuizResponse response) => await WrapReadWrite(service => service.CreateQuizResponse(response));

        private async Task WrapReadWrite(Func<QuizInternalWriteService, Task> function)
        {
            await WrapReadWriteWithTransactionContext(async c =>
            {
                await function(c);
                return false;
            });
        }

        private async Task<T> WrapReadWriteWithTransactionContext<T>(Func<QuizInternalWriteService, Task<T>> function)
        {
            using (var transaction = await _dbConnFactory.Create())
            {
                var hitmanCoreContextFactory = transaction.FactoryFor<QuizDbContext>();

                using (var dbContext = hitmanCoreContextFactory.GetReadWrite())
                {
                    var service = new QuizInternalWriteService(dbContext);
                    var result = await function(service);
                    transaction.Commit();
                    return result;
                }
            }
        }
    }
}
