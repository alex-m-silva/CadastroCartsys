using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes
{
    public interface IClientView
    {
        event EventHandler SearchClientsEvent;
        event EventHandler FilterAlteredEvent;
        event EventHandler ClientSelectionEvent;

        void SetCustomerListBindingSource(BindingSource source);
        void FillFilterComboBox(DataGridViewColumnCollection Columns);

        DataGridView DataGridClients { get; }
        DataGridViewColumnCollection Columns { get; }
        Cliente? SelectedClient { get; }

        string SearchTerm { get; }
        string FieldResearch { get; }
        int SelectedId { get; set; }
    }
}
