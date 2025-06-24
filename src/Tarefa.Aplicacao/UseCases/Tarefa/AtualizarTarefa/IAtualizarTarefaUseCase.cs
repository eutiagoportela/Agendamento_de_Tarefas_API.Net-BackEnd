
using Tarefa.Comunicacao.Requests;
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.AtualizarTarefa;

public interface IAtualizarTarefaUseCase
{
    Task<TarefaResponse> ExecuteAsync(int tarefaId, AtualizarTarefaRequest request, int usuarioId);
}
