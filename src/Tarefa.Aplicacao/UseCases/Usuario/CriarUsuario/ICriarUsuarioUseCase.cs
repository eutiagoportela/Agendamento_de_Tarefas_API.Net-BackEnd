using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses.Usuario;

namespace Tarefa.Aplicacao.UseCases.Usuario.CriarUsuario;
public interface ICriarUsuarioUseCase
{
    Task<UsuarioResponse> ExecuteAsync(CriarUsuarioRequest request);
}
