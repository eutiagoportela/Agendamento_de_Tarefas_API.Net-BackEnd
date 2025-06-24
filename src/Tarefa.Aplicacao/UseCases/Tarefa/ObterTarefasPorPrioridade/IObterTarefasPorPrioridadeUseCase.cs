
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Enum;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPorPrioridade;

public interface IObterTarefasPorPrioridadeUseCase
{
    Task<List<TarefaResponse>> ExecuteAsync(int usuarioId, PrioridadeTarefa prioridade);
}
