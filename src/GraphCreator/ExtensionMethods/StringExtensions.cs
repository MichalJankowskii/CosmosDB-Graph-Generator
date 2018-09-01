namespace GraphCreator.ExtensionMethods
{
    using System.Text;

    public static class StringExtensions
    {
        public static string EnsureOnlyLetterDigitOrWhiteSpace (this string text)
        {
            StringBuilder cleanedInput = null;
            for (var i = 0; i < text.Length; ++i)
            {
                var currentChar = text[i];
                var charIsValid = char.IsLetterOrDigit(currentChar) || char.IsWhiteSpace(currentChar);

                if (charIsValid)
                {
                    cleanedInput?.Append(currentChar);
                }
                else
                {
                    if (cleanedInput != null) continue;
                    cleanedInput = new StringBuilder();
                    if (i > 0)
                    {
                        cleanedInput.Append(text.Substring(0, i));
                    }
                }
            }

            return cleanedInput == null ? text : cleanedInput.ToString();
        }
    }
}