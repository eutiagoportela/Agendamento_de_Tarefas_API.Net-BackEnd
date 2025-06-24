
using Tarefa.Comunicacao.Requests.Tarefa;
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ListarTarefasPaginadas;

public interface IListarTarefasPaginadasUseCase
{
    Task<PaginatedResponse<TarefaResponse>> ExecuteAsync(int usuarioId, ListarTarefasRequest request);
}
