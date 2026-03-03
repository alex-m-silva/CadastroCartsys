using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using Dapper;

namespace CadastroCartsys.Data.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly DbContext _context;

        public CityRepository(DbContext context)
        {
            _context = context;
        }

        public IEnumerable<Cidade> GetAll()
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                                    SELECT
                                        c.ID,
                                        c.NOME,
                                        c.ESTADOID,
                                        e.ID   AS ESTADO_ID,
                                        e.NOME AS ESTADO_NOME,
                                        e.UF
                                    FROM dbo.CIDADE c
                                    JOIN dbo.ESTADO e ON e.ID = c.ESTADOID
                                    ORDER BY c.NOME ASC
                                    """;

            return connection.Query<Cidade, Estado, Cidade>(
                sql,
                map: (cidade, estado) => new Cidade(cidade.Id, cidade.Nome, estado.Id, estado),
                splitOn: "ESTADO_ID"
            );
        }
    }
}
