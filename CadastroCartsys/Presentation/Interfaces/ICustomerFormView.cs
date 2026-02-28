using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Presentation.Interfaces
{
    public interface ICustomerFormView
    {
        event EventHandler SearchCepEvent;

        string Cep { get; }
        string Endereco { get; set; } 
        string Bairro { get; set; }
        string Cidade { get; set; }
        string Estado { get; set; }
    }
}
