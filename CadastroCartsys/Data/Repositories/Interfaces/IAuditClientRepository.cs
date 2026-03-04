using CadastroCartsys.Core.Enums;
using CadastroCartsys.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface IAuditClientRepository
    {
        Task LogAsync(Cliente cliente, AuditAction action);
    }
}
