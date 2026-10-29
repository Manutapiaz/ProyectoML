
using ProyectoML.Application.Exceptions;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Application.Productos;

namespace ProyectoML.Application.MercadoLibre.NotificacionML;

public class NotificacionMercadoLibreService
{
    private readonly IMercadoLibreApiClient _mercadoLibreApiClient;
    private readonly ICredencialMercadoLibreRepository _credencialMercadoLibreRepository;
    private readonly MercadoLibreAuthService _mercadoLibreAuthService;

    private readonly IProductoRepository _productoRepository;

    private readonly IPublicacionMercadoLibreRepository _publicacionMercadoLibreService;

    public NotificacionMercadoLibreService(
        IMercadoLibreApiClient mercadoLibreApiClient,
        ICredencialMercadoLibreRepository credencialMercadoLibreRepository,
        MercadoLibreAuthService mercadoLibreAuthService,
        IProductoRepository productoRepository,
        IPublicacionMercadoLibreRepository publicacionMercadoLibreService)
    {
        _mercadoLibreApiClient = mercadoLibreApiClient;
        _mercadoLibreAuthService = mercadoLibreAuthService;
        _credencialMercadoLibreRepository = credencialMercadoLibreRepository;
        _productoRepository = productoRepository;
        _publicacionMercadoLibreService = publicacionMercadoLibreService;
    }

    public async Task ProcesarNotificacionAsync(NotificationRequest notificacion)
    {
        if (notificacion.Topic != "orders_v2")
            return;

        var idOrden = notificacion.Resource?.Replace("/orders/", "");
        if (!long.TryParse(idOrden, out var orderId))
            return;

        var credencial = await _credencialMercadoLibreRepository.GetByUserIdAsync(notificacion.UserId);
        if (credencial is null)
            return;

        var token = await _mercadoLibreAuthService.ObtenerAccessTokenAsync(credencial.UserId, credencial.UsuarioId);

        var orden = await _mercadoLibreApiClient.ObtenerOrdenAsync(orderId, token);
        if (orden.Estado != "paid")
            return;

        var referencia = "Venta ML: Orden: " + orden.Id;

        foreach (var item in orden.Items)
        {
            var publicacion = await _publicacionMercadoLibreService.GetByExternalIdAsync(item.ItemId);
            if (publicacion is null)
                continue;

            var producto = await _productoRepository.GetByIdAsync(publicacion.ProductoId, credencial.UsuarioId);
            if (producto is null)
                continue;

            if (await _productoRepository.ExisteMovimientoAsync(producto.Id, referencia))
                continue;

            if (producto.Stock < item.Cantidad)
                continue;

            producto.RegistrarMovimiento(-item.Cantidad, Domain.Enums.MotivoMovimiento.Venta, referencia);
            try
            {
                await _productoRepository.UpdateAsync(producto);
            }
            catch (ConflictException)
            {
                continue;
            }
        }
    }
}
