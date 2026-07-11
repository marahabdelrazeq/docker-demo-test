using System.Text;

namespace CommonLibrary.Utilities.StringHelper
{
    public class StringConverter
    {
        /// <summary>
        /// Converts a given string to Upper Case Snake Case.
        /// Example: "HelloWorld" becomes "HELLO_WORLD".
        /// </summary>
        public static string ConvertToUpperCaseSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('_');
                    }
                    stringBuilder.Append(char.ToUpperInvariant(c));
                }
                else
                {
                    stringBuilder.Append(char.ToUpperInvariant(c));
                }
            }

            return stringBuilder.ToString();
        }
    }
}
