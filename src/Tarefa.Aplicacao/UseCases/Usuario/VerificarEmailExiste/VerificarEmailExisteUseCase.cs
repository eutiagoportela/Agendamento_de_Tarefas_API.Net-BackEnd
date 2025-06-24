using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Tarefa.Domain.Repositorios.Interfaces;

namespace Tarefa.Aplicacao.UseCases.Usuario.VerificarEmailExiste;

public class VerificarEmailExisteUseCase : IVerificarEmailExisteUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<VerificarEmailExisteUseCase> _logger;
    private readonly IMemoryCache _cache;

    public VerificarEmailExisteUseCase(
        IUsuarioRepository usuarioRepository,
        ILogger<VerificarEmailExisteUseCase> logger,
        IMemoryCache cache)
    {
        _usuarioRepository = usuarioRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> ExecuteAsync(string email)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório");

        // Normalizar email
        var emailNormalizado = email.Trim().ToLowerInvariant();

        // Validar formato básico
        if (!IsValidEmailFormat(emailNormalizado))
            throw new ArgumentException("Formato de email inválido");

        // Log da tentativa (para monitoramento de ataques)
        _logger.LogInformation("Verificação de email solicitada: {Email}", emailNormalizado);

        // Cache para reduzir consultas ao banco (5 minutos)
        var cacheKey = $"email_exists_{emailNormalizado}";
        if (_cache.TryGetValue(cacheKey, out bool cachedResult))
        {
            _logger.LogDebug("Resultado obtido do cache para email: {Email}", emailNormalizado);
            return cachedResult;
        }

        // Consultar banco
        var existe = await _usuarioRepository.EmailExisteAsync(emailNormalizado);

        // Cachear resultado
        _cache.Set(cacheKey, existe, TimeSpan.FromMinutes(5));

        // Log apenas se email existe (para auditoria)
        if (existe)
        {
            _logger.LogWarning("Tentativa de verificação de email existente: {Email}", emailNormalizado);
        }

        return existe;
    }

    private static bool IsValidEmailFormat(string email)
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
