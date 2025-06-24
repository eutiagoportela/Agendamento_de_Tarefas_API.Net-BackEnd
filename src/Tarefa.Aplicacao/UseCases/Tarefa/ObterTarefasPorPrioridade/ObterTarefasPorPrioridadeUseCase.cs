using AutoMapper;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Enum;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPorPrioridade;

public class ObterTarefasPorPrioridadeUseCase : IObterTarefasPorPrioridadeUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public ObterTarefasPorPrioridadeUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<List<TarefaResponse>> ExecuteAsync(int usuarioId, PrioridadeTarefa prioridade)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        var tarefas = await _tarefaRepository.ObterTarefasPorPrioridadeAsync(usuarioId, prioridade);
        return _mapper.Map<List<TarefaResponse>>(tarefas);
    }
}
