
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Usuario.DeletarUsuario;

public class DeletarUsuarioUseCase : IDeletarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITarefaRepository _tarefaRepository;

    public DeletarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        ITarefaRepository tarefaRepository)
    {
        _usuarioRepository = usuarioRepository;
        _tarefaRepository = tarefaRepository;
    }

    public async Task ExecuteAsync(int usuarioId)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        // Deletar todas as tarefas do usuário primeiro (devido às foreign keys)
        var tarefasDoUsuario = await _tarefaRepository.ObterPorUsuarioAsync(usuarioId);
        foreach (var tarefa in tarefasDoUsuario)
        {
            await _tarefaRepository.DeletarAsync(tarefa.Id);
        }

        // Deletar usuário
        await _usuarioRepository.DeletarAsync(usuarioId);
    }
}
