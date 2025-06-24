using Microsoft.EntityFrameworkCore;
using Tarefa.Domain.Entidades;
using Tarefa.Domain.Enum;

public class PostgreSqlDbContext : DbContext
{
    public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) : base(options)
    {
    }

    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Tarefas> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== CONFIGURAÇÕES DAS ENTIDADES =====
        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).IsRequired();
            entity.Property(u => u.DataCriacao).IsRequired();
            entity.Property(u => u.DataAtualizacao).IsRequired();

            entity.HasMany(u => u.Tarefas)
                  .WithOne(t => t.Usuario)
                  .HasForeignKey(t => t.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tarefas>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Descricao).HasMaxLength(1000);
            entity.Property(t => t.DataCriacao).IsRequired();
            entity.Property(t => t.DataAtualizacao).IsRequired();
            entity.Property(t => t.DataConclusao).IsRequired();
            entity.Property(t => t.Prioridade).IsRequired();
            entity.Property(t => t.Concluida).IsRequired().HasDefaultValue(false);
            entity.Property(t => t.LembreteEnviado).IsRequired().HasDefaultValue(false);
            entity.Property(t => t.UsuarioId).IsRequired();

            entity.Property(t => t.Prioridade)
                  .HasConversion<int>();

            // Índices para performance
            entity.HasIndex(t => t.UsuarioId);
            entity.HasIndex(t => t.Concluida);
            entity.HasIndex(t => t.DataConclusao);
            entity.HasIndex(t => t.Prioridade);
            entity.HasIndex(t => new { t.UsuarioId, t.Concluida });
            entity.HasIndex(t => new { t.DataLembrete, t.LembreteEnviado });
        });

        // ===== 🌱 SEED DATA - USUÁRIOS DE TESTE =====
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // ===== USUÁRIO ADMINISTRADOR =====
        modelBuilder.Entity<Usuarios>().HasData(
            new Usuarios
            {
                Id = 1,
                Nome = "Administrador",
                Email = "admin@teste.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"), // Senha: 123456
                DataCriacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // ===== USUÁRIO DE TESTE =====
        modelBuilder.Entity<Usuarios>().HasData(
            new Usuarios
            {
                Id = 2,
                Nome = "João Silva",
                Email = "joao@teste.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"), // Senha: 123456
                DataCriacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // ===== TAREFAS DE EXEMPLO =====
        modelBuilder.Entity<Tarefas>().HasData(
            new Tarefas
            {
                Id = 1,
                Titulo = "Estudar Clean Architecture",
                Descricao = "Revisar conceitos de DDD, SOLID e implementar projeto",
                DataConclusao = new DateTime(2024, 12, 31, 18, 0, 0, DateTimeKind.Utc),
                Prioridade = PrioridadeTarefa.Alta,
                Concluida = false,
                DataLembrete = new DateTime(2024, 12, 30, 16, 0, 0, DateTimeKind.Utc),
                LembreteEnviado = false,
                UsuarioId = 2, // João Silva
                DataCriacao = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            },
            new Tarefas
            {
                Id = 2,
                Titulo = "Implementar Frontend React",
                Descricao = "Criar interface responsiva para gerenciar tarefas",
                DataConclusao = new DateTime(2024, 12, 25, 20, 0, 0, DateTimeKind.Utc),
                Prioridade = PrioridadeTarefa.Media,
                Concluida = false,
                DataLembrete = null,
                LembreteEnviado = false,
                UsuarioId = 2, // João Silva
                DataCriacao = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc)
            },
            new Tarefas
            {
                Id = 3,
                Titulo = "Deploy da Aplicação",
                Descricao = "Configurar CI/CD e fazer deploy no Heroku",
                DataConclusao = new DateTime(2024, 12, 20, 15, 0, 0, DateTimeKind.Utc),
                Prioridade = PrioridadeTarefa.Urgente,
                Concluida = true, // Já concluída
                DataLembrete = null,
                LembreteEnviado = false,
                UsuarioId = 1, // Admin
                DataCriacao = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc),
                DataAtualizacao = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}

// ===== 🔧 COMMANDS PARA APLICAR SEED DATA =====

/*
1. Gerar nova migration com seed data:
   dotnet ef migrations add SeedData --project ../Tarefa.Infraestrutura

2. Aplicar no banco:
   dotnet ef database update --project ../Tarefa.Infraestrutura

3. Verificar dados:
   docker exec -it tarefa-postgres psql -U usuario123 -d tarefadb -c "SELECT * FROM \"Usuarios\";"
   
4. Testar login:
   Email: admin@teste.com
   Senha: 123456
   
   Email: joao@teste.com  
   Senha: 123456
*/