using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses.Usuario;

namespace Tarefa.Aplicacao.UseCases.Usuario.AtualizarUsuario;

public interface IAtualizarUsuarioUseCase
{
    Task<UsuarioResponse> ExecuteAsync(int usuarioId, AtualizarUsuarioRequest request);
}
