

namespace Tarefa.Exceptions;

public class UsuarioNaoEncontradoException : System.Exception
{
    public UsuarioNaoEncontradoException() : base("Usuário não encontrado.")
    {
    }

    public UsuarioNaoEncontradoException(string message) : base(message)
    {
    }

    public UsuarioNaoEncontradoException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}
