namespace QuizApplication.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TrimToNull(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return null; }

            var trimmed = input.Trim();
            return trimmed.Length == 0 ? null : trimmed;
        }
    }
}
