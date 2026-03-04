using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Core.Enums;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Infrastructure.ViaCep.Interfaces;
using CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes;
using CadastroCartsys.Presentation.Presenters.Cadastro.Clientes;
using CadastroCartsys.Presentation.Views;
using FluentAssertions;
using NSubstitute;

namespace CadastroCartsys.Tests.Presenters
{
    public class ClientFormPresenterTests
    {
        private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
        private readonly IStateRepository _stateRepository = Substitute.For<IStateRepository>();
        private readonly ICityRepository _cityRepository = Substitute.For<ICityRepository>();
        private readonly ICepService _cepService = Substitute.For<ICepService>();
        private readonly IAuditClientRepository _auditRepository = Substitute.For<IAuditClientRepository>();
        private readonly IClientFormView _view = Substitute.For<IClientFormView>();
        private readonly ClientFormPresenter _presenter;

        public ClientFormPresenterTests()
        {
            Func<Action<Cliente>, ClientView> clientFactory = _ => null!;

            _view.ComboState.Returns(new ComboBox());
            _view.ComboCity.Returns(new ComboBox());

            _stateRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Estado>>([]));
            _cityRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Cidade>>([]));

            _presenter = new ClientFormPresenter(
                _clientRepository,
                _cityRepository,
                _stateRepository,
                _auditRepository,
                _cepService,
                clientFactory
            );

            _presenter.SetView(_view);
        }

        [Fact]
        public void SalvarCliente_NomeVazio_DeveExibirErro()
        {
            var resultado = _presenter.ValidateClientSave(new ClientFormDto
            {
                Nome = "",
                CpfCnpj = "12345678901",
                CidadeId = 1
            });

            resultado.Should().BeFalse();
            _view.Received(1).DisplayAttentionMessage("Nome é obrigatório.");
        }

        [Fact]
        public void SalvarCliente_CpfVazio_DeveExibirErro()
        {
            var resultado = _presenter.ValidateClientSave(new ClientFormDto
            {
                Nome = "Alex Silva",
                CpfCnpj = "",
                CidadeId = 1
            });

            resultado.Should().BeFalse();
            _view.Received(1).DisplayAttentionMessage("CPF/CNPJ é obrigatório.");
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234567890")]
        [InlineData("123456789012")]
        public void SalvarCliente_CpfCnpjTamanhoInvalido_DeveExibirErro(string cpfCnpj)
        {
            var resultado = _presenter.ValidateClientSave(new ClientFormDto
            {
                Nome = "Alex Silva",
                CpfCnpj = cpfCnpj,
                CidadeId = 1
            });

            resultado.Should().BeFalse();
            _view.Received(1).DisplayErrorMessage(
                "CPF deve conter 11 dígitos ou CNPJ deve conter 14 dígitos.");
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("123456789")]
        public void SalvarCliente_CepInvalido_DeveExibirErro(string cep)
        {
            var resultado = _presenter.ValidateClientSave(new ClientFormDto
            {
                Nome = "Alex Silva",
                CpfCnpj = "12345678901",
                CidadeId = 1,
                Cep = cep
            });

            resultado.Should().BeFalse();
            _view.Received(1).DisplayErrorMessage("CEP deve conter 8 dígitos.");
        }

        [Fact]
        public void SalvarCliente_SemCidade_DeveExibirErro()
        {
            var resultado = _presenter.ValidateClientSave(new ClientFormDto
            {
                Nome = "Alex Silva",
                CpfCnpj = "12345678901",
                CidadeId = 0,
                CidadeNome = string.Empty
            });

            resultado.Should().BeFalse();
            _view.Received(1).DisplayAttentionMessage("Cidade é obrigatória.");
        }

        [Fact]
        public void SalvarCliente_CidadeNaoCadastrada_DeveExibirAtencao()
        {
            var resultado = _presenter.ValidateClientSave(new ClientFormDto
            {
                Nome = "Alex Silva",
                CpfCnpj = "12345678901",
                CidadeId = 0,
                CidadeNome = "Cidade Inexistente",
                EstadoNome = "Minas Gerais"
            });

            resultado.Should().BeFalse();
            _view.Received(1).DisplayAttentionMessage(
                Arg.Is<string>(m => m.Contains("Cidade Inexistente")));
        }

        [Fact]
        public void SalvarCliente_DadosValidos_DeveRetornarTrue()
        {
            var resultado = _presenter.ValidateClientSave(new ClientFormDto
            {
                Nome = "Alex Silva",
                CpfCnpj = "12345678901",
                CidadeId = 1
            });

            resultado.Should().BeTrue();
        }

        [Fact]
        public void ExcluirCliente_IdProtegido_DeveExibirAtencao()
        {
            var cliente = new Cliente(1, "Cliente Protegido", "12345678901", 1);
            var resultado = _presenter.ValidateClientExclusion(cliente);

            resultado.Should().BeFalse();
            _view.Received(1).DisplayAttentionMessage(Arg.Any<string>());
        }

        [Theory]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(15)]
        public void ExcluirCliente_TodosIdsProtegidos_DeveExibirAtencao(int id)
        {
            var cliente = new Cliente(id, "Cliente Protegido", "12345678901", 1);
            var resultado = _presenter.ValidateClientExclusion(cliente);

            resultado.Should().BeFalse();
            _view.Received(1).DisplayAttentionMessage(Arg.Any<string>());
        }

        [Fact]
        public void ExcluirCliente_IdNaoProtegido_UsuarioConfirma_DeveRetornarTrue()
        {
            var cliente = new Cliente(99, "Alex Silva", "12345678901", 1);
            _view.ShowConfirmation
                (Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var resultado = _presenter.ValidateClientExclusion(cliente);

            resultado.Should().BeTrue();
        }

        [Fact]
        public void ExcluirCliente_IdNaoProtegido_UsuarioCancelou_DeveRetornarFalse()
        {
            var cliente = new Cliente(99, "Alex Silva", "12345678901", 1);
            _view.ShowConfirmation
                (Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            var resultado = _presenter.ValidateClientExclusion(cliente);

            resultado.Should().BeFalse();
        }

        [Fact]
        public void ExcluirCliente_ClienteNulo_DeveExibirErro()
        {
            var resultado = _presenter.ValidateClientExclusion(null);

            resultado.Should().BeFalse();
            _view.Received(1).DisplayErrorMessage(Arg.Any<string>());
        }
    }
}
