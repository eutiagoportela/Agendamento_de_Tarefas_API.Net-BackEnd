
using System.ComponentModel.DataAnnotations;


namespace Tarefa.Comunicacao.Requests.Usuario;

public class AtualizarUsuarioRequest
{
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string? Email { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
    public string? Senha { get; set; }

    [Compare("Senha", ErrorMessage = "Confirmação de senha deve ser igual à nova senha")]
    public string? ConfirmarSenha { get; set; }
}
