using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Infrastructure.Comparers;
using TWP.Api.Infrastructure.Converters;

namespace TWP.Api.Infrastructure.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<List<T>> HasEnumListConversion<T>(this PropertyBuilder<List<T>> source)
            where T : Enum
        {
            source.HasConversion(new EnumListToStringConverter<T>());
            source.Metadata.SetValueComparer(new EnumListComparer<T>());

            return source;
        }
        public static PropertyBuilder<List<string>> HasStringListConversion(this PropertyBuilder<List<string>> source)
        {
            source.HasConversion(new StringListToStringConverter());
            source.Metadata.SetValueComparer(new StringListToStringComparer());

            return source;
        }
    }
}
