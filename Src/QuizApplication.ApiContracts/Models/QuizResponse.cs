namespace QuizApplication.ApiContracts.Models
{
    public class QuizResponse
    {
        public int Id { get; set; }
        public Quiz Quiz { get; set; }
        public Question Question { get; set; }
        public Answer GivenAnswer { get; set; }
        public int AnswerPoint { get; set; }
        public int UserId { get; set; }
    }
}
