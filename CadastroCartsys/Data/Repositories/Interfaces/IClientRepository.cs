using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Domain.Entities;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface IClientRepository
    {
        IEnumerable<Cliente> GetAll();
        Cliente? GetById(int id);
        IEnumerable<Cliente> Search(CustomerFilterDto filtro);
        int Save(Cliente cliente);
        void Delete(int id);
    }
}
