using Microsoft.EntityFrameworkCore;
using ProyectoML.Api.Handlers;
using ProyectoML.Application.Categorias;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Application.Productos;
using ProyectoML.Infrastructure.Client;
using ProyectoML.Infrastructure.MercadoLibre;
using ProyectoML.Infrastructure.Persistence;
using ProyectoML.Infrastructure.Repositories;

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
var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
