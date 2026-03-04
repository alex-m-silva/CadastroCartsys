using CadastroCartsys.Core.Enums;
using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using Dapper;

namespace CadastroCartsys.Data.Repositories
{
    public class AuditClientRepository : IAuditClientRepository
    {
        private readonly DbContext _context;

        public AuditClientRepository(DbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(Cliente cliente, AuditAction action)
        {
            using var connection = _context.CreateConnection();

            const string sql = """
            INSERT INTO dbo.AUDITORIA_CLIENTE
                (CLIENTE_ID, NOME, CPF_CNPJ, ACAO, DATA_HORA, USUARIO, WORKSTATION)
            VALUES
                (@ClienteId, @Nome, @CpfCnpj, @Acao, @DataHora, @Usuario, @Workstation)
            """;

            await connection.ExecuteAsync(sql, new
            {
                ClienteId = cliente.Id,
                Nome = cliente.Nome,
                CpfCnpj = cliente.CpfCnpj,
                Acao = action.ToString().ToUpper(),
                DataHora = DateTime.Now,
                Usuario = Environment.UserName,
                Workstation = Environment.MachineName
            });
        }
    }
}
