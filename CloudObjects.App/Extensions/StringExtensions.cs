namespace CloudObjects.App.Extensions
{
    public static class StringExtensions
    {
        public static string MaxLength(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return null;

            return (input.Length > maxLength) ?
                input.Substring(0, maxLength) :
                input;
        }
    }
}
