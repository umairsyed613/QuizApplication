using System.Collections;
using System.Collections.Generic;
using QuizApplication.TestsCommon;

namespace QuizApplication.Service.IntegrationTests
{
    public class QuizResponsePointsCalculatorData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { QuizResponseBuilder.New().WithPoints(2), 2 };
            yield return new object[] { QuizResponseBuilder.New().WithPoints(5), 2 };
            yield return new object[] { QuizResponseBuilder.New().WithPoints(1), 2 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
