using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.NativeInterop;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Infrastructure.MercadoLibre;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ProyectoML.Infrastructure;

public class MercadoLibreAuthClient : IMercadoLibreAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<MercadoLibreOptions> _options;



    public MercadoLibreAuthClient(HttpClient httpClient, IOptions<MercadoLibreOptions> options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public string GetAuthorizationUrl()
        => $"{_options.Value.AuthUrl}?response_type=code&client_id={_options.Value.ClientId}&redirect_uri={Uri.EscapeDataString(_options.Value.RedirectUri)}";

    public async Task<TokenResult> GetAccessTokenAsync(string code)
    {
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", _options.Value.ClientId },
            { "client_secret", _options.Value.ClientSecret },
            { "code", code },
            { "redirect_uri", _options.Value.RedirectUri }
        });

        var response = await _httpClient.PostAsync("/oauth/token", form);
        response.EnsureSuccessStatusCode();

        var ml = await response.Content.ReadFromJsonAsync<MlTokenResponse>(JsonOptions);
        return new TokenResult(ml!.AccessToken, ml.ExpiresIn, ml.RefreshToken, ml.UserId);
    }


    public async Task<TokenResult> RefreshAccessTokenAsync(string refreshToken)
    {
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", _options.Value.ClientId },
            { "client_secret", _options.Value.ClientSecret },
            { "refresh_token", refreshToken }
        });
        
        var response = await _httpClient.PostAsync("/oauth/token", form);
        response.EnsureSuccessStatusCode();

        var ml = await response.Content.ReadFromJsonAsync<MlTokenResponse>(JsonOptions);
        return new TokenResult(ml!.AccessToken, ml.ExpiresIn, ml.RefreshToken, ml.UserId);
    }

    private record MlTokenResponse(string AccessToken, int ExpiresIn, string RefreshToken, long UserId);

    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
}
