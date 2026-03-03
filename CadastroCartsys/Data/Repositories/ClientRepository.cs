using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using Dapper;

namespace CadastroCartsys.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly DbContext _context;

        public ClientRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                            SELECT
                                c.ID,
                                c.NOME,
                                c.Cpf_Cnpj  AS CpfCnpj,
                                c.CEP,
                                c.ENDERECO,
                                c.NUMERO,
                                c.COMPLEMENTO,
                                c.BAIRRO,
                                c.DATANASCIMENTO,
                                ci.ID       AS CIDADE_ID,
                                ci.NOME     AS CIDADE_NOME,
                                ci.ESTADOID,
                                e.ID        AS ESTADO_ID,
                                e.NOME      AS ESTADO_NOME,
                                e.UF
                            FROM dbo.CLIENTE c
                            JOIN dbo.CIDADE  ci ON ci.ID = c.CIDADE
                            JOIN dbo.ESTADO  e  ON e.ID  = ci.ESTADOID
                            ORDER BY c.ID ASC
                            """;

            var estadoCache = new Dictionary<int, Estado>();
            var cidadeCache = new Dictionary<int, Cidade>();

            var resultado = await connection.QueryAsync<Cliente, Cidade, Estado, Cliente>(
                sql,
                map: (cliente, cidade, estado) =>
                {
                    if (!estadoCache.TryGetValue(estado.Id, out var estadoMapeado))
                    {
                        estadoMapeado = new Estado(estado.Id, estado.Nome, estado.Uf);
                        estadoCache[estado.Id] = estadoMapeado;
                    }

                    if (!cidadeCache.TryGetValue(cidade.Id, out var cidadeMapeada))
                    {
                        cidadeMapeada = new Cidade(cidade.Id, cidade.Nome, estadoMapeado.Id, estadoMapeado);
                        cidadeCache[cidade.Id] = cidadeMapeada;
                    }

                    return new Cliente(
                        cliente.Id,
                        cliente.Nome,
                        cliente.CpfCnpj,
                        cidadeMapeada.Id,
                        cliente.Cep,
                        cliente.Endereco,
                        cliente.Numero,
                        cliente.Complemento,
                        cliente.Bairro,
                        cliente.DataNascimento,
                        cidadeMapeada
                    );
                },
                splitOn: "CIDADE_ID,ESTADO_ID"
            );

            return resultado.ToList();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();

            const string sql = "DELETE FROM dbo.CLIENTE WHERE ID = @Id";

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                            SELECT
                                c.ID,
                                c.NOME,
                                c.CEP,
                                c.CPF_CNPJ AS CpfCnpj,
                                c.ENDERECO,
                                c.NUMERO,
                                c.COMPLEMENTO,
                                c.BAIRRO,
                                c.CIDADE,
                                c.DATANASCIMENTO,
                                ci.ID       AS CIDADE_ID,
                                ci.NOME     AS CIDADE_NOME,
                                ci.ESTADOID,
                                e.ID        AS ESTADO_ID,
                                e.NOME      AS ESTADO_NOME,
                                e.UF
                            FROM dbo.CLIENTE c
                            JOIN dbo.CIDADE  ci ON ci.ID = c.CIDADE
                            JOIN dbo.ESTADO  e  ON e.ID  = ci.ESTADOID
                            WHERE c.ID = @Id
                            """;

            var result = await connection.QueryAsync<Cliente, Cidade, Estado, Cliente>(
                sql,
                map: (cliente, cidade, estado) =>
                {
                    cidade = new Cidade(cidade.Id, cidade.Nome, estado.Id, estado);
                    cliente = new Cliente(
                        cliente.Id,
                        cliente.Nome,
                        cliente.CpfCnpj,
                        cidade.Id,
                        cliente.Cep,
                        cliente.Endereco,
                        cliente.Numero,
                        cliente.Complemento,
                        cliente.Bairro,
                        cliente.DataNascimento,
                        cidade
                    );
                    return cliente;
                },
                param: new { Id = id },
                splitOn: "CIDADE_ID,ESTADO_ID"
            );

            return result.FirstOrDefault();
        }

        public async Task<int> SaveAsync(Cliente cliente)
        {
            using var connection = _context.CreateConnection();

            // INSERT
            if (cliente.Id == 0)
            {
                const string sql = """
                                        INSERT INTO dbo.CLIENTE
                                            (ID, NOME, CEP, CPF_CNPJ, ENDERECO, NUMERO, COMPLEMENTO, BAIRRO, CIDADE, DATANASCIMENTO)
                                        VALUES
                                            (NEXT VALUE FOR dbo.SEQ_CLIENTE_ID, @Nome, @Cep, @CpfCnpj, @Endereco, @Numero, @Complemento, @Bairro, @CidadeId, @DataNascimento);

                                        SELECT CAST(CURRENT_VALUE AS INT) 
                                                  FROM SYS.SEQUENCES 
                                                  WHERE NAME = 'SEQ_CLIENTE_ID';
                                        """;

                return connection.QuerySingle<int>(sql, cliente);
            }

            // UPDATE
            const string sqlUpdate = """
                                        UPDATE dbo.CLIENTE SET
                                            NOME           = @Nome,
                                            CEP            = @Cep,
                                            CPF_CNPJ       = @CpfCnpj,
                                            ENDERECO       = @Endereco,
                                            NUMERO         = @Numero,
                                            COMPLEMENTO    = @Complemento,
                                            BAIRRO         = @Bairro,
                                            CIDADE         = @CidadeId,
                                            DATANASCIMENTO = @DataNascimento
                                        WHERE ID = @Id
                                        """;

            await connection.ExecuteAsync(sqlUpdate, cliente);
            return cliente.Id;
        }

        public async Task<IEnumerable<ClientReportDto>> GetReportAsync(ClientReportFilterDto filter)
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                                SELECT
                                    c.ID,
                                    c.NOME,
                                    c.CPF_CNPJ  AS CpfCnpj,
                                    c.CEP,
                                    c.BAIRRO,
                                    ci.NOME     AS Cidade,
                                    e.NOME      AS Estado
                                FROM dbo.CLIENTE c
                                JOIN dbo.CIDADE  ci ON ci.ID = c.CIDADE
                                JOIN dbo.ESTADO  e  ON e.ID  = ci.ESTADOID
                                WHERE
                                    (@Todos = 1
                                        OR
                                        (
                                            (@IdInicial IS NULL OR c.ID >= @IdInicial)
                                            AND (@IdFinal   IS NULL OR c.ID <= @IdFinal)
                                            AND (@CidadeId  IS NULL OR c.CIDADE    = @CidadeId)
                                            AND (@EstadoId  IS NULL OR ci.ESTADOID = @EstadoId)
                                        )
                                    )
                                ORDER BY c.ID ASC
                                """;

            return await connection.QueryAsync<ClientReportDto>(sql, new
            {
                Todos = filter.Todos ? 1 : 0,
                IdInicial = filter.IdInicial,
                IdFinal = filter.IdFinal,
                CidadeId = filter.CidadeId,
                EstadoId = filter.EstadoId
            });
        }
    }
}
