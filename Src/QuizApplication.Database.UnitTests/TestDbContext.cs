using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizApplication.Database.Models;
using Xunit;

namespace QuizApplication.Database.UnitTests
{
    public class TestDbContext
    {
        [Fact]
        public async Task TestMethod_UsingInMemoryProvider()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                         .UseInMemoryDatabase(databaseName: "Test1")
                         .Options;

            await using (var context = new QuizDbContext(options))
            {
                var quiz = new Quiz() { Title = "Test 1" };
                context.Quiz.Add(quiz);
                await context.SaveChangesAsync();
            }

            await using (var context = new QuizDbContext(options))
            {
                var q = Assert.Single(context.Quiz.ToList());
                Assert.NotNull(q);
                Assert.Equal("Test 1", q.Title);
            }
        }

        [Fact]
        public async Task TestMethod_UsingSqliteInMemoryProvider_Fail()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                         .UseSqlite("DataSource=:memory:")
                         .Options;

            await using (var context = new QuizDbContext(options)) { Assert.True(await context.Database.EnsureCreatedAsync()); }

            await using (var context = new QuizDbContext(options)) { Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(() => context.Quiz.Count()); }
        }

        [Fact]
        public void GetReadOnly_CoreDatabase_ReturnCoreContext()
        {
            var factory = new DbFactory<QuizDbContext>("DataSource=:memory:");
            using var context = factory.GetReadOnly();
            Assert.IsType<QuizDbContext>(context);
        }

        [Fact]
        public void GetReadOnly_CannotWriteToContext()
        {
            var factory = new DbFactory<QuizDbContext>("DataSource=:memory:");

            using (var context = factory.GetReadOnly())
                using (context.Database.BeginTransaction())
                {
                    var entity = new Quiz { Title = "Test Title" };
                    context.Quiz.Add(entity);
                    Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
                }
        }
    }
}
