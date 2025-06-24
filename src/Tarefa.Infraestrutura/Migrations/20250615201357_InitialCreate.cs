using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tarefa.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Concluida = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Prioridade = table.Column<int>(type: "integer", nullable: false),
                    DataLembrete = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LembreteEnviado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarefas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "DataAtualizacao", "DataCriacao", "Email", "Nome", "SenhaHash" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@teste.com", "Administrador", "$2a$11$5Ugj78AjWYTtCSF60cJQ1.DFM88D01VA3cYXO3mU4gDiPJTqKVUJS" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "joao@teste.com", "João Silva", "$2a$11$Px5lZvitdt4NjLObT2JbWuPx/PjmAr.jkvMM6ttaVYbQTnadCYaqa" }
                });

            migrationBuilder.InsertData(
                table: "Tarefas",
                columns: new[] { "Id", "DataAtualizacao", "DataConclusao", "DataCriacao", "DataLembrete", "Descricao", "Prioridade", "Titulo", "UsuarioId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 31, 18, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 30, 16, 0, 0, 0, DateTimeKind.Utc), "Revisar conceitos de DDD, SOLID e implementar projeto", 3, "Estudar Clean Architecture", 2 },
                    { 2, new DateTime(2024, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 25, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), null, "Criar interface responsiva para gerenciar tarefas", 2, "Implementar Frontend React", 2 }
                });

            migrationBuilder.InsertData(
                table: "Tarefas",
                columns: new[] { "Id", "Concluida", "DataAtualizacao", "DataConclusao", "DataCriacao", "DataLembrete", "Descricao", "Prioridade", "Titulo", "UsuarioId" },
                values: new object[] { 3, true, new DateTime(2024, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 20, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), null, "Configurar CI/CD e fazer deploy no Heroku", 4, "Deploy da Aplicação", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Concluida",
                table: "Tarefas",
                column: "Concluida");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_DataConclusao",
                table: "Tarefas",
                column: "DataConclusao");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_DataLembrete_LembreteEnviado",
                table: "Tarefas",
                columns: new[] { "DataLembrete", "LembreteEnviado" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Prioridade",
                table: "Tarefas",
                column: "Prioridade");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UsuarioId",
                table: "Tarefas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UsuarioId_Concluida",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "Concluida" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
