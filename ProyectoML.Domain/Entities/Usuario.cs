using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Domain.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string NombreUsuario { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public DateTime FechaCreacion { get; private set; }

    private readonly List<CredencialMercadoLibre> _credencialesMercadoLibre = new();
    public IReadOnlyCollection<CredencialMercadoLibre> CredencialesMercadoLibre => _credencialesMercadoLibre;

    private Usuario()
    {

    }
    public Usuario(string nombre, string email, string passwordHash)
    {
        nombre = nombre?.Trim() ?? "";
        email = NormalizarEmail(email);
        if (string.IsNullOrWhiteSpace(nombre)) throw new DomainException("Nombre no puede estar vacío");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email no puede estar vacío");

        if (nombre.Length > 50) throw new DomainException("Nombre no puede tener más de 50 caracteres");
        if (email.Length > 256) throw new DomainException("Email no puede tener más de 256 caracteres");
      
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("PasswordHash no puede estar vacío");
        NombreUsuario = nombre;
        Email = email;
        PasswordHash = passwordHash;
        FechaCreacion = DateTime.UtcNow;
    }

    public static string NormalizarEmail(string? email) => email?.Trim().ToLowerInvariant() ?? "";

}
