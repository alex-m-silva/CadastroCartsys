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

        public static string FormatCpfCnpj(this string value)
        {

            if (value == null) return string.Empty;
            var digits = new string(value.Where(char.IsDigit).ToArray());

            return digits.Length switch
            {
                11 => $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..11]}",  // CPF: 000.000.000-00
                14 => $"{digits[..2]}.{digits[2..5]}.{digits[5..8]}/{digits[8..12]}-{digits[12..14]}", // CNPJ: 00.000.000/0000-00
                _ => value
            };
        }

        public static string FormatCep(this string? value)
        {
            if (value == null) return string.Empty;
            var digits = new string(value.Where(char.IsDigit).ToArray());

            return digits.Length == 8
                ? $"{digits[..5]}-{digits[5..8]}"
                : value;
        }

        public static string OnlyDigits(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return new string(value.Where(char.IsDigit).ToArray());
        }
    }
}
