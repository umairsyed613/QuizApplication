using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizApplication.Database;
using QuizApplication.Service.Mappers;
using apiModels = QuizApplication.ApiContracts.Models;

namespace QuizApplication.Service.Internal
{
    public class QuizInternalReadService
    {
        private readonly QuizDbContext _dbContext;

        public QuizInternalReadService(QuizDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<apiModels.Quiz>> GetAllQuiz()
        {
            return _dbContext.Quiz.Include(i => i.Question).ThenInclude(ti => ti.Answer).Select(QuizMapper.ToApiModel).ToList();
        }

        public async Task<apiModels.Quiz> GetQuizById(int id)
        {
            var quiz = await _dbContext.Quiz.Include(i => i.Question).ThenInclude(ti => ti.Answer).FirstOrDefaultAsync(w => w.Id == id);
            return QuizMapper.ToApiModel(quiz);
        }
    }
}
