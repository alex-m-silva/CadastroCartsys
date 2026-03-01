using CadastroCartsys.Common.Extensions;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Presentation.Interfaces;

namespace CadastroCartsys.Presentation.Presenters
{
    public class ClientPresenter
    {
        private readonly IClientRepository _clientRepository;
        private IClientView _view = null!;

        private List<dynamic> _clientsInMemory = [];
        private readonly BindingSource _clientBindingSource = [];

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
            _view.FilterAlteredEvent += (s, e) => UpdateGrid(_clientsInMemory);

            LoadAllClients();
        }

        private void LoadAllClients()
        {
            try
            {
                _clientsInMemory = _clientRepository.GetAll()
               .Select(c => (dynamic)new
               {
                   c.Id,
                   c.Nome,
                   c.CpfCnpj,
                   c.Endereco,
                   c.Numero,
                   c.Bairro,
                   c.Complemento,
                   Cidade = c.Cidade.Nome ?? string.Empty,
                   Estado = c.Cidade.Estado.Nome ?? string.Empty
               })
               .ToList();

                UpdateGrid(_clientsInMemory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar clientes: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void SearchinMemory(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_view.SearchTerm))
            {
                UpdateGrid(_clientsInMemory);
                return;
            }

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
                })
                .ToList();

            UpdateGrid(result);
        }
        private void UpdateGrid(List<dynamic> clientes)
        {
            try
            {
                _clientBindingSource.RaiseListChangedEvents = false;
                _clientBindingSource.DataSource = clientes;
            }
            finally
            {
                _clientBindingSource.RaiseListChangedEvents = true;
                _clientBindingSource.ResetBindings(false);
            }
        }
    }
}
