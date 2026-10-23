using ProyectoML.Domain.Entities;

namespace ProyectoML.Application.Auth;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verificar(string hashedPassword, string password);   
}
