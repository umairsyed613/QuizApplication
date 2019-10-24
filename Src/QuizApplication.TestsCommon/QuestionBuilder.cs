using System;
using System.Collections.Generic;
using System.Linq;
using QuizApplication.ApiContracts.Models;

namespace QuizApplication.TestsCommon
{
    public class QuestionBuilder
    {
        public Question Entity { get; }

        public QuestionBuilder(Question question)
        {
            Entity = question;
        }

        public static QuestionBuilder New()
        {
            return new QuestionBuilder(new Question { Text = "This is Test Question" });
        }

        public QuestionBuilder WithId(int id)
        {
            Entity.Id = id;
            return this;
        }

        public QuestionBuilder WithText(string text)
        {
            Entity.Text = text;
            return this;
        }

        public QuestionBuilder WithAnswer(Answer answer)
        {
            Entity.AvailableAnswers = new List<Answer> { answer };
            return this;
        }

        public QuestionBuilder WithAnswers(IEnumerable<Answer> answers)
        {
            Entity.AvailableAnswers = answers;
            return this;
        }

        public QuestionBuilder WithCorrectAnswer(Answer answer)
        {
            Entity.CorrectAnswer = answer;
            return this;
        }

        public QuestionBuilder WithCorrectAnswerId(int answerId)
        {
            Entity.CorrectAnswer = Entity.AvailableAnswers.First(f => f.Id == answerId) ?? throw new InvalidOperationException("Wrong Correct Answer");
            return this;
        }
    }
}
