
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoConcluida;

public interface IMarcarTarefaComoConcluidaUseCase
{
    Task<TarefaResponse> ExecuteAsync(int tarefaId, int usuarioId);
}
