
using Tarefa.Domain.Entidades;

namespace Tarefa.Infraestrutura.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(Usuarios usuario);
}
