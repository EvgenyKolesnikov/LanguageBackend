using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Language.JwtExtensions;

public static class JwtExtensions
{
    public static string? GetClaimFromJwtToken(this string? bearerToken, string claimType)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
            return null;

        const string bearerPrefix = "Bearer ";

        if (!bearerToken.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            return null;

        var token = bearerToken.Substring(bearerPrefix.Length).Trim();

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken? jwt;

        try
        {
            jwt = handler.ReadJwtToken(token);
        }
        catch
        {
            return null; // токен невалиден
        }

        var userId = jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

        return userId;
    }
}