

namespace Tarefa.Aplicacao.UseCases.Usuario.VerificarEmailExiste;
public interface IVerificarEmailExisteUseCase
{
    Task<bool> ExecuteAsync(string email);
}
