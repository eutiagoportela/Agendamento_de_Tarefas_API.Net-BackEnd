

namespace Tarefa.Exceptions;

public class AcessoNegadoException : System.Exception
{
    public AcessoNegadoException() : base("Acesso negado a este recurso.")
    {
    }

    public AcessoNegadoException(string message) : base(message)
    {
    }

    public AcessoNegadoException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}
