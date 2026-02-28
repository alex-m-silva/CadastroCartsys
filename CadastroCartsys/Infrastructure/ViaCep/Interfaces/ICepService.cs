using CadastroCartsys.Core.DTOs;

namespace CadastroCartsys.Infrastructure.ViaCep.Interfaces
{
    public interface ICepService
    {
        Task<CepResultDto?> GetCepAsync(string cep);
    }
}
