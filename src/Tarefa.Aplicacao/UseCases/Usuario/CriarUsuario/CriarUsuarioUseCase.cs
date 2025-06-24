using AutoMapper;
using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses.Usuario;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Infraestrutura.Security; // ✅ Correto

namespace Tarefa.Aplicacao.UseCases.Usuario.CriarUsuario;

public class CriarUsuarioUseCase : ICriarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher; // ✅ Interface correta

    public CriarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher) // ✅ Interface correta
    {
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioResponse> ExecuteAsync(CriarUsuarioRequest request)
    {
        // Validar dados obrigatórios
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Senha))
            throw new ArgumentException("Senha é obrigatória");

        // Validar formato do email
        if (!IsValidEmail(request.Email))
            throw new ArgumentException("Email deve ter formato válido");

        // Verificar se email já existe
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuarioExistente != null)
            throw new ArgumentException("Email já está em uso");

        // Validar senha
        if (request.Senha.Length < 6)
            throw new ArgumentException("Senha deve ter pelo menos 6 caracteres");

        // ✅ Criar novo usuário - CORRIGIDO
        var novoUsuario = new Domain.Entidades.Usuarios // ✅ Nome correto da classe
        {
            Nome = request.Nome.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            SenhaHash = _passwordHasher.HashPassword(request.Senha), // ✅ Campo e método corretos
            DataCriacao = DateTime.UtcNow
            // ✅ Removido DataAtualizacao - não existe na entidade
        };

        // Salvar no repositório
        var usuarioCriado = await _usuarioRepository.CriarAsync(novoUsuario);

        // Retornar resposta mapeada (sem senha)
        return _mapper.Map<UsuarioResponse>(usuarioCriado);
    }

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