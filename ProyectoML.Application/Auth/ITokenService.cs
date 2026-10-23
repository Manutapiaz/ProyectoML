using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Auth;

public interface ITokenService
{
    string GenerarToken(Usuario usuario);
}
