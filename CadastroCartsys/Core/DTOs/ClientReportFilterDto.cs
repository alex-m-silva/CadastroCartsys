using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Core.DTOs
{
    public class ClientReportFilterDto
    {
        public int? IdInicial { get; set; }
        public int? IdFinal { get; set; }
        public int? CidadeId { get; set; }
        public int? EstadoId { get; set; }
        public bool Todos { get; set; } = false;
    }
}
