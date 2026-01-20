using System.Text;

namespace TravelTechApi.Common.Utils
{
    /// <summary>
    /// Utility class for text operations
    /// </summary>
    public static class TextUtils
    {
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = Char.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        public static bool CheckContains(string text, string keyword)
        {
            return RemoveDiacritics(text).Contains(RemoveDiacritics(keyword), StringComparison.OrdinalIgnoreCase);
        }
    }
}