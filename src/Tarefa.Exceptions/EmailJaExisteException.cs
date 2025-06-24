

namespace Tarefa.Exceptions;

public class EmailJaExisteException : System.Exception
{
    public EmailJaExisteException() : base("Email já está em uso.")
    {
    }

    public EmailJaExisteException(string message) : base(message)
    {
    }

    public EmailJaExisteException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}
