using System.ComponentModel.DataAnnotations;
using Tarefa.Domain.Entidades;
using Tarefa.Domain.Enum;

public class Tarefas
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descricao { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime DataConclusao { get; set; }

    public bool Concluida { get; set; } = false;

    [Required]
    public PrioridadeTarefa Prioridade { get; set; } = PrioridadeTarefa.Media;

    public DateTime? DataLembrete { get; set; }
    public bool LembreteEnviado { get; set; } = false;

    [Required]
    public int UsuarioId { get; set; }
    public Usuarios Usuario { get; set; } = null!;

    // ===== MÉTODOS ESSENCIAIS (USADO NOS USE CASES) =====

    /// <summary>
    /// Marca a tarefa como concluída - USADO no MarcarTarefaComoConcluidaUseCase
    /// </summary>
    public void MarcarComoConcluida()
    {
        if (Concluida)
            throw new InvalidOperationException("Tarefa já está concluída");

        Concluida = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Define lembrete para a tarefa - USADO no CriarTarefaUseCase
    /// </summary>
    public void DefinirLembrete(DateTime dataLembrete)
    {
        // Obter horário atual do Brasil (UTC-3)
        var timeZoneBrasil = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        var agoraBrasil = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneBrasil);

        if (dataLembrete <= agoraBrasil)
            throw new ArgumentException("Data do lembrete nao pode ser menor que a atual data:  "+ agoraBrasil);
        if (dataLembrete > DataConclusao)
            throw new ArgumentException("Lembrete deve ser antes da data de conclusão");

        DataLembrete = dataLembrete;
        LembreteEnviado = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Valida se a tarefa pode ser criada - USADO no CriarTarefaUseCase
    /// </summary>
    public void ValidarCriacao()
    {
        if (string.IsNullOrWhiteSpace(Titulo))
            throw new ArgumentException("Título é obrigatório");

        if (DataConclusao.Date < DateTime.Now.Date)
            throw new ArgumentException("Data de conclusão deve ser futura");

        if (DataLembrete.HasValue && DataLembrete > DataConclusao)
            throw new ArgumentException("Lembrete deve ser antes da data de conclusão");
    }
}