using FluentAssertions;
using Tarefa.Domain.Enum;
using Xunit;

namespace Tarefa.Tests;

public class TarefaTest
{
    [Fact]
    public void CriarTarefa_DeveTerPropriedadesCorretas()
    {
        // Arrange & Act
        var tarefa = new Tarefas
        {
            Id = 1,
            Titulo = "Implementar testes",
            Descricao = "Criar testes unitários",
            DataConclusao = DateTime.UtcNow.AddDays(7),
            Prioridade = PrioridadeTarefa.Alta,
            Concluida = false,
            UsuarioId = 1,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };

        // Assert
        tarefa.Id.Should().Be(1);
        tarefa.Titulo.Should().Be("Implementar testes");
        tarefa.Descricao.Should().Be("Criar testes unitários");
        tarefa.Prioridade.Should().Be(PrioridadeTarefa.Alta);
        tarefa.Concluida.Should().BeFalse();
        tarefa.UsuarioId.Should().Be(1);
    }

    [Theory]
    [InlineData(PrioridadeTarefa.Baixa, 1)]
    [InlineData(PrioridadeTarefa.Media, 2)]
    [InlineData(PrioridadeTarefa.Alta, 3)]
    [InlineData(PrioridadeTarefa.Urgente, 4)]
    public void Prioridade_DeveCorresponderAoValorNumerico(PrioridadeTarefa prioridade, int valorEsperado)
    {
        // Act
        var valorNumerico = (int)prioridade;

        // Assert
        valorNumerico.Should().Be(valorEsperado);
    }

    [Fact]
    public void Tarefa_QuandoConcluida_DeveTerDataAtualizacao()
    {
        // Arrange
        var dataAntes = DateTime.UtcNow;
        var tarefa = new Tarefas
        {
            Titulo = "Tarefa teste",
            Concluida = false,
            DataCriacao = dataAntes
        };

        // Act - Simular conclusão
        tarefa.Concluida = true;
        tarefa.DataAtualizacao = DateTime.UtcNow;

        // Assert
        tarefa.Concluida.Should().BeTrue();
        tarefa.DataAtualizacao.Should().BeAfter(dataAntes);
    }
}
