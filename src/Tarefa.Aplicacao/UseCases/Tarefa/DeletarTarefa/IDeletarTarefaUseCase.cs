
namespace Tarefa.Aplicacao.UseCases.Tarefa.DeletarTarefa;

public interface IDeletarTarefaUseCase
{
    Task ExecuteAsync(int tarefaId, int usuarioId);
}
