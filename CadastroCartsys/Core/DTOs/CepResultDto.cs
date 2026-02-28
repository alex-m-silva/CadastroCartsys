using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Core.DTOs
{
    public class CepResultDto
    {
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Bairro { get; set; }
        public string? Localidade { get; set; } 
        public string? Uf { get; set; }  
        public bool Erro { get; set; }
    }
}
