using CadastroCartsys.Domain.Entities;
using Dapper;
using System.Reflection;

namespace CadastroCartsys.Data.Mappings
{
    public class EstadoMap
    {
        private readonly DefaultTypeMap _defaultMap = new(typeof(Estado));

        public static void Register()
        {
            SqlMapper.SetTypeMap(typeof(Estado), new CustomPropertyTypeMap(
                typeof(Estado), (type, columnName) => columnName.ToUpper() switch
                {
                    "ID" => type.GetProperty(nameof(Estado.Id))!,
                    "NOME" => type.GetProperty(nameof(Estado.Nome))!,
                    "UF" => type.GetProperty(nameof(Estado.Uf))!,
                    _ => null!
                }
            ));
        }

        public ConstructorInfo? FindConstructor(string[] names, Type[] types)
            => _defaultMap.FindConstructor(names, types);
        public ConstructorInfo? FindExplicitConstructor()
            => _defaultMap.FindExplicitConstructor();
        public SqlMapper.IMemberMap? GetConstructorParameter(ConstructorInfo constructor, string columnName)
            => _defaultMap.GetConstructorParameter(constructor, columnName);
        public SqlMapper.IMemberMap? GetMember(string columnName)
            => _defaultMap.GetMember(columnName);
    }
}
