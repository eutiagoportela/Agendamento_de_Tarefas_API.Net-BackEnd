using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Infraestrutura.Repositorios.Implementacoes;

namespace Tarefa.Infraestrutura;

public static class PostgreSqlConfig
{
    public static IServiceCollection AddPostgreSqlConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);

        services.AddDbContext<PostgreSqlDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITarefaRepository, TarefaRepository>();

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        // 1. Tentar connection string normal (Development)
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 2. Se não tem, tentar DATABASE_URL (Heroku)
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            // 3. Converter formato Heroku para .NET
            if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres://"))
            {
                connectionString = ConvertHerokuConnectionString(connectionString);
            }
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string não encontrada");
        }

        return connectionString;
    }

    private static string ConvertHerokuConnectionString(string herokuConnectionString)
    {
        var uri = new Uri(herokuConnectionString);
        var userInfo = uri.UserInfo.Split(':');

        return $"Host={uri.Host};Database={uri.AbsolutePath.Trim('/')};Username={userInfo[0]};Password={userInfo[1]};Port={uri.Port};SSL Mode=Require;Trust Server Certificate=true";
    }
}