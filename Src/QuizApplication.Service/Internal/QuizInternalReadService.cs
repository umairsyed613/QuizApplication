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
            if (quiz == null) { throw new InvalidOperationException($"No Quiz Exits with Id {id}"); }

            return QuizMapper.ToApiModel(quiz);
        }

        public async Task<IEnumerable<apiModels.QuizResponse>> GetQuizResponseByQuizId(int quizId)
        {
            var res = (from response in _dbContext.QuizResponse
                join quiz in _dbContext.Quiz on response.QuizId equals quiz.Id
                join question in _dbContext.Question on response.QuestionId equals question.Id
                join answer in _dbContext.Answer on response.AnswerId equals answer.Id
                where response.QuizId == quizId
                select new { Quiz = quiz, Question = question, GivenAnswer = answer }).ToList();

            return res.Select(item => new apiModels.QuizResponse { Quiz = QuizMapper.ToApiModel(item.Quiz), Question = QuizMapper.ToApiModel(item.Question), GivenAnswer = QuizMapper.ToApiModel(item.GivenAnswer), AnswerPoint = item.Question.CorrectAnswerId.GetValueOrDefault(0) == item.GivenAnswer.Id ? 1 : 0 }).ToList();
        }
    }
}
