using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuizApplication.ApiContracts.Interfaces;
using QuizApplication.ApiContracts.Models;
using QuizApplication.Database;
using QuizApplication.Service.Internal;

namespace QuizApplication.Service
{
    public class QuizReadService : IQuizReadService
    {
        private readonly IDbFactory<QuizDbContext> _dbFactory;

        public QuizReadService(IDbFactory<QuizDbContext> dbFactory)
        {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        }

        public async Task<IEnumerable<Quiz>> GetAllQuiz()
        {
            return await WrapReadOnly(service => service.GetAllQuiz());
        }

        public async Task<Quiz> GetQuizById(int id)
        {
            return await WrapReadOnly(service => service.GetQuizById(id));
        }

        public async Task<IEnumerable<QuizResponse>> GetQuizResponse(int quizId) => await WrapReadOnly(service => service.GetQuizResponseByQuizId(quizId));

        private async Task<T> WrapReadOnly<T>(Func<QuizInternalReadService, Task<T>> function)
        {
            using (var dbContext = _dbFactory.GetReadOnly())
            {
                var service = new QuizInternalReadService(dbContext);
                return await function(service);
            }
        }
    }
}
