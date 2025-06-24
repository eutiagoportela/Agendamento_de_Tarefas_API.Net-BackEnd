
using System.ComponentModel.DataAnnotations;
using Tarefa.Exceptions;


namespace Tarefa.Domain.Entidades;

public class Usuarios
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    public List<Tarefas> Tarefas { get; set; } = new();

    // Método para validar email único
    public void ValidarEmailUnico(bool emailExiste)
    {
        if (emailExiste)
            throw new EmailJaExisteException("Email já está em uso por outro usuário.");
    }
}
