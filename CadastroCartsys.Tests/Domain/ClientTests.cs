using CadastroCartsys.Domain.Entities;
using FluentAssertions;

namespace CadastroCartsys.Tests.Domain
{
    public class ClientTests
    {
        [Theory]
        [InlineData(1, false)]
        [InlineData(5, false)]
        [InlineData(8, false)]
        [InlineData(10, false)]
        [InlineData(15, false)]
        [InlineData(2, true)]
        [InlineData(99, true)]
        public void CanBeDeleted_DeveRetornarCorreto(int id, bool esperado)
        {
            // Arrange
            var cliente = new Cliente(id, "Teste", "12345678901", 1);

            // Act
            var resultado = cliente.CanBeDeleted();

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("12345678901")]
        [InlineData("12345678901234")]
        public void Cliente_CpfCnpjValido_DeveCriarSemErro(string cpfCnpj)
        {
            // Act
            var act = () => new Cliente(1, "Teste", cpfCnpj, 1);

            // Assert
            act.Should().NotThrow();
        }
    }
}
