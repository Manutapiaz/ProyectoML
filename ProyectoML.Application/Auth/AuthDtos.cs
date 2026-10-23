
namespace ProyectoML.Application.Auth;



public record RegisterRequest(string NombreUsuario, string Email, string Password);
public record RegisterResponse(string Respuesta);

public record LoginRequest(string Email, string Password);