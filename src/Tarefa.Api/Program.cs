using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using Tarefa.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAR SERILOG ===== 
Log.Logger = new LoggerConfiguration()
    .CreateLogger();

// ===== REGISTRAR SERVIÇOS =====
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Mantém PascalCase
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Aceita enum como string também
    });

builder.Services.AddHealthChecks(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = @"Cabeçalho de autorização JWT usando o esquema Bearer. Insira 'Bearer' [espaço] e, em seguida, seu token no campo de texto abaixo. Exemplo: 'Bearer 12345abcdef'.",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Type = SecuritySchemeType.ApiKey
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// ===== DEPENDÊNCIAS DA APLICAÇÃO ===== ✅
Log.Information("Registrando dependências da aplicação...");
builder.Services.AddApplicationDependencies(builder.Configuration, builder.Environment);
Log.Information("Dependências registradas com sucesso!");

var app = builder.Build();

// ===== CONFIGURAR PIPELINE =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS AUTOMÁTICO BASEADO NO AMBIENTE
if (app.Environment.IsDevelopment())
{
    Log.Information("Aplicando CORS para DESENVOLVIMENTO");
    app.UseCors("Development");
}
else
{
    Log.Information("Aplicando CORS para PRODUÇÃO");
    app.UseCors("Production");
}


// ✅ (retorna JSON)
app.MapHealthChecks("/api/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            service = "TarefaAPI",
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Log.Information($"Aplicação iniciada em modo: {app.Environment.EnvironmentName}");
app.Run();