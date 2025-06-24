
namespace Tarefa.Comunicacao.Responses.Usuario;

public class UsuarioMaisAtivoResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalTarefas { get; set; }
    public int TarefasConcluidas { get; set; }
    public double PercentualConclusao { get; set; }
}
