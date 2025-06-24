using Tarefa.Domain.Entidades;

public interface IUsuarioRepository
{
    // ===== OPERAÇÕES BÁSICAS (ESSENCIAIS) =====
    Task<Usuarios?> ObterPorIdAsync(int id);
    Task<Usuarios?> ObterPorEmailAsync(string email);
    Task<bool> EmailExisteAsync(string email);
    Task<Usuarios> CriarAsync(Usuarios usuario);
    Task<Usuarios> AtualizarAsync(Usuarios usuario);
    Task DeletarAsync(int id);
    Task<List<Usuarios>> ListarTodosAsync();
}