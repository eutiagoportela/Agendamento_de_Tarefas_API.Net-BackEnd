using AutoMapper;
using Tarefa.Comunicacao.Responses.Usuario;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Usuario.ObterUsuario;

public class ObterUsuarioUseCase : IObterUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public ObterUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<UsuarioResponse> ExecuteAsync(int usuarioId)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        return _mapper.Map<UsuarioResponse>(usuario);
    }
}
