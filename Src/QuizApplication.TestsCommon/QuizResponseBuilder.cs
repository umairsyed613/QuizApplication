using System;
using System.Collections.Generic;
using System.Text;
using QuizApplication.ApiContracts.Models;

namespace QuizApplication.TestsCommon
{
    public class QuizResponseBuilder
    {
        public QuizResponse Entity { get; }

        public QuizResponseBuilder(QuizResponse response)
        {
            Entity = response;
        }

        public static QuizResponseBuilder New()
        {
            return new QuizResponseBuilder(new QuizResponse
            {
                Quiz = QuizBuilder.New().Entity,
                Question = QuestionBuilder.New().Entity,
                GivenAnswer = AnswerBuilder.New().Entity,
                AnswerPoint = 0
            });
        }

        public QuizResponseBuilder WithPoints(int points)
        {
            Entity.AnswerPoint = points;
            return this;
        }

    }
}
