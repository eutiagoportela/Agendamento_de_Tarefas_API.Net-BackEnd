
using Tarefa.Comunicacao.Requests.Tarefa;
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.CriarTarefa;
public interface ICriarTarefaUseCase
{
    Task<TarefaResponse> ExecuteAsync(CriarTarefaRequest request, int usuarioId);
}
