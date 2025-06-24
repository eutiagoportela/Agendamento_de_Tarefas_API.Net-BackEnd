using Tarefa.Domain.Entidades;
using Tarefa.Domain.Enum;

namespace Tarefa.Domain.Repositorios.Interfaces;

public interface ITarefaRepository
{

    Task<Tarefas?> ObterPorIdAsync(int id);
    Task<List<Tarefas>> ObterPorUsuarioAsync(int usuarioId);
    Task<Tarefas> CriarAsync(Tarefas tarefa);
    Task<Tarefas> AtualizarAsync(Tarefas tarefa);
    Task DeletarAsync(int id);
    Task<List<Tarefas>> ObterTarefasPorPrioridadeAsync(int usuarioId, PrioridadeTarefa prioridade);
    Task<List<Tarefas>> ObterTarefasPendentesAsync(int usuarioId);
    Task<List<Tarefas>> ObterTarefasComLembreteAsync();
    Task MarcarLembreteEnviadoAsync(int tarefaId);
    Task<List<Tarefas>> ObterTarefasConcluidasAsync(int usuarioId);

    Task<(List<Tarefas> tarefas, int totalCount)> ObterTarefasComPaginacaoAsync(
        int usuarioId,
        int skip,
        int take,
        string? searchTerm = null,
        bool? concluida = null,
        PrioridadeTarefa? prioridade = null);
}
