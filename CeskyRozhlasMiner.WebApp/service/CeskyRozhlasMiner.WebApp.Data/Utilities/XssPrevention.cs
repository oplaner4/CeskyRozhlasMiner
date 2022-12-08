using Ganss.Xss;

namespace CeskyRozhlasMiner.WebApp.Data.Utilities
{
    public class XssPrevention
    {
        /// <summary>
        /// Sanitizes <paramref name="input"/> and saves sanitized value 
        /// into the <paramref name="result"/>.
        /// </summary>
        /// <param name="input">Input to sanitize.</param>
        /// <param name="result">Buffer to fill.</param>
        /// <returns><see cref="true"/> if sanitization was necessary, otherwise 
        /// <see cref="false"/>.</returns>
        public static bool Sanitize(string input, out string result)
        {
            result = Sanitize(input);
            return input != result;
        }

        /// <summary>
        /// Sanitizes <paramref name="input"/>.
        /// </summary>
        /// <param name="input">Input to sanitize.</param>
        /// <returns>Sanitized value.</returns>
        public static string Sanitize(string input)
        {
            if (input == null)
            {
                return null;
            }

            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(input);
        }
    }
}
