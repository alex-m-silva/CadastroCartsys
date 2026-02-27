using CadastroCartsys.Domain.Entities;
using Dapper;

namespace CadastroCartsys.Data.Mappings
{
    public static class ClienteMap
    {
        public static void Register()
        {
            SqlMapper.SetTypeMap(typeof(Cliente), new CustomPropertyTypeMap(
                typeof(Cliente), (type, columnName) => columnName.ToUpper() switch
                {
                    "ID" => type.GetProperty(nameof(Cliente.Id))!,
                    "NOME" => type.GetProperty(nameof(Cliente.Nome))!,
                    "CEP" => type.GetProperty(nameof(Cliente.Cep))!,
                    "CPF_CNPJ" => type.GetProperty(nameof(Cliente.CpfCnpj))!,
                    "ENDERECO" => type.GetProperty(nameof(Cliente.Endereco))!,
                    "NUMERO" => type.GetProperty(nameof(Cliente.Numero))!,
                    "COMPLEMENTO" => type.GetProperty(nameof(Cliente.Complemento))!,
                    "BAIRRO" => type.GetProperty(nameof(Cliente.Bairro))!,
                    "CIDADE" => type.GetProperty(nameof(Cliente.CidadeId))!,
                    "DATANASCIMENTO" => type.GetProperty(nameof(Cliente.DataNascimento))!,
                    _ => null!
                }
            ));
        }
    }
}
