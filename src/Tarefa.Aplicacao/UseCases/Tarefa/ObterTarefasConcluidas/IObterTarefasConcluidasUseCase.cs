
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasConcluidas;

public interface IObterTarefasConcluidasUseCase
{
    Task<List<TarefaResponse>> ExecuteAsync(int usuarioId);
}
