using Microsoft.EntityFrameworkCore;
using Tarefa.Domain.Entidades;

namespace Tarefa.Infraestrutura.Repositorios.Implementacoes;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly PostgreSqlDbContext _context;

    public UsuarioRepository(PostgreSqlDbContext context)
    {
        _context = context;
    }

    public async Task<Usuarios?> ObterPorIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Tarefas)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuarios?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email);
    }

    public async Task<Usuarios> CriarAsync(Usuarios usuario)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return usuario;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Usuarios> AtualizarAsync(Usuarios usuario)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return usuario;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeletarAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Usuarios>> ListarTodosAsync()
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }
}