using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Infrastructure.ViaCep.Interfaces;
using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Views;
using System.Runtime.ConstrainedExecution;

namespace CadastroCartsys.Presentation.Presenters
{
    public class ClientFormPresenter
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IStateRepository _stateRepository;

        private readonly Func<Action<Cliente>, ClientView> _clientFactory;

        private readonly ICepService _cepService;

        private ClientView? _customerForm;
        private IClientFormView _view = null!;

        public ClientFormPresenter(
            IClientRepository clientRepository,
            ICityRepository cityRepository,
            IStateRepository stateRepository,
            ICepService cepService,
            Func<Action<Cliente>, ClientView> clientFactory
            )
        {
            _clientRepository = clientRepository;
            _cityRepository = cityRepository;
            _stateRepository = stateRepository;
            _cepService = cepService;

            _clientFactory = clientFactory;
        }

        public void SetView(IClientFormView view)
        {
            _view = view;
            AssociateEventHandlers();
            LoadStates();
        }

        private void AssociateEventHandlers()
        {
            _view.LoadSearchClientEvent += LoadSearchClient;
            _view.SearchCepEvent += SearchCep;
            _view.SaveClientEvent += SaveClient;
            _view.FilterCityEvent += FilterCity;
        }

        private void LoadSearchClient(object? sender, EventArgs e)
        {
            OpenClientView();
        }

        private async void SearchCep(object? sender, EventArgs e)
        {
            try
            {
                var cep = _view.Cep;

                if (string.IsNullOrWhiteSpace(cep)) return;

                var result = await _cepService.GetCepAsync(cep);

                if (result is null)
                {
                    MessageBox.Show("CEP não encontrado.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _view.Endereco = result.Logradouro ?? string.Empty;
                _view.Bairro = result.Bairro ?? string.Empty;
                _view.Cidade = result.Localidade ?? string.Empty;
                _view.Estado = result.Uf ?? string.Empty;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "CEP Inválido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao consultar CEP. Verifique sua conexão.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenClientView()
        {
            if (_customerForm == null || _customerForm.IsDisposed)
            {
                _customerForm = _clientFactory(cliente => PopularForm(cliente));
                _customerForm.FormClosed += (s, e) => _customerForm = null;
                _customerForm.ShowDialog();
            }
            else
            {
                _customerForm.BringToFront();
            }
        }
        private async void PopularForm(Cliente cliente)
        {
            var dto = new ClientFormDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                CpfCnpj = cliente.CpfCnpj,
                Cep = cliente.Cep,
                Endereco = cliente.Endereco,
                Numero = cliente.Numero,
                Complemento = cliente.Complemento,
                Bairro = cliente.Bairro,
                CidadeNome = cliente.Cidade?.Nome ?? string.Empty,
                EstadoNome = cliente.Cidade?.Estado?.Nome ?? string.Empty,
                DataNascimento = cliente.DataNascimento
            };

            if (!string.IsNullOrWhiteSpace(cliente.Cep))
            {
                //var result = await _cepService.GetCepAsync(cliente.Cep);
               //  _view.ComboState = result.
            }
            _view.PopularForm(dto);
        }

        private void SaveClient(object? sender, EventArgs e)
        {
            try
            {
                var dto = _view.GetForm();

                // Valida campos obrigatórios
                if (string.IsNullOrWhiteSpace(dto.Nome))
                {
                    _view.DisplayErrorMessage("Nome é obrigatório.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(dto.CpfCnpj))
                {
                    _view.DisplayErrorMessage("CPF/CNPJ é obrigatório.");
                    return;
                }

                if (dto.CidadeId == 0)
                {
                    _view.DisplayErrorMessage(
                        $@"Cidade '{dto.CidadeNome}' não encontrada no sistema.
                        Por favor, entre em contato com o responsavel para cadastrá-la."
                    );
                    return;
                }

                var cliente = new Cliente(
                    dto.Id,
                    dto.Nome,
                    dto.CpfCnpj,
                    dto.CidadeId,
                    dto.Cep,
                    dto.Endereco,
                    dto.Numero,
                    dto.Complemento,
                    dto.Bairro,
                    dto.DataNascimento
                );

                var novoId = _clientRepository.Save(cliente);

                var mensagem = dto.Id == 0
                    ? $"Cliente cadastrado com sucesso! ID: {novoId}"
                    : "Cliente atualizado com sucesso!";

                _view.DisplaySuccessMessage(mensagem);
            }
            catch (Exception ex)
            {
                _view.DisplayErrorMessage($"Erro ao salvar cliente: {ex.Message}");
            }
        }

        private void LoadStates()
        {
            var estados = _stateRepository.GetAll().ToList();

            _view.ComboState.DataSource = estados;
            _view.ComboState.DisplayMember = nameof(Estado.Nome);
            _view.ComboState.ValueMember = nameof(Estado.Id);
            _view.ComboState.SelectedIndex = -1;
        }

        private void FilterCity(object? sender, EventArgs e)
        {
            if (_view.ComboState.SelectedValue is not int estadoId) return;

            var cidades = _cityRepository.GetByState(estadoId).ToList();

            _view.ComboCity.DataSource = cidades;
            _view.ComboCity.DisplayMember = nameof(Cidade.Nome);
            _view.ComboCity.ValueMember = nameof(Cidade.Id);
            _view.ComboCity.SelectedIndex = -1;
        }
    }
}
