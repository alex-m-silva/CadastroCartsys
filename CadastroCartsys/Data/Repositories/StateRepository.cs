using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using Dapper;

namespace CadastroCartsys.Data.Repositories
{
    internal class StateRepository : IStateRepository
    {
        private readonly DbContext _context;

        public StateRepository(DbContext context)
        {
            _context = context;
        }

        public IEnumerable<Estado> GetAll()
        {
            using var connection = _context.CreateConnection();

            const string sql = """
            SELECT
                ID,
                NOME,
                UF
            FROM dbo.ESTADO
            ORDER BY NOME ASC
            """;

            return connection.Query<Estado>(sql);
        }
    }
}
