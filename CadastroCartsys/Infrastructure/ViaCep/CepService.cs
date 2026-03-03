using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Infrastructure.ViaCep.Interfaces;
using System.Net.Http.Json;

namespace CadastroCartsys.Infrastructure.ViaCep
{
    public class CepService : ICepService
    {
        private readonly HttpClient _httpClient;

        public CepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CepResultDto?> GetCepAsync(string cep)
        {
            var cepClear = cep.Replace("-", "").Trim();

            if (cepClear.Length != 8 || !cepClear.All(char.IsDigit))
                throw new ArgumentException("CEP inválido. Informe 8 dígitos numéricos.");

            var result = await _httpClient
                .GetFromJsonAsync<CepResultDto>($"https://viacep.com.br/ws/{cepClear}/json/");

            if (result is null || result.Erro)
                return null;

            return result;
        }
    }
}
