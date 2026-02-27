using CadastroCartsys.Domain.Entities;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface ICityRepository
    {
        IEnumerable<Cidade> GetAll();
        IEnumerable<Cidade> GetByState(int estadoId);
        Cidade? GetById(int id);
    }
}
