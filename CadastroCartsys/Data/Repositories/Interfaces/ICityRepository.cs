using CadastroCartsys.Domain.Entities;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface ICityRepository
    {
        IEnumerable<Cidade> GetAll();
    }
}
