using CadastroCartsys.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface IStateRepository
    {
        IEnumerable<Estado> GetAll();
    }
}
