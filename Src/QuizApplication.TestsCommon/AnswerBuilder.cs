using QuizApplication.ApiContracts.Models;

namespace QuizApplication.TestsCommon
{
    public class AnswerBuilder
    {
        public Answer Entity { get; }

        public AnswerBuilder(Answer answer)
        {
            Entity = answer;
        }

        public static AnswerBuilder New()
        {
            return new AnswerBuilder(new Answer { Text = "This is Test Answer" });
        }

        public AnswerBuilder WithId(int id)
        {
            Entity.Id = id;
            return this;
        }

        public AnswerBuilder WithText(string text)
        {
            Entity.Text = text;
            return this;
        }
    }
}
