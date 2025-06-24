using AutoMapper;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasConcluidas;

public class ObterTarefasConcluidasUseCase : IObterTarefasConcluidasUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public ObterTarefasConcluidasUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<List<TarefaResponse>> ExecuteAsync(int usuarioId)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        var tarefasConcluidas = await _tarefaRepository.ObterTarefasConcluidasAsync(usuarioId);
        return _mapper.Map<List<TarefaResponse>>(tarefasConcluidas);
    }
}
