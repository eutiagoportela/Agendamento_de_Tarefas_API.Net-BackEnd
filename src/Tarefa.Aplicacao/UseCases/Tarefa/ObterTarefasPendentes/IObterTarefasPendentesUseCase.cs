
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPendentes;

public interface IObterTarefasPendentesUseCase
{
    Task<List<TarefaResponse>> ExecuteAsync(int usuarioId);
}
