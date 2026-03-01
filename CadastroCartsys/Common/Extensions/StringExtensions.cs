using System.Globalization;
using System.Text;

namespace CadastroCartsys.Common.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveAccents(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var normalize = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalize)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
