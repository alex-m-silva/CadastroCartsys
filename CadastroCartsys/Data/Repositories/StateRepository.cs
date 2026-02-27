using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Estado? GetById(int id)
        {
            using var connection = _context.CreateConnection();

            const string sql = """
            SELECT
                ID,
                NOME,
                UF
            FROM dbo.ESTADO
            WHERE ID = @Id
            """;

            return connection.QueryFirstOrDefault<Estado>(sql, new { Id = id });
        }
    }
}
