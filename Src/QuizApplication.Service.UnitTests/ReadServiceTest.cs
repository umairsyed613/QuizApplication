using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizApplication.Database;
using QuizApplication.Database.Models;
using Xunit;

namespace QuizApplication.Service.UnitTests
{
    public class ReadServiceTest
    {
        [Fact]
        public async Task TestMethod_UsingInMemoryProvider()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                         .UseInMemoryDatabase(databaseName: "Test1")
                         .Options;

            using (var context = new QuizDbContext(options))
            {
                var quiz = new Quiz() { Title = "Test 1" };
                context.Quiz.Add(quiz);
                await context.SaveChangesAsync();
            }

            // New context with the data as the database name is the same
            using (var context = new QuizDbContext(options))
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

            await using (var context = new QuizDbContext(options))
            {
                Assert.True(await context.Database.EnsureCreatedAsync());
            }

            await using (var context = new QuizDbContext(options))
            {
                Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(() => context.Quiz.Count());
            }
        }
    }
}
