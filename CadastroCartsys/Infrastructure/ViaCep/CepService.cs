using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Infrastructure.ViaCep.Interfaces;
using System.Net.Http;
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
            var cepLimpo = cep.Replace("-", "").Trim();

            if (cepLimpo.Length != 8 || !cepLimpo.All(char.IsDigit))
                throw new ArgumentException("CEP inválido. Informe 8 dígitos numéricos.");

            var resultado = await _httpClient
                .GetFromJsonAsync<CepResultDto>($"https://viacep.com.br/ws/{cepLimpo}/json/");

            if (resultado is null || resultado.Erro)
                return null;

            return resultado;
        }
    }
}
