using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.ApiContracts.Interfaces;
using QuizApplication.ApiContracts.Models;

namespace QuizApplication.RestApi.Controllers
{
    [Route("api/[controller]")]
    public class MyQuizController : Controller, IQuizReadService, IQuizWriteService
    {
        private readonly IQuizReadService _quizReadService;
        private readonly IQuizWriteService _quizWriteService; // TODO: Read Controller should be separate

        public MyQuizController(IQuizReadService quizReadService, IQuizWriteService quizWriteService)
        {
            _quizReadService = quizReadService ?? throw new ArgumentNullException(nameof(quizReadService));
            _quizWriteService = quizWriteService ?? throw new ArgumentNullException(nameof(quizWriteService));
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Quiz>> GetAllQuiz() => await _quizReadService.GetAllQuiz();

        [HttpGet("[action]/{id}")]
        public async Task<Quiz> GetQuizById(int id) => await _quizReadService.GetQuizById(id);

        [HttpPost("[action]")]
        public async Task CreateQuiz([FromBody] Quiz quiz) => await _quizWriteService.CreateQuiz(quiz);

        [HttpPost("[action]")]
        public async Task UpdateQuiz([FromBody] Quiz quiz) => await _quizWriteService.UpdateQuiz(quiz);

        [HttpPost("[action]/{quizId}")]
        public async Task DeleteQuiz(int quizId) => await _quizWriteService.DeleteQuiz(quizId);

        [HttpPost("[action]/{quizId}")]
        public async Task CreateQuestion(int quizId, [FromBody] Question question) => await _quizWriteService.CreateQuestion(quizId, question);

        public async Task CreateQuestions(int quizId, List<Question> questions) => throw new NotImplementedException();

        [HttpPost("[action]")]
        public async Task UpdateQuestion([FromBody] Question question) => await _quizWriteService.UpdateQuestion(question);

        [HttpPost("[action]/{questionId}")]
        public async Task DeleteQuestion(int questionId) => await _quizWriteService.DeleteQuestion(questionId);

        [HttpPost("[action]/{questionId}")]
        public async Task CreateAnswer(int questionId, [FromBody] Answer answer) => await _quizWriteService.CreateAnswer(questionId, answer);

        [HttpPost("[action]")]
        public async Task UpdateAnswer([FromBody] Answer answer) => await _quizWriteService.UpdateAnswer(answer);

        [HttpPost("[action]/{answerId}")]
        public async Task DeleteAnswer(int answerId) => await _quizWriteService.DeleteAnswer(answerId);

        [HttpPost("[action]")]
        public async Task CreateQuizResponse([FromBody] QuizResponse response) => await _quizWriteService.CreateQuizResponse(response);
    }
}
