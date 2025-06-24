using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tarefa.Domain.Entidades;

namespace Tarefa.Infraestrutura.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Usuarios usuario)
    {
        // ✅ CORRIGIR: Usar as mesmas chaves que Autenticacao.cs
        var jwtKey = _configuration["JwtSettings:Secret"]
                    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                    ?? throw new InvalidOperationException("JWT Key não encontrada");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim("nameid", usuario.Id.ToString()) // Para compatibilidade
        };

        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes", 60);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"] ?? "TarefaAPI",
            audience: _configuration["JwtSettings:Audience"] ?? "TarefaAPI",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes), // ✅ Usar configuração
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}