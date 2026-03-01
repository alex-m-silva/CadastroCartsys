using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Presentation.Interfaces
{
    public interface IClientView
    {
        event EventHandler SearchClientsEvent;
        event EventHandler FilterAlteredEvent;

        void SetCustomerListBindingSource(BindingSource source);
        void FillFilterComboBox(DataGridViewColumnCollection Columns);

        DataGridViewColumnCollection Columns { get; }

        string SearchTerm { get; }
        string FieldResearch { get; }
    }
}
