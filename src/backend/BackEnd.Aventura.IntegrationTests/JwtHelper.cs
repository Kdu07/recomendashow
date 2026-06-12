using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BackEnd.Aventura.ApiTests.Fixtures;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Aventura.ApiTests.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(Guid userId, string email = "test@aventura.com")
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFixture.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("name", "Teste"),
        };
        var token = new JwtSecurityToken(
            issuer:             ApiFixture.JwtIssuer,
            audience:           ApiFixture.JwtAudience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static void SetBearerToken(this HttpClient client, Guid userId)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GenerateToken(userId));
    }
}