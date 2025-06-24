using AutoMapper;
using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses.Usuario;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Infraestrutura.Security;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Usuario.AtualizarUsuario;

public class AtualizarUsuarioUseCase : IAtualizarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public AtualizarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioResponse> ExecuteAsync(int id, AtualizarUsuarioRequest request)
    {
        // ===== VALIDAÇÕES DE ENTRADA =====
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email é obrigatório");

        // Validar formato do email
        if (!IsValidEmail(request.Email))
            throw new ArgumentException("Email deve ter formato válido");

        // ===== BUSCAR USUÁRIO EXISTENTE =====
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException("Usuário não encontrado");

        // ===== VERIFICAR SE EMAIL JÁ EXISTE (PARA OUTRO USUÁRIO) =====
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();
        if (usuario.Email != emailNormalizado)
        {
            var emailExiste = await _usuarioRepository.EmailExisteAsync(emailNormalizado);
            if (emailExiste)
                throw new EmailJaExisteException("Este email já está em uso por outro usuário");
        }

        // ===== ATUALIZAR DADOS =====
        usuario.Nome = request.Nome.Trim();
        usuario.Email = emailNormalizado;
        usuario.DataAtualizacao = DateTime.UtcNow; 

        // ===== ATUALIZAR SENHA (SE FORNECIDA) =====
        if (!string.IsNullOrWhiteSpace(request.Senha))
        {
            // Validar força da senha
            if (request.Senha.Length < 6)
                throw new ArgumentException("Senha deve ter pelo menos 6 caracteres");

            usuario.SenhaHash = _passwordHasher.HashPassword(request.Senha); 
        }

        // ===== SALVAR ALTERAÇÕES =====
        var usuarioAtualizado = await _usuarioRepository.AtualizarAsync(usuario);

        // ===== RETORNAR RESPOSTA (SEM SENHA) =====
        return _mapper.Map<UsuarioResponse>(usuarioAtualizado);
    }

    /// <summary>
    /// Valida formato do email usando MailAddress
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}