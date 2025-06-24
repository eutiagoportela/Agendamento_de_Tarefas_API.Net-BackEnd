
using Tarefa.Comunicacao.Responses.Tarefa;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasUsuario;

public interface IObterTarefasUsuarioUseCase
{
    Task<List<TarefaResponse>> ExecuteAsync(int usuarioId);
}
