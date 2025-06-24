
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.DeletarTarefa;

public class DeletarTarefaUseCase : IDeletarTarefaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public DeletarTarefaUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task ExecuteAsync(int tarefaId, int usuarioId)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        // Verificar se a tarefa existe e pertence ao usuário
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
        if (tarefa == null)
            throw new TarefaNaoEncontradaException();

        if (tarefa.UsuarioId != usuarioId)
            throw new AcessoNegadoException("Usuário não tem permissão para deletar esta tarefa");

        // Deletar tarefa
        await _tarefaRepository.DeletarAsync(tarefaId);
    }
}
