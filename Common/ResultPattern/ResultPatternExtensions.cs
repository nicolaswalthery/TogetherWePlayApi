using System.Text.Json;

namespace Common.ResultPattern
{
    public static class ResultPatternExtensions
    {
        public static Result MapToResult(this string httpResponseContent)
            => JsonSerializer.Deserialize<Result>(httpResponseContent);

        public static T MapToResult<T>(this string httpResponseContent)
            => JsonSerializer.Deserialize<T>(httpResponseContent);
        
    }
}
