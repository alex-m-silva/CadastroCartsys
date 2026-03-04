using CadastroCartsys.Common.Extensions;
using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes;

namespace CadastroCartsys.Presentation.Presenters.Cadastro.Clientes
{
    public class ClientPresenter
    {
        private readonly IClientRepository _clientRepository;

        private IEnumerable<ClientFormDto> _clientsInMemory = [];
        private readonly BindingSource _clientBindingSource = [];

        private IClientView _view = null!;
        private Action<Cliente>? _onSelectedClient;

        public ClientPresenter(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public void SetView(IClientView view)
        {
            _view = view;
            AssociateEventHandlers();
            _view.FillFilterComboBox(_view.Columns);
        }

        private void AssociateEventHandlers()
        {
            _view.SetCustomerListBindingSource(_clientBindingSource);
            _view.SearchClientsEvent += SearchinMemory;
            _view.FilterAlteredEvent += SearchinMemory;
            _view.ClientSelectionEvent += WhenSelectingClient;

            LoadAllClients();
        }

        private async void LoadAllClients()
        {
            try
            {
                var clients = await _clientRepository.GetAllAsync();

                _clientsInMemory = clients
                    .Select(c => new ClientFormDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        CpfCnpj = c.CpfCnpj,
                        Cep = c.Cep,
                        Endereco = c.Endereco,
                        Numero = c.Numero,
                        Complemento = c.Complemento,
                        Bairro = c.Bairro,
                        CidadeId = c.CidadeId,
                        CidadeNome = c.Cidade?.Nome ?? string.Empty,
                        EstadoNome = c.Cidade?.Estado?.Nome ?? string.Empty,
                        DataNascimento = c.DataNascimento
                    });
            }
            catch (Exception ex)
            {
                _view.DisplayErrorMessage($"Erro ao carregar clientes");
            }
        }
        private async void SearchinMemory(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_view.SearchTerm)) return;

            var field = _view.FieldResearch;
            var term = _view.SearchTerm.RemoveAccents().ToLower();

            var result = _clientsInMemory
                .Where(c =>
                {
                    var propriedade = c.GetType().GetProperty(field);
                    if (propriedade is null) return false;

                    var value = propriedade.GetValue(c)?.ToString() ?? string.Empty;
                    return value.ToLower()
                                .StartsWith(term, StringComparison.OrdinalIgnoreCase);
                }).Take(1000);

            _clientBindingSource.DataSource = result.ToList();
        }

        private async void WhenSelectingClient(object? sender, EventArgs e)
        {
            var client = await _clientRepository.GetByIdAsync(_view.SelectedId.Value);
            if (client is null) return;

            _onSelectedClient?.Invoke(client);
        }
        public void SetCallback(Action<Cliente> callback)
        {
            _onSelectedClient = callback;
        }
    }
}
