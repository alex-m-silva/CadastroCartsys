using CadastroCartsys.Common.Extensions;
using FluentAssertions;

namespace CadastroCartsys.Tests.Common
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("12345678901", "123.456.789-01")] // CPF
        [InlineData("12345678901234", "12.345.678/9012-34")] // CNPJ
        [InlineData("123", "123")] // invalido 
        public void FormatCpfCnpj_DeveFormatarCorretamente(string input, string esperado)
        {
            input.FormatCpfCnpj().Should().Be(esperado);
        }

        [Theory]
        [InlineData("12345678", "12345-678")]  // valido
        [InlineData("1234", "1234")]       // invalido 
        public void FormatCep_DeveFormatarCorretamente(string input, string esperado)
        {
            input.FormatCep().Should().Be(esperado);
        }

        [Theory]
        [InlineData("123.456.789-01", "12345678901")] // CPF formatado
        [InlineData("12.345.678/9012-34", "12345678901234")] // CNPJ formatado
        public void OnlyDigits_DeveRemoverFormatacao(string input, string esperado)
        {
            input.OnlyDigits().Should().Be(esperado);
        }

        [Theory]
        [InlineData("café", "cafe")]
        [InlineData("São Paulo", "Sao Paulo")]
        [InlineData("Goiânia", "Goiania")]
        public void RemoveAccents_DeveRemoverAcentos(string input, string esperado)
        {
            input.RemoveAccents().Should().Be(esperado);
        }
    }
}
