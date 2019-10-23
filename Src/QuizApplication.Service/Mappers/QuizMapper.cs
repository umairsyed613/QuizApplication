using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using apiModels = QuizApplication.ApiContracts.Models;
using dbModels = QuizApplication.Database.Models;

namespace QuizApplication.Service.Mappers
{
    public static class QuizMapper
    {
        public static apiModels.Quiz ToApiModel(dbModels.Quiz quiz)
        {
            return quiz == null ? null : new apiModels.Quiz { Id = quiz.Id, Title = quiz.Title, Questions = quiz.Question.Select(ToApiModel).ToList(), Links = null };
        }

        public static apiModels.Question ToApiModel(dbModels.Question question)
        {
            return question == null ? null : new apiModels.Question
            {
                Id = question.Id,
                AvailableAnswers = question.Answer.Select(ToApiModel).ToList(),
                CorrectAnswer = ToApiModel(question.Answer.First(f => question.CorrectAnswerId.HasValue && f.Id == question.CorrectAnswerId)),
                Text = question.Text
            };
        }

        public static apiModels.Answer ToApiModel(dbModels.Answer answer)
        {
            return answer == null ? null : new apiModels.Answer { Id = answer.Id, Text = answer.Text };
        }
    }
}
