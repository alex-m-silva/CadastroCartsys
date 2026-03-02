using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Core.DTOs
{
    public class ClientFormDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string? Cep { get; set; }
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public int CidadeId { get; set; }
        public string CidadeNome { get; set; } = string.Empty;
        public string EstadoNome { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
    }
}
