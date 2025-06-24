using AutoMapper;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoConcluida;

public class MarcarTarefaComoConcluidaUseCase : IMarcarTarefaComoConcluidaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public MarcarTarefaComoConcluidaUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<TarefaResponse> ExecuteAsync(int tarefaId, int usuarioId)
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
            throw new AcessoNegadoException("Usuário não tem permissão para marcar esta tarefa como concluída");

        // Marcar como concluída usando método da entidade
        tarefa.MarcarComoConcluida();

        var tarefaAtualizada = await _tarefaRepository.AtualizarAsync(tarefa);
        return _mapper.Map<TarefaResponse>(tarefaAtualizada);
    }
}
