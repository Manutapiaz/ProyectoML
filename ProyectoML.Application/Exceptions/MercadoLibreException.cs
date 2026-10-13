
namespace ProyectoML.Application.Exceptions;

public class MercadoLibreException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
