using CadastroCartsys.Common.Extensions;
using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Core.Enums;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Infrastructure.ViaCep.Interfaces;
using CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes;
using CadastroCartsys.Presentation.Views;
using Microsoft.CodeAnalysis;

namespace CadastroCartsys.Presentation.Presenters.Cadastro.Clientes
{
    public class ClientFormPresenter
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IAuditClientRepository _auditRepository;

        private readonly ICepService _cepService;

        private readonly Func<Action<Cliente>, ClientView> _clientFactory;

        private IEnumerable<Estado> _stateCache = [];
        private IEnumerable<Cidade> _citiesCache = [];

        private IClientFormView _view = null!;
        private ClientView? _customerForm;

        public ClientFormPresenter(
            IClientRepository clientRepository,
            ICityRepository cityRepository,
            IStateRepository stateRepository,
            IAuditClientRepository auditRepository,
            ICepService cepService,
            Func<Action<Cliente>, ClientView> clientFactory
            )
        {
            _clientRepository = clientRepository;
            _cityRepository = cityRepository;
            _stateRepository = stateRepository;
            _auditRepository = auditRepository;
            _cepService = cepService;

            _clientFactory = clientFactory;
        }

        public void SetView(IClientFormView view)
        {
            _view = view;
            AssociateEventHandlers();
            LoadStates();
            LoadCities();
        }

        private void AssociateEventHandlers()
        {
            _view.LoadSearchClientEvent += LoadSearchClient;

            _view.FormatCpfCnpjEvent += FormatCpfCnpj;

            _view.SearchCepEvent += SearchCep;
            _view.FilterCityEvent += FilterCity;

            _view.DeleteClientEvent += DeleteClient;
            _view.SaveClientEvent += SaveClient;
        }

        private void LoadSearchClient(object? sender, EventArgs e)
        {
            OpenClientView();
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

        private void FormatCpfCnpj(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_view.CpfCnpj)) return;

            _view.CpfCnpj = _view.CpfCnpj.FormatCpfCnpj();
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
                    _view.DisplayAttentionMessage
                        ("CEP não encontrado.");
                    return;
                }

                _view.Endereco = result.Logradouro ?? string.Empty;
                _view.Bairro = result.Bairro ?? string.Empty;
                _view.Cidade = result.Localidade ?? string.Empty;
                _view.Estado = _stateCache
                    .FirstOrDefault(e => e.Uf == result.Uf)?
                    .Nome ?? string.Empty;

                _view.Cep = _view.Cep.FormatCep();

            }
            catch (ArgumentException ex)
            {
                _view.DisplayErrorMessage
                    ("CEP Inválido");
            }
            catch (Exception)
            {
                _view.DisplayErrorMessage
                    ("Erro ao consultar CEP. Verifique sua conexão.");
            }
        }
        private void FilterCity(object? sender, EventArgs e)
        {
            if (_view.ComboState.SelectedValue is not int stateId)
            {
                FilterCitiesDataSource(_citiesCache.ToList());
                FilterStatesDataSource(_stateCache.ToList());
                return;
            }

            _view.ComboCity.SelectedIndex = -1;

            var cities = _citiesCache
                .Where(x => x.EstadoId == stateId)
                .ToList();

            FilterCitiesDataSource(cities);
        }
        private async void LoadStates()
        {
            _stateCache = await _stateRepository.GetAllAsync();
            FilterStatesDataSource(_stateCache.ToList());
        }
        private async void LoadCities()
        {
            _citiesCache = await _cityRepository.GetAllAsync();
            FilterCitiesDataSource(_citiesCache.ToList());
        }
        private void FilterStatesDataSource(List<Estado> states)
        {
            _view.ComboState.DataSource = states;
            _view.ComboState.DisplayMember = nameof(Estado.Nome);
            _view.ComboState.ValueMember = nameof(Estado.Id);
            _view.ComboState.SelectedIndex = -1;
        }
        private void FilterCitiesDataSource(IEnumerable<Cidade> cities)
        {
            _view.ComboCity.DataSource = cities;
            _view.ComboCity.DisplayMember = nameof(Cidade.Nome);
            _view.ComboCity.ValueMember = nameof(Cidade.Id);
            _view.ComboCity.SelectedIndex = -1;
        }

        private async void SaveClient(object? sender, EventArgs e)
        {
            try
            {
                var form = _view.GetForm();

                if (!ValidateClientSave(form)) return;

                var client = new Cliente(
                    form.Id,
                    form.Nome,
                    form.CpfCnpj.OnlyDigits(),
                    form.CidadeId,
                    form.Cep.OnlyDigits(),
                    form.Endereco,
                    form.Numero,
                    form.Complemento,
                    form.Bairro,
                    form.DataNascimento
                );

                var isInsert = form.Id == 0;
                var newId = await _clientRepository.SaveAsync(client);

                var clientSaved = await _clientRepository.GetByIdAsync(newId);

                await _auditRepository.LogAsync(
                    clientSaved,
                    isInsert ? AuditAction.Insert : AuditAction.Update
);

                var message = isInsert
                            ? $"Cliente cadastrado com sucesso! Código: {newId}"
                    : "Cliente atualizado com sucesso!";

                _view.DisplaySuccessMessage(message);
                if (isInsert)
                {
                    if (clientSaved is not null)
                        PopularForm(clientSaved);
                }
            }
            catch (Exception ex)
            {
                _view.DisplayErrorMessage($"Erro ao salvar cliente: {ex.Message}");
            }
        }
        public bool ValidateClientSave(ClientFormDto clientForm)
        {
            if (string.IsNullOrWhiteSpace(clientForm.Nome))
            {
                _view.DisplayAttentionMessage("Nome é obrigatório.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(clientForm.CpfCnpj))
            {
                _view.DisplayAttentionMessage("CPF/CNPJ é obrigatório.");
                return false;
            }

            var digits = clientForm.CpfCnpj.OnlyDigits();
            if (digits.Length != 11 && digits.Length != 14)
            {
                _view.DisplayErrorMessage(
                    "CPF deve conter 11 dígitos ou CNPJ deve conter 14 dígitos.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(clientForm.Cep))
            {
                var cepDigits = clientForm.Cep.OnlyDigits();
                if (cepDigits.Length != 8)
                {
                    _view.DisplayErrorMessage("CEP deve conter 8 dígitos.");
                    return false;
                }
            }

            if (clientForm.CidadeId == 0 && !string.IsNullOrWhiteSpace(clientForm.CidadeNome))
            {
                var location = !string.IsNullOrWhiteSpace(clientForm.EstadoNome)
                    ? $"{clientForm.EstadoNome}: {clientForm.CidadeNome}"
                    : clientForm.CidadeNome;

                _view.DisplayAttentionMessage(
                    $"A cidade '{location}' não está cadastrada no sistema.\n" +
                    "Entre em contato com o administrador.");

                return false;
            }
            if (clientForm.CidadeId == 0)
            {
                _view.DisplayAttentionMessage(
                    $"Cidade é obrigatória.");

                return false;
            }

            return true;
        }

        private async void DeleteClient(object? sender, EventArgs e)
        {
            var dto = _view.GetForm();

            if (dto.Id == 0)
            {
                _view.DisplayAttentionMessage("Nenhum cliente selecionado para exclusão.");
                return;
            }
            var client = await _clientRepository.GetByIdAsync(dto.Id);

            if (!ValidateClientExclusion(client)) return;

            await _auditRepository.LogAsync(client!, AuditAction.Delete);

            await _clientRepository.DeleteAsync(client!.Id);
            _view.DisplaySuccessMessage($"Cliente {client.Nome} excluído com sucesso!");
            _view.ClearFields();
        }
        public bool ValidateClientExclusion(Cliente? client)
        {
            if (client == null)
            {
                _view.DisplayErrorMessage("Cliente não encontrado no banco de dados.");
                return false;
            }

            if (!client.CanBeDeleted())
            {
                _view.DisplayAttentionMessage(
                    $"O cliente Código {client.Id} não pode ser excluído por questões de segurança.\n\n" +
                    $"Motivo: Código protegido pelo sistema."
                    );
                return false;
            }

            return _view.ShowConfirmation(
                $"Confirmar exclusão do cliente:\n\n" +
                $"Código: {client.Id}\n" +
                $"Nome: {client.Nome}\n" +
                $"Esta operação não pode ser desfeita!",
                "Confirmar Exclusão");
        }
        
        private async void PopularForm(Cliente client)
        {
            var dto = new ClientFormDto
            {
                Id = client.Id,
                Nome = client.Nome,
                CpfCnpj = client.CpfCnpj,
                Cep = client.Cep,
                Endereco = client.Endereco,
                Numero = client.Numero,
                Complemento = client.Complemento,
                Bairro = client.Bairro,
                CidadeNome = client.Cidade?.Nome ?? string.Empty,
                EstadoNome = client.Cidade?.Estado?.Nome ?? string.Empty,
                DataNascimento = client.DataNascimento
            };

            _view.PopularForm(dto);
            await SelectCombos(client);
        }
        private async Task SelectCombos(Cliente client)
        {
            if (!string.IsNullOrWhiteSpace(client.Cep))
            {
                var result = await _cepService.GetCepAsync(client.Cep);

                if (result is not null && !result.Erro)
                {
                    SelectStateandCity(result.Uf!, result.Localidade!);
                    return;
                }
            }

            if (client.Cidade is not null)
                SelectStateandCity(
                    client.Cidade.Estado?.Uf ?? string.Empty,
                    client.Cidade.Nome
                );
        }

        private void SelectStateandCity(string uf, string cityName)
        {
            if (string.IsNullOrWhiteSpace(uf)) return;

            var state = _stateCache
                .FirstOrDefault(e => e.Uf.Equals(uf, StringComparison.OrdinalIgnoreCase));

            if (state is null) return;

            _view.ComboState.SelectedValue = state.Id;

            if (string.IsNullOrWhiteSpace(cityName)) return;

            var city = _citiesCache
                .FirstOrDefault(c => c.EstadoId == state.Id &&
                                     c.Nome.Equals(cityName, StringComparison.OrdinalIgnoreCase));

            if (city is not null)
                _view.ComboCity.SelectedValue = city.Id;
        }
    }
}
