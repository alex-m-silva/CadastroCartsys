using CadastroCartsys.Data.Repositories;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Presentation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Presentation.Presenters
{
    public class CustomerPresenter
    {
        private readonly IClientRepository _clientRepository;
        private ICustomerView _view = null!;

        private readonly BindingSource customerBindingSource = [];

        public CustomerPresenter(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public void SetView(ICustomerView view)
        {
            _view = view;
            AssociateEventHandlers();
        }

        private void AssociateEventHandlers()
        {
            _view.SetCustomerListBindingSource(customerBindingSource);
            _view.SearchCustomersEvent += SearchCustomers;

            LoadAllClients();
        }

        private void LoadAllClients()
        {
            try
            {
                customerBindingSource.RaiseListChangedEvents = false;
                customerBindingSource.DataSource = _clientRepository.GetAll().ToList();
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
            finally
            {
                customerBindingSource.RaiseListChangedEvents = true;  
                customerBindingSource.ResetBindings(false);
            }
        }

        private void SearchCustomers(object? sender, EventArgs e)
        {
            try
            {
                var filtro = _view.GetFilters();

                // Se todos os campos estiverem vazios recarrega tudo
                var temFiltro = filtro.Id is not null ||
                                filtro.Nome is not null ||
                                filtro.CpfCnpj is not null ||
                                filtro.Cep is not null ||
                                filtro.Cidade is not null ||
                                filtro.Estado is not null ||
                                filtro.DataNascimento is not null;

                customerBindingSource.RaiseListChangedEvents = false;
                customerBindingSource.DataSource = temFiltro
                    ? _clientRepository.Search(filtro).ToList()
                    : _clientRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao pesquisar: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                customerBindingSource.RaiseListChangedEvents = true;
                customerBindingSource.ResetBindings(false);
            }
        }
    }
}
