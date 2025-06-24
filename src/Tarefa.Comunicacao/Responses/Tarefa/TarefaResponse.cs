
using Tarefa.Domain.Enum;

namespace Tarefa.Comunicacao.Responses.Tarefa;

public class TarefaResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataConclusao { get; set; }
    public PrioridadeTarefa Prioridade { get; set; }
    public string PrioridadeTexto => Prioridade.ToString();
    public bool Concluida { get; set; }
    public bool LembreteEnviado { get; set; }
    public DateTime? DataLembrete { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; } 
    public int UsuarioId { get; set; }
    public string? NomeUsuario { get; set; }
    public bool EstaAtrasada => !Concluida && DataConclusao < DateTime.UtcNow;
    public int DiasRestantes => (int)(DataConclusao - DateTime.UtcNow).TotalDays;
}
