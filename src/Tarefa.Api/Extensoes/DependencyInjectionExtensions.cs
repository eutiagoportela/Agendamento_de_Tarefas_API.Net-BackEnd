using Microsoft.EntityFrameworkCore;
using Tarefa.Aplicacao.AutoMapper;
using Tarefa.Aplicacao.UseCases.Login.DoLogin;

// ===== USE CASES DE USUÁRIO =====
using Tarefa.Aplicacao.UseCases.Usuario.CriarUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.ObterUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.AtualizarUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.DeletarUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.ObterPerfilUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.VerificarEmailExiste;

// ===== USE CASES DE TAREFA =====
using Tarefa.Aplicacao.UseCases.Tarefa.CriarTarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.AtualizarTarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.DeletarTarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasUsuario;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPendentes;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPorPrioridade;
using Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoConcluida;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasConcluidas;
using Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoLembreteEnviado;
using Tarefa.Aplicacao.UseCases.Tarefa.ListarTarefasPaginadas;

using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Infraestrutura;
using Tarefa.Infraestrutura.Repositorios.Implementacoes;
using Tarefa.Infraestrutura.Security;

namespace Tarefa.Api.Extensions;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registra o banco de dados PostgreSQL
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<PostgreSqlDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    /// <summary>
    /// Registra o AutoMapper com validação
    /// </summary>
    public static IServiceCollection AddAutoMapperWithValidation(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        // Validação do AutoMapper (recomendado em desenvolvimento)
        var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>());
        mapperConfig.AssertConfigurationIsValid();

        return services;
    }

    /// <summary>
    /// Registra todos os repositórios
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITarefaRepository, TarefaRepository>();

        return services;
    }

    /// <summary>
    /// Registra serviços de segurança e infraestrutura
    /// </summary>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        // Registrar serviços de segurança
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Cache para otimização
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Registra todos os Use Cases de Usuário
    /// </summary>
    public static IServiceCollection AddUsuarioUseCases(this IServiceCollection services)
    {
        // ===== CRUD BÁSICO =====
        services.AddScoped<ICriarUsuarioUseCase, CriarUsuarioUseCase>();
        services.AddScoped<IObterUsuarioUseCase, ObterUsuarioUseCase>();
        services.AddScoped<IAtualizarUsuarioUseCase, AtualizarUsuarioUseCase>();
        services.AddScoped<IDeletarUsuarioUseCase, DeletarUsuarioUseCase>();

        // ===== FUNCIONALIDADES ESPECÍFICAS =====
        services.AddScoped<IObterPerfilUsuarioUseCase, ObterPerfilUsuarioUseCase>();
        services.AddScoped<IVerificarEmailExisteUseCase, VerificarEmailExisteUseCase>();

        return services;
    }

    /// <summary>
    /// Registra todos os Use Cases de Tarefa
    /// </summary>
    public static IServiceCollection AddTarefaUseCases(this IServiceCollection services)
    {
        // ===== CRUD BÁSICO =====
        services.AddScoped<ICriarTarefaUseCase, CriarTarefaUseCase>();
        services.AddScoped<IObterTarefaUseCase, ObterTarefaUseCase>();
        services.AddScoped<IAtualizarTarefaUseCase, AtualizarTarefaUseCase>();
        services.AddScoped<IDeletarTarefaUseCase, DeletarTarefaUseCase>();

        // ===== CONSULTAS E FILTROS =====
        services.AddScoped<IObterTarefasUsuarioUseCase, ObterTarefasUsuarioUseCase>();
        services.AddScoped<IObterTarefasPendentesUseCase, ObterTarefasPendentesUseCase>();
        services.AddScoped<IObterTarefasPorPrioridadeUseCase, ObterTarefasPorPrioridadeUseCase>();
        services.AddScoped<IObterTarefasConcluidasUseCase, ObterTarefasConcluidasUseCase>();

        // ===== PAGINAÇÃO =====
        services.AddScoped<IListarTarefasPaginadasUseCase, ListarTarefasPaginadasUseCase>();

        // ===== AÇÕES ESPECÍFICAS =====
        services.AddScoped<IMarcarTarefaComoConcluidaUseCase, MarcarTarefaComoConcluidaUseCase>();
        services.AddScoped<IMarcarTarefaComoLembreteEnviado, MarcarTarefaComoLembreteEnviado>();

        return services;
    }

    /// <summary>
    /// Registra Use Cases de Autenticação
    /// </summary>
    public static IServiceCollection AddAuthUseCases(this IServiceCollection services)
    {
        services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();

        return services;
    }

    /// <summary>
    /// Registra todos os Use Cases
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddUsuarioUseCases();
        services.AddTarefaUseCases();
        services.AddAuthUseCases();

        return services;
    }

    /// <summary>
    /// Configura CORS para desenvolvimento e produção
    /// </summary>
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            // ===== DESENVOLVIMENTO =====
            options.AddPolicy("Development", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            // ===== PRODUÇÃO =====
            options.AddPolicy("Production", builder =>
            {
                builder.WithOrigins(
                    // Local development
                    "http://localhost:3000",    // React
                    "http://localhost:5173",    // Vite (React/Vue)
                    "http://localhost:8080",    // Vue CLI

                    // Production hosting
                    "https://your-react-app.vercel.app",    // React no Vercel
                    "https://your-vue-app.netlify.app"      // Vue no Netlify
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// 🔔 SISTEMA DE LEMBRETES - BACKGROUND SERVICE
    /// 
    /// ✅ IMPLEMENTADO: Código completo do LembreteService disponível
    /// ⚠️ DESABILITADO: Para demonstração, usando frontend interativo
    /// 
    /// 💼 DECISÃO ARQUITETURAL:
    /// - DEMO: Frontend interativo (visual, demonstrável)
    /// - PRODUÇÃO: Background service (escalável, offline)
    /// 
    /// 🚀 Para habilitar: descomente a linha services.AddHostedService<LembreteService>();
    /// </summary>
    public static IServiceCollection AddLembreteService(this IServiceCollection services)
    {
        // ❌ COMENTADO PARA DEMONSTRAÇÃO - Frontend interativo é mais visual
        // services.AddHostedService<LembreteService>();

        // ✅ Log explicativo para demonstração
        Console.WriteLine("🔔 LembreteService: Disponível mas desabilitado para demo (frontend interativo ativo)");

        return services;
    }

    /// <summary>
    /// Registra todas as dependências da aplicação
    /// </summary>
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // ===== 1. PERSISTÊNCIA =====
        services.AddDatabase(configuration);
        services.AddRepositories();

        // ===== 2. MAPEAMENTO =====
        services.AddAutoMapperWithValidation();

        // ===== 3. SEGURANÇA =====
        services.AddSecurityServices();
        services.AddJwtAuthentication(configuration, environment);

        // ===== 4. REGRAS DE NEGÓCIO =====
        services.AddUseCases();

        // ===== 5. CORS =====
        services.AddCorsConfiguration();

        // ===== 6. SISTEMA DE LEMBRETES =====
        /*
         * 🔔 LEMBRETE STRATEGY:
         * 
         * ✅ IMPLEMENTADO: Background service completo (LembreteService.cs)
         * ⚠️ MODO DEMO: Desabilitado - Frontend interativo está ativo
         * 
         * 💡 PARA HABILITAR: Descomente internamente no AddLembreteService()
         * 🎯 PRODUÇÃO: Combinaria ambas as abordagens
         */
        services.AddLembreteService(); // ⬅️ Vai estar comentado internamente

        return services;
    }

    /// <summary>
    /// 📋 ALTERNATIVA: Registrar dependências SEM background service
    /// </summary>
    public static IServiceCollection AddApplicationDependenciesForDemo(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // ===== TUDO IGUAL, MAS SEM LEMBRETE SERVICE =====
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddAutoMapperWithValidation();
        services.AddSecurityServices();
        services.AddJwtAuthentication(configuration, environment);
        services.AddUseCases();
        services.AddCorsConfiguration();

        // ❌ NÃO REGISTRA LEMBRETE SERVICE
        Console.WriteLine("🔔 Sistema de Lembretes: Frontend interativo ativo (Background service disponível mas desabilitado para demo)");

        return services;
    }

    /// <summary>
    /// Use Cases essenciais
    /// </summary>
    public static IServiceCollection AddEssentialDependencies(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // ===== ESSENCIAIS PARA O DESAFIO =====
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddAutoMapperWithValidation();
        services.AddSecurityServices();
        services.AddJwtAuthentication(configuration, environment);

        // ===== USE CASES MÍNIMOS =====
        // Usuário
        services.AddScoped<ICriarUsuarioUseCase, CriarUsuarioUseCase>();
        services.AddScoped<IObterUsuarioUseCase, ObterUsuarioUseCase>();
        services.AddScoped<IAtualizarUsuarioUseCase, AtualizarUsuarioUseCase>();
        services.AddScoped<IDeletarUsuarioUseCase, DeletarUsuarioUseCase>();

        // Tarefa
        services.AddScoped<ICriarTarefaUseCase, CriarTarefaUseCase>();
        services.AddScoped<IObterTarefaUseCase, ObterTarefaUseCase>();
        services.AddScoped<IAtualizarTarefaUseCase, AtualizarTarefaUseCase>();
        services.AddScoped<IDeletarTarefaUseCase, DeletarTarefaUseCase>();
        services.AddScoped<IObterTarefasUsuarioUseCase, ObterTarefasUsuarioUseCase>();
        services.AddScoped<IObterTarefasPendentesUseCase, ObterTarefasPendentesUseCase>();
        services.AddScoped<IObterTarefasPorPrioridadeUseCase, ObterTarefasPorPrioridadeUseCase>();
        services.AddScoped<IMarcarTarefaComoConcluidaUseCase, MarcarTarefaComoConcluidaUseCase>();
        services.AddScoped<IMarcarTarefaComoLembreteEnviado, MarcarTarefaComoLembreteEnviado>();
        services.AddScoped<IListarTarefasPaginadasUseCase, ListarTarefasPaginadasUseCase>();
        services.AddScoped<IObterTarefasConcluidasUseCase, ObterTarefasConcluidasUseCase>();

        // Auth
        services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();

        // CORS básico
        services.AddCorsConfiguration();

        // ⚠️ LEMBRETE SERVICE: Comentado para demo
        services.AddLembreteService();

        return services;
    }

    /// <summary>
    /// 🔧 HELPER - Validar se todas as dependências estão registradas
    /// </summary>
    public static void ValidateDependencies(this IServiceProvider serviceProvider)
    {
        try
        {
            // ===== TESTAR REPOSITÓRIOS =====
            serviceProvider.GetRequiredService<IUsuarioRepository>();
            serviceProvider.GetRequiredService<ITarefaRepository>();

            // ===== TESTAR SEGURANÇA =====
            serviceProvider.GetRequiredService<IPasswordHasher>();
            serviceProvider.GetRequiredService<IJwtTokenGenerator>();

            // ===== TESTAR USE CASES ESSENCIAIS =====
            serviceProvider.GetRequiredService<ICriarUsuarioUseCase>();
            serviceProvider.GetRequiredService<IDoLoginUseCase>();
            serviceProvider.GetRequiredService<ICriarTarefaUseCase>();
            serviceProvider.GetRequiredService<IMarcarTarefaComoConcluidaUseCase>();
            serviceProvider.GetRequiredService<IMarcarTarefaComoLembreteEnviado>();
            serviceProvider.GetRequiredService<IListarTarefasPaginadasUseCase>();

            Console.WriteLine("✅ Todas as dependências estão registradas corretamente!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro na validação de dependências: {ex.Message}");
            throw;
        }
    }
}