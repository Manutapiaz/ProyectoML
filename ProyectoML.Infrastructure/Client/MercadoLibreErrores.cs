using ProyectoML.Application.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProyectoML.Infrastructure.Client;

internal static class MercadoLibreErrores
{
    private record MlError(string? Message, List<MlErrorCause>? Cause);

    private record MlErrorCause(string? Type, string? Message);

    internal static async Task<MercadoLibreException> CrearExcepcionAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        var detalle = body;

        try
        {
            var error = JsonSerializer.Deserialize<MlError>(body, JsonOptions);
            var causas = error?.Cause?
                .Where(c => c.Type != "warning" && !string.IsNullOrWhiteSpace(c.Message))
                .Select(c => c.Message)
                .ToList() ?? [];

            if (causas.Count > 0) detalle = string.Join(" | ", causas);
            else if (!string.IsNullOrWhiteSpace(error?.Message)) detalle = error.Message;
        }
        catch (JsonException)
        {
        }

        return new MercadoLibreException(detalle, (int)response.StatusCode);
    }
    private static readonly JsonSerializerOptions JsonOptions =
       new()
       {
           PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
           DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
       };
    }




