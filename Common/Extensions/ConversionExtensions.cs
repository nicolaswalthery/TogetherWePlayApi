using Common.Exceptions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Extensions
{
    public static class ConversionExtensions
    {
        public static Guid ToGuid(this string str)
            => Guid.TryParse(str, out Guid result) ? result : throw new ConversionException("Conversion from string to guid Exception");

        public static string ToJson<T>(this T dto) 
            => JsonSerializer.Serialize(dto);

        public static string ToJson<T>(this T dto, JsonIgnoreCondition defaultIgnoreCondition)
            => JsonSerializer.Serialize(dto, options: new JsonSerializerOptions { DefaultIgnoreCondition = defaultIgnoreCondition });

        public static Uri ToUri(this string path) 
            => new Uri(path);

        public static StringContent ToHttpContent<T>(this T dto) 
            => new StringContent(dto.ToJson(), Encoding.UTF8, "application/json");

        /// <summary>
        /// Take a nullable boolean in the entry parameter If it is null the method return false, if not the value of the entry parameter.
        /// </summary>
        /// <param name="boolean">The boolean.</param>
        /// <returns></returns>
        public static bool ToNonNullableBool(this bool? boolean)
            => boolean is not null ? (bool)boolean : false;

        public static int ToInt(this string str) 
            => int.TryParse(str, out int result) ? result : throw new ConversionException("Can't parse string to int");
    }
}
