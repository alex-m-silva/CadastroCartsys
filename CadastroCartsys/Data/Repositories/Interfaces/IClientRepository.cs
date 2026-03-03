using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Domain.Entities;

namespace CadastroCartsys.Data.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task<int> SaveAsync(Cliente cliente);
        Task DeleteAsync(int id);

        Task<IEnumerable<ClientReportDto>> GetReportAsync
            (ClientReportFilterDto filter);
    }
}
