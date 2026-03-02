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
                            ORDER BY c.NOME ASC
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

        // ClienteRepository
        public async Task<IEnumerable<Cliente>> SearchAsync(CustomerFilterDto filtro)
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                                SELECT
                                    v.ID, v.NOME, v.CPF_CNPJ AS CpfCnpj, v.CEP,
                                    v.ENDERECO, v.NUMERO, v.COMPLEMENTO, v.BAIRRO,
                                    v.CIDADE_ID, v.CIDADE_NOME,
                                    v.ESTADO_ID, v.ESTADO_NOME, v.ESTADO_UF,
                                    v.DATANASCIMENTO
                                FROM dbo.vw_ClienteCompleto v
                                WHERE
                                    (@Id             IS NULL OR CAST(v.ID AS VARCHAR) LIKE @Id             + '%')
                                    AND (@Nome        IS NULL OR v.NOME                LIKE @Nome           + '%')
                                    AND (@CpfCnpj     IS NULL OR v.CPF_CNPJ            LIKE @CpfCnpj        + '%')
                                    AND (@Cep         IS NULL OR v.CEP                 LIKE @Cep            + '%')
                                    AND (@Cidade      IS NULL OR v.CIDADE_NOME         LIKE @Cidade         + '%')
                                    AND (@Estado      IS NULL OR v.ESTADO_NOME         LIKE @Estado         + '%'
                                                             OR v.ESTADO_UF            LIKE @Estado         + '%')
                                    AND (@DataNasc    IS NULL OR CONVERT(VARCHAR, v.DATANASCIMENTO, 103) LIKE @DataNasc + '%')
                                ORDER BY v.NOME ASC
                                """;

            return await connection.QueryAsync<Cliente>(sql, new
            {
                Id = string.IsNullOrWhiteSpace(filtro.Id) ? null : filtro.Id,
                Nome = string.IsNullOrWhiteSpace(filtro.Nome) ? null : filtro.Nome,
                CpfCnpj = string.IsNullOrWhiteSpace(filtro.CpfCnpj) ? null : filtro.CpfCnpj,
                Cep = string.IsNullOrWhiteSpace(filtro.Cep) ? null : filtro.Cep,
                Cidade = string.IsNullOrWhiteSpace(filtro.Cidade) ? null : filtro.Cidade,
                Estado = string.IsNullOrWhiteSpace(filtro.Estado) ? null : filtro.Estado,
                DataNasc = string.IsNullOrWhiteSpace(filtro.DataNascimento) ? null : filtro.DataNascimento
            });
        }
    }
}
