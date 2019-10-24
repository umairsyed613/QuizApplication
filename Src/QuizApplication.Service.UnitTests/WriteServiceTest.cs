using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuizApplication.ApiContracts.Models;
using QuizApplication.Database;
using QuizApplication.TestsCommon;
using Xunit;
using static QuizApplication.TestsCommon.TransactionCreator;

namespace QuizApplication.Service.IntegrationTests
{
    public class WriteServiceTest
    {
        private static QuizReadService GetQuizReadService(IDbWithTransactionConnection dbTrans)
        {
            return new QuizReadService(dbTrans.FactoryFor<QuizDbContext>());
        }

        private static QuizWriteService GetQuizWriteService(IDbWithTransactionConnection dbTrans)
        {
            return new QuizWriteService(dbTrans.Clone());
        }

        [Fact]
        public async Task WriteService_CreateQuizWithQuestion_CreatedSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var writeService = GetQuizWriteService(dbTrans);
            var quizBuilder = QuizBuilder.New().WithTitle("Test Quiz");
            await writeService.CreateQuiz(quizBuilder.Entity);
            var service = GetQuizReadService(dbTrans);
            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            var questionBuilder1 = QuestionBuilder.New().WithText("Question 1");
            var questionBuilder2 = QuestionBuilder.New().WithText("Question 2");
            var questionBuilder3 = QuestionBuilder.New().WithText("Question 3");
            var questionBuilder4 = QuestionBuilder.New().WithText("Question 4");

            await writeService.CreateQuestion(quiz.Id, questionBuilder1.Entity);
            await writeService.CreateQuestion(quiz.Id, questionBuilder2.Entity);
            await writeService.CreateQuestion(quiz.Id, questionBuilder3.Entity);
            await writeService.CreateQuestion(quiz.Id, questionBuilder4.Entity);

            quiz = await service.GetQuizById(quiz.Id);

            Assert.InRange(quiz.Questions.ToList().Count, 4, 4);
        }

        [Fact]
        public async Task WriteService_CreateQuizWithQuestionsAndAnswers_CreatedSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var writeService = GetQuizWriteService(dbTrans);
            var quizBuilder = QuizBuilder.New().WithTitle("Test Quiz");
            await writeService.CreateQuiz(quizBuilder.Entity);
            var service = GetQuizReadService(dbTrans);
            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            var questionBuilder1 = QuestionBuilder.New().WithText("Question 1").WithAnswers(new List<Answer>
            {
                AnswerBuilder.New().WithText("Q1: Answer 1").Entity,
                AnswerBuilder.New().WithText("Q1: Answer 2").Entity,
                AnswerBuilder.New().WithText("Q1: Answer 3").Entity
            });

            var questionBuilder2 = QuestionBuilder.New().WithText("Question 2").WithAnswers(new List<Answer>
            {
                AnswerBuilder.New().WithText("Q2: Answer 1").Entity,
                AnswerBuilder.New().WithText("Q2: Answer 2").Entity,
                AnswerBuilder.New().WithText("Q2: Answer 3").Entity
            });

            await writeService.CreateQuestion(quiz.Id, questionBuilder1.Entity);
            await writeService.CreateQuestion(quiz.Id, questionBuilder2.Entity);

            quiz = await service.GetQuizById(quiz.Id);

            Assert.InRange(quiz.Questions.ToList().Count, 2, 2);

            foreach (var question in quiz.Questions) { Assert.InRange(question.AvailableAnswers.Count(), 3, 3); }
        }

        [Fact]
        public async Task WriteService_CreateQuizWithQuestionsAndAnswersAndCorrectAnswer_CreatedSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var service = GetQuizReadService(dbTrans);
            var writeService = GetQuizWriteService(dbTrans);
            var questionBuilder1 = QuestionBuilder.New().WithText("Question 1").WithAnswers(new List<Answer>
            {
                AnswerBuilder.New().WithText("Q1: Answer 1").Entity,
                AnswerBuilder.New().WithText("Q1: Answer 2").Entity,
                AnswerBuilder.New().WithText("Q1: Answer 3").Entity
            });

            var questionBuilder2 = QuestionBuilder.New().WithText("Question 2").WithAnswers(new List<Answer>
            {
                AnswerBuilder.New().WithText("Q2: Answer 1").Entity,
                AnswerBuilder.New().WithText("Q2: Answer 2").Entity,
                AnswerBuilder.New().WithText("Q2: Answer 3").Entity
            });

            var quizBuilder = QuizBuilder.New().WithTitle("Test Quiz").WithQuestions(new List<Question> { questionBuilder1.Entity, questionBuilder2.Entity });
            await writeService.CreateQuiz(quizBuilder.Entity);
            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            Assert.InRange(quiz.Questions.ToList().Count, 2, 2);

            foreach (var question in quiz.Questions)
            {
                Assert.Null(question.CorrectAnswer);
                Assert.InRange(question.AvailableAnswers.Count(), 3, 3);
            }

            quiz = await service.GetQuizById(quiz.Id);
            Assert.NotNull(quiz);
            var q = quiz.Questions.First();
            q.CorrectAnswer = q.AvailableAnswers.First();
            await writeService.UpdateQuestion(q);
            Assert.Single(quiz.Questions.Where(a => a.CorrectAnswer != null).ToList());
        }

        [Fact]
        public async Task WriteService_DeleteQuiz_DeleteSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var service = GetQuizReadService(dbTrans);
            var writeService = GetQuizWriteService(dbTrans);

            await writeService.CreateQuiz(QuizBuilder.New().WithTitle("Test Quiz").Entity);

            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            await writeService.DeleteQuiz(quiz.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetQuizById(quiz.Id));

            var allQuiz = await service.GetAllQuiz();
            Assert.Empty(allQuiz);
        }

        [Fact]
        public async Task WriteService_DeleteQuizWithQuestions_DeleteSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var service = GetQuizReadService(dbTrans);
            var writeService = GetQuizWriteService(dbTrans);

            await writeService.CreateQuiz(QuizBuilder.New().WithTitle("Test Quiz").WithQuestion(QuestionBuilder.New().WithText("Question 1").Entity).Entity);

            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            Assert.NotEmpty(quiz.Questions);

            await writeService.DeleteQuiz(quiz.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetQuizById(quiz.Id));

            Assert.Empty(dbTrans.FactoryFor<QuizDbContext>().GetReadOnly().Question.ToList());
            var allQuiz = await service.GetAllQuiz();
            Assert.Empty(allQuiz);
        }

        [Fact]
        public async Task WriteService_DeleteQuizQuestion_DeleteSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var service = GetQuizReadService(dbTrans);
            var writeService = GetQuizWriteService(dbTrans);

            await writeService.CreateQuiz(QuizBuilder.New().WithTitle("Test Quiz").WithQuestion(QuestionBuilder.New().WithText("Question 1").Entity).Entity);

            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            Assert.NotEmpty(quiz.Questions);
            var question = quiz.Questions.First();
            await writeService.DeleteQuestion(question.Id);

            quiz = await service.GetQuizById(quiz.Id);
            Assert.NotNull(quiz);
            Assert.Empty(quiz.Questions);
        }

        [Fact]
        public async Task WriteService_DeleteAnswer_DeleteSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var service = GetQuizReadService(dbTrans);
            var writeService = GetQuizWriteService(dbTrans);

            await writeService.CreateQuiz(QuizBuilder.New().WithTitle("Test Quiz")
                                                     .WithQuestion(QuestionBuilder.New().WithText("Question 1").WithAnswer(AnswerBuilder.New().WithText("Test Answer 1").Entity)
                                                                                  .Entity).Entity);

            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            Assert.NotEmpty(quiz.Questions);
            var question = quiz.Questions.First();
            await writeService.DeleteAnswer(question.AvailableAnswers.First().Id);

            quiz = await service.GetQuizById(quiz.Id);
            Assert.NotNull(quiz);
            Assert.NotEmpty(quiz.Questions);
            Assert.Null(quiz.Questions.First().AvailableAnswers);
        }

        [Fact]
        public async Task WriteService_CreateQuizResponse_CreatedSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var service = GetQuizReadService(dbTrans);
            var writeService = GetQuizWriteService(dbTrans);
            var questionBuilder1 = QuestionBuilder.New().WithText("Question 1").WithAnswers(new List<Answer>
            {
                AnswerBuilder.New().WithText("Q1: Answer 1").Entity,
                AnswerBuilder.New().WithText("Q1: Answer 2").Entity,
                AnswerBuilder.New().WithText("Q1: Answer 3").Entity
            });

            var questionBuilder2 = QuestionBuilder.New().WithText("Question 2").WithAnswers(new List<Answer>
            {
                AnswerBuilder.New().WithText("Q2: Answer 1").Entity,
                AnswerBuilder.New().WithText("Q2: Answer 2").Entity,
                AnswerBuilder.New().WithText("Q2: Answer 3").Entity
            });

            var quizBuilder = QuizBuilder.New().WithTitle("Test Quiz").WithQuestions(new List<Question> { questionBuilder1.Entity, questionBuilder2.Entity });
            await writeService.CreateQuiz(quizBuilder.Entity);
            var quiz = Assert.Single(await service.GetAllQuiz());

            Assert.NotNull(quiz);

            Assert.InRange(quiz.Questions.ToList().Count, 2, 2);

            foreach (var question in quiz.Questions)
            {
                Assert.Null(question.CorrectAnswer);
                Assert.InRange(question.AvailableAnswers.Count(), 3, 3);
            }

            quiz = await service.GetQuizById(quiz.Id);
            Assert.NotNull(quiz);
            var q = quiz.Questions.First();
            q.CorrectAnswer = q.AvailableAnswers.First();
            await writeService.UpdateQuestion(q);
            Assert.Single(quiz.Questions.Where(a => a.CorrectAnswer != null).ToList());

            var q2 = quiz.Questions.Last();
            q2.CorrectAnswer = q2.AvailableAnswers.Last();
            await writeService.UpdateQuestion(q2);

            quiz = await service.GetQuizById(quiz.Id);

            Assert.InRange(quiz.Questions.Where(a => a.CorrectAnswer != null).ToList().Count, 2, 2);


            // Quiz is ready now

            await writeService.CreateQuizResponse(new QuizResponse
            {
                Quiz = quiz, Question = quiz.Questions.First(), GivenAnswer = quiz.Questions.First().AvailableAnswers.Last() // wrong answer
            });

            await writeService.CreateQuizResponse(new QuizResponse
            {
                Quiz = quiz, Question = quiz.Questions.Last(), GivenAnswer = quiz.Questions.Last().AvailableAnswers.Last() // correct answer
            });

            var response = await service.GetQuizResponse(quiz.Id);
            Assert.NotEmpty(response);

            Assert.InRange(response.Count(), 2, 2);

            Assert.Equal(1, response.Sum(s => s.AnswerPoint));
        }
    }
}
