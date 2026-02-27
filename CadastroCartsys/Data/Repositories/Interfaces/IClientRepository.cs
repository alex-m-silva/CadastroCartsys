using CadastroCartsys.Domain.Entities;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Cliente? GetById(int id);
        IEnumerable<Cliente> Search(string campo, string termo);
        int Save(Cliente cliente);
        void Delete(int id);
    }
}
