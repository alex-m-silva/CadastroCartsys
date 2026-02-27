using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using Dapper;
using System.Data;

namespace CadastroCartsys.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly DbContext _context;

        public ClientRepository(DbContext context)
        {
            _context = context;
        }

        public void Delete(int id)
        {
            using var connection = _context.CreateConnection();

            const string sql = "DELETE FROM dbo.CLIENTE WHERE ID = @Id";

            connection.Execute(sql, new { Id = id });
        }

        public Cliente? GetById(int id)
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                                    SELECT
                                        c.ID,
                                        c.NOME,
                                        c.CEP,
                                        c.CPF_CNPJ,
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

            return connection.Query<Cliente, Cidade, Estado, Cliente>(
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
            ).FirstOrDefault();
        }

        public int Save(Cliente cliente)
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

                                        SELECT CAST(SCOPE_IDENTITY() AS INT);
                                        """;

                return connection.ExecuteScalar<int>(sql, cliente);
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

            connection.Execute(sqlUpdate, cliente);
            return cliente.Id;
        }

        public IEnumerable<Cliente> Search(string campo, string termo)
        {
            using var connection = _context.CreateConnection();

            var sql = $"""
                            SELECT
                                v.ID,
                                v.NOME,
                                v.CPF_CNPJ,
                                v.CEP,
                                v.ENDERECO,
                                v.NUMERO,
                                v.COMPLEMENTO,
                                v.BAIRRO,
                                v.CIDADE_ID,
                                v.CIDADE_NOME,
                                v.ESTADO_ID,
                                v.ESTADO_NOME,
                                v.ESTADO_UF,
                                v.DATANASCIMENTO
                            FROM dbo.vw_ClienteCompleto v
                            WHERE v.{campo.ToUpper()} LIKE @Termo + '%'
                            ORDER BY v.NOME ASC
                            """;

            return connection.Query<Cliente>(sql, new { Termo = termo });
        }
    }
}
