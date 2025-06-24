
using System.ComponentModel.DataAnnotations;

namespace Tarefa.Comunicacao.Requests.Usuario;

public class CriarUsuarioRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    [StringLength(100, ErrorMessage = "Senha deve ter no máximo 100 caracteres")]
    public string Senha { get; set; } = string.Empty;
}
