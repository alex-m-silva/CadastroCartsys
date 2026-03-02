using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Domain.Entities;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task<IEnumerable<Cliente>> SearchAsync(CustomerFilterDto filtro);
        Task<int> SaveAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }
}
