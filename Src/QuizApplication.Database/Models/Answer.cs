using System;
using System.Collections.Generic;

namespace QuizApplication.Database.Models
{
    public partial class Answer
    {
        public Answer()
        {
            QuestionNavigation = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
        public virtual ICollection<Question> QuestionNavigation { get; set; }
    }
}
