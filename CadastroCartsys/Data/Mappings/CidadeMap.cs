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
                    "NOME" => type.GetProperty(nameof(Cidade.Nome))!,
                    "ESTADOID" => type.GetProperty(nameof(Cidade.EstadoId))!,
                    _ => null!
                }
            ));
        }
    }
}
