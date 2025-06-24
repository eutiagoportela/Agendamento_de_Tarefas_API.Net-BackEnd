using AutoMapper;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPendentes;

public class ObterTarefasPendentesUseCase : IObterTarefasPendentesUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public ObterTarefasPendentesUseCase(
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
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        var tarefasPendentes = await _tarefaRepository.ObterTarefasPendentesAsync(usuarioId);
        return _mapper.Map<List<TarefaResponse>>(tarefasPendentes);
    }
}
