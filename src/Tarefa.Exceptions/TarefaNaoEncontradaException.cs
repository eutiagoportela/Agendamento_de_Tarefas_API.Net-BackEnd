

namespace Tarefa.Exceptions;

public class TarefaNaoEncontradaException : System.Exception
{
    public TarefaNaoEncontradaException() : base("Tarefa não encontrada.")
    {
    }

    public TarefaNaoEncontradaException(string message) : base(message)
    {
    }

    public TarefaNaoEncontradaException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}
