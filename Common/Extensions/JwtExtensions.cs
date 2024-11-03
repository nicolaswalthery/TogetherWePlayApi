using System.IdentityModel.Tokens.Jwt;
namespace Common.Extensions
{
    public static class JwtExtensions
    {
        public static JwtSecurityToken Decode(this string jwToken)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(jwToken);
        }

        public static DateTime GetExpirationDate(this string jwToken)
        {
            var tokendecoded = jwToken.Decode();
            var exp = tokendecoded.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var tokenTicks = long.Parse(exp);
            var tokenExpDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

            //var now = DateTime.Now.ToUniversalTime();
            return tokenExpDate;
        }
    }
}
