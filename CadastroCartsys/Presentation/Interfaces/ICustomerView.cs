using CadastroCartsys.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Presentation.Interfaces
{
    public interface ICustomerView
    {
        event EventHandler SearchCustomersEvent;
        void SetCustomerListBindingSource(BindingSource source);
        CustomerFilterDto GetFilters();
    }
}
