using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoML.Api.Handlers;
using ProyectoML.Application.Auth;
using ProyectoML.Application.Categorias;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Application.Productos;
using ProyectoML.Infrastructure.Auth;
using ProyectoML.Infrastructure.Client;
using ProyectoML.Infrastructure.MercadoLibre;
using ProyectoML.Infrastructure.Persistence;
using ProyectoML.Infrastructure.Repositories;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectoMlCONN")));


builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<CategoriaService>();

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ProductoService>();


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<MercadoLibreOptions>(
    builder.Configuration.GetSection("MercadoLibre"));

builder.Services.AddHttpClient<IMercadoLibreAuthClient, MercadoLibreAuthClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MercadoLibre:ApiUrl"]!);
});
builder.Services.AddScoped<ICredencialMercadoLibreRepository, CredencialMercadoLibreRepository>();
builder.Services.AddScoped<MercadoLibreAuthService>();

builder.Services.AddHttpClient<IMercadoLibreApiClient, MercadoLibreApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MercadoLibre:ApiUrl"]!);
});
builder.Services.AddScoped<MercadoLibreApiClientService>();


builder.Services.AddScoped<IPublicacionMercadoLibreRepository, PublicacionMercadoLibreRepository>();
builder.Services.AddScoped<PublicacionMercadoLibreService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<AuthService>();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Falta Jwt:Key en la configuración");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))

    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddDataProtection();
builder.Services.AddSingleton<IAuthStateProtector, AuthStateProtector>();


var app = builder.Build();




app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
