using System;
using System.Collections.Generic;

namespace QuizApplication.Database.Models
{
    public partial class Question
    {
        public Question()
        {
            Answer = new HashSet<Answer>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int QuizId { get; set; }
        public int? CorrectAnswerId { get; set; }

        public virtual Answer CorrectAnswer { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<Answer> Answer { get; set; }
    }
}
