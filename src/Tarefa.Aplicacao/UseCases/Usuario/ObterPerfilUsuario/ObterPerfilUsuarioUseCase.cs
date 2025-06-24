using AutoMapper;
using Tarefa.Comunicacao.Responses.Usuario;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Usuario.ObterPerfilUsuario;

public class ObterPerfilUsuarioUseCase : IObterPerfilUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IMapper _mapper;

    public ObterPerfilUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        ITarefaRepository tarefaRepository,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _tarefaRepository = tarefaRepository;
        _mapper = mapper;
    }

    public async Task<PerfilUsuarioResponse> ExecuteAsync(int usuarioId)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        // Obter estatísticas das tarefas
        var todasTarefas = await _tarefaRepository.ObterPorUsuarioAsync(usuarioId);
        var tarefasPendentes = await _tarefaRepository.ObterTarefasPendentesAsync(usuarioId);

        var totalTarefas = todasTarefas.Count;
        var totalPendentes = tarefasPendentes.Count;
        var totalConcluidas = totalTarefas - totalPendentes;

        // Criar resposta com dados do usuário e estatísticas
        var perfil = new PerfilUsuarioResponse
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            DataCriacao = usuario.DataCriacao,
            TotalTarefas = totalTarefas,
            TarefasPendentes = totalPendentes,
            TarefasConcluidas = totalConcluidas
        };

        return perfil;
    }
}
