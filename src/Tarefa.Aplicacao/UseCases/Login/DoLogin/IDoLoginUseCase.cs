using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses.Usuario;

namespace Tarefa.Aplicacao.UseCases.Login.DoLogin;

public interface IDoLoginUseCase
{
    Task<LoginResponse> ExecuteAsync(LoginRequest request);
}
