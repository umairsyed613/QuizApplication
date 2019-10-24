using System;
using System.Threading.Tasks;
using QuizApplication.ApiContracts.Models;
using QuizApplication.Database;
using Xunit;
using static QuizApplication.TestsCommon.TransactionCreator;

namespace QuizApplication.Service.IntegrationTests
{
    public class ReadServiceTest
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
        public void Constructor_WithNullParams_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new QuizReadService(null));
        }

        [Fact]
        public void Constructor_DbConnFactoryIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new QuizWriteService(null));
        }

        [Fact]
        public async Task GetQuiz_InvalidQuizId_ThrowsInvalidOperationException()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var service = GetQuizReadService(dbTrans);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetQuizById(25));
        }

        [Fact]
        public async Task GetQuiz_validQuizId_ReturnsQuizSucessfully()
        {
            using var dbTrans = await NoCommitFactory().Create();
            var writeService = GetQuizWriteService(dbTrans);
            await writeService.CreateQuiz(new Quiz { Title = "Test Quiz" });
            var service = GetQuizReadService(dbTrans);
            var quiz = await service.GetQuizById(1);

            Assert.NotNull(quiz);
            Assert.Equal("Test Quiz", quiz.Title);
        }
    }
}
