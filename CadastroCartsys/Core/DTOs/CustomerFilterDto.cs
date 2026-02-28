using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Core.DTOs
{
    public class CustomerFilterDto
    {
        public string? Id { get; set; }
        public string? Nome { get; set; }
        public string? CpfCnpj { get; set; }
        public string? Cep { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? DataNascimento { get; set; }
    }
}
