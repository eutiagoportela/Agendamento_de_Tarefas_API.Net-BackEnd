using Tarefa.Comunicacao.Responses.Usuario;

namespace Tarefa.Aplicacao.UseCases.Usuario.ObterUsuario;
public interface IObterUsuarioUseCase
{
    Task<UsuarioResponse> ExecuteAsync(int usuarioId);
}
