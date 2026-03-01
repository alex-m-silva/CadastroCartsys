using CadastroCartsys.Domain.Entities;
using Dapper;

namespace CadastroCartsys.Data.Mappings
{
    public static class CidadeMap
    {
        public static void Register()
        {
            SqlMapper.SetTypeMap(typeof(Cidade), new CustomPropertyTypeMap(
                typeof(Cidade), (type, columnName) => columnName.ToUpper() switch
                {
                    "ID" => type.GetProperty(nameof(Cidade.Id))!,
                    "CIDADE_ID" => type.GetProperty(nameof(Cidade.Id))!,    // ← alias splitOn
                    "NOME" => type.GetProperty(nameof(Cidade.Nome))!,
                    "CIDADE_NOME" => type.GetProperty(nameof(Cidade.Nome))!,  // ← alias
                    "ESTADOID" => type.GetProperty(nameof(Cidade.EstadoId))!,
                    _ => null!
                }
            ));
        }
    }
}
