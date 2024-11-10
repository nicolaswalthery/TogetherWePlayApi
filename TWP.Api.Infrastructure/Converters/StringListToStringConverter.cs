using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace TWP.Api.Infrastructure.Converters
{
    public class StringListToStringConverter : ValueConverter<List<string>, string>
    {
        public StringListToStringConverter(ConverterMappingHints mappingHints = null)
            : base(ListStrToString(), StringToList(), mappingHints)
        {
        }

        private static Expression<Func<List<string>, string>> ListStrToString() => s => string.Join(",", s);
        private static Expression<Func<string, List<string>>> StringToList() => v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
