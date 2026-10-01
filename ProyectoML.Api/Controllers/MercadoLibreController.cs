using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProyectoML.Infrastructure.MercadoLibre;

namespace ProyectoML.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MercadoLibreController : ControllerBase
{

    private readonly IOptions<MercadoLibreOptions> _configuration;

    public MercadoLibreController(IOptions<MercadoLibreOptions> configuration)
    {
        _configuration = configuration;
    }


    [HttpGet("login")]
    public async Task<IActionResult> Login()
    {
        //https://auth.mercadolibre.com.ar/authorization?response_type=code&client_id=$APP_ID&redirect_uri=$YOUR_URL&code_challenge=$CODE_CHALLENGE&code_challenge_method=$CODE_METHOD
        // Aquí deberías redirigir al usuario a la página de login de Mercado Libre
        var redirectUrl = $"https://auth.mercadolibre.com.ar/authorization?response_type=code&client_id={_configuration.
            Value.ClientId}&redirect_uri={_configuration.Value.RedirectUri}";
        return Redirect(redirectUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {

        string url = "https://api.mercadolibre.com/oauth/token";
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(url);
            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _configuration.Value.ClientId },
                { "client_secret", _configuration.Value.ClientSecret },
                { "code", code },
                { "redirect_uri", _configuration.Value.RedirectUri }
            };

            await client.PostAsync(url, new FormUrlEncodedContent(requestData)).ContinueWith(responseTask =>
            {
                var response = responseTask.Result;
                if (!response.IsSuccessStatusCode)
                {
                }

                var responseContent = response.Content.ReadAsStringAsync().Result;
               
            });
            return Ok();
        }

    }
}
