using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApplication.ApiContracts.Models
{
    public class QuizResponse
    {
        public int Id { get; set; }
        public Quiz Quiz { get; set; }
        public Question Question { get; set; }
        public Answer Answer { get; set; }
        public int UserId { get; set; }
    }
}
