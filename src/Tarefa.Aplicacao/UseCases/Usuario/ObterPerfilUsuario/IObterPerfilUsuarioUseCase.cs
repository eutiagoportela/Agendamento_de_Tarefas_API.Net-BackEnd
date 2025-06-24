
using Tarefa.Comunicacao.Responses.Usuario;

namespace Tarefa.Aplicacao.UseCases.Usuario.ObterPerfilUsuario;

public interface IObterPerfilUsuarioUseCase
{
    Task<PerfilUsuarioResponse> ExecuteAsync(int usuarioId);
}
