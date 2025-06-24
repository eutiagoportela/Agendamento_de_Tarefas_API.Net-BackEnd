
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefa;
public interface IObterTarefaUseCase
{
    Task<TarefaResponse> ExecuteAsync(int tarefaId, int usuarioId);
}
