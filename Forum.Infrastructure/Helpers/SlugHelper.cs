using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Forum.Infrastructure.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim(); 
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        private static string RemoveAccent(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            char[] chars = text
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}