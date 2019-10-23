using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.Database;

namespace QuizApplication.RestApi.Controllers
{
    [Route("api/values")]
    public class ValuesController : Controller
    {
        private IDbWithTransactionFactory _factory;

        public ValuesController(IDbWithTransactionFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            using var fac = await _factory.Create();
            var con = fac.FactoryFor<QuizDbContext>();
            var context = con.GetReadOnly();
            return context.Quiz.First().Title;
        }
    }
}