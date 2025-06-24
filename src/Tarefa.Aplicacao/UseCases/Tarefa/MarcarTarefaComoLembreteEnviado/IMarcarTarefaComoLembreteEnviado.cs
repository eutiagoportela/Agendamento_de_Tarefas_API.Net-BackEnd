
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoLembreteEnviado;

public interface IMarcarTarefaComoLembreteEnviado
{
    Task<TarefaResponse> ExecuteAsync(int tarefaId, int usuarioId);
}
