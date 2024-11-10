using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace TWP.Api.Infrastructure.Converters
{
    public class EnumListToStringConverter<T> : ValueConverter<List<T>, string>
        where T : Enum
    {
        public EnumListToStringConverter(ConverterMappingHints mappingHints = null)
            : base(ListEnumToString(), StringToListEnum(), mappingHints)
        {
        }

        private static Expression<Func<List<T>, string>> ListEnumToString() => v => string.Join(",", v);
        private static Expression<Func<string, List<T>>> StringToListEnum() => v => v != null ? v.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => (T)Enum.Parse(typeof(T), s)).ToList() : new List<T>();
    }
}
