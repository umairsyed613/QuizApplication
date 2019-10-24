using System;
using System.Collections.Generic;
using QuizApplication.ApiContracts.Models;

namespace QuizApplication.TestsCommon
{
    public class QuizBuilder
    {
        public Quiz Entity { get; }

        public QuizBuilder(Quiz quiz)
        {
            Entity = quiz;
        }

        public static QuizBuilder New()
        {
            return new QuizBuilder(new Quiz { Title = "Test Quiz" });
        }

        public QuizBuilder WithId(int id)
        {
            Entity.Id = id;
            return this;
        }

        public QuizBuilder WithTitle(string text)
        {
            Entity.Title = text;
            return this;
        }

        public QuizBuilder WithQuestion(Question question)
        {
            Entity.Questions = new List<Question> { question };
            return this;
        }

        public QuizBuilder WithQuestions(IEnumerable<Question> questions)
        {
            Entity.Questions = questions;
            return this;
        }
    }
}
