
namespace Tarefa.Comunicacao.Responses.Usuario;

public class PerfilUsuarioResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public int TotalTarefas { get; set; }
    public int TarefasPendentes { get; set; }
    public int TarefasConcluidas { get; set; }
    public DateTime? UltimaAtividade { get; set; }
    public double PercentualConclusao => TotalTarefas > 0 ? (double)TarefasConcluidas / TotalTarefas * 100 : 0;
}
