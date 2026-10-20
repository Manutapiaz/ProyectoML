
using ProyectoML.Domain.Entities;
using ProyectoML.Domain.Enums;
using ProyectoML.Domain.Exceptions;

namespace ProyectoML.UnitTests.Domain;

public class ProductoTests
{
    [Fact]
    public void RegistrarMovimiento_IngresoPositivo_AumentaStock()
    {
        var producto = CrearProducto();

        producto.RegistrarMovimiento(10, MotivoMovimiento.Ingreso, "Test");

        Assert.Equal(10, producto.Stock);
        Assert.Single(producto.Movimientos);
    }

    [Fact]
    public void RegistrarMovimiento_IngresoNegativo_LanzaDomainException()
    {
        var producto = CrearProducto();

        Assert.Throws<DomainException>(() =>
            producto.RegistrarMovimiento(-5, MotivoMovimiento.Ingreso, "Test"));

        Assert.Equal(0, producto.Stock);
        Assert.Empty(producto.Movimientos);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_PrecioInvalido_LanzaDomainException(decimal precio)
    {
        Assert.Throws<DomainException>(() =>
            new Producto("SKU-1", "Nombre", "Desc", precio, new Categoria("Cat", null)));
    }

    [Fact]
    public void RegistrarMovimiento_VentaNegativa_DescuentaStock()
    {
        var producto = CrearProducto();
        producto.RegistrarMovimiento(10, MotivoMovimiento.Ingreso, "Ingreso");
        producto.RegistrarMovimiento(-5, MotivoMovimiento.Venta, "Venta");

        Assert.Equal(5, producto.Stock);
    }

    [Fact]
    public void RegistrarMovimiento_VentaStockNegativo_LanzaDomainException()
    {
        var producto = CrearProducto();
        Assert.Throws<DomainException>(() =>
            producto.RegistrarMovimiento(-15, MotivoMovimiento.Venta, "Stock Negativo"));

        Assert.Equal(0, producto.Stock);
        Assert.Empty(producto.Movimientos);
    }

    [Fact]
    public void RegistrarMovimiento_VentaCantidadPositiva_LanzaDomainException()
    {
        var producto = CrearProducto();
        Assert.Throws<DomainException>(() =>
            producto.RegistrarMovimiento(5, MotivoMovimiento.Venta, "Venta cantida positiva"));

        Assert.Equal(0, producto.Stock);
        Assert.Empty(producto.Movimientos);
    }

    [Fact]
    public void RegistrarMovimiento_VentaCantidadCero_LanzaDomainException()
    {
        var producto = CrearProducto();
        Assert.Throws<DomainException>(() =>
            producto.RegistrarMovimiento(0, MotivoMovimiento.Venta, "Venta cantida cero"));
        Assert.Equal(0, producto.Stock);
        Assert.Empty(producto.Movimientos);
    }

    [Theory]
    [InlineData(5, 15)]
    [InlineData(-3, 7)]
    public void RegistrarMovimiento_Ajuste_ModificaStock(int ajuste, int stockEsperado)
    {

        var producto = CrearProducto();
        producto.RegistrarMovimiento(10, MotivoMovimiento.Ingreso, "Ingreso");
        producto.RegistrarMovimiento(ajuste, MotivoMovimiento.Ajuste, "Ajuste");
        Assert.Equal(stockEsperado, producto.Stock);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_SkuVacio_LanzaDomainException(string sku)
    {
        Assert.Throws<DomainException>(() =>
            new Producto(sku, "Nombre", "Desc", 1000m, new Categoria("Cat", null)));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaProductoConStockCeroYActivo()
    {
        var producto = CrearProducto();

        Assert.Equal(0, producto.Stock);
        Assert.True(producto.Activo);
        Assert.Empty(producto.Movimientos);

    }

    [Fact]
    public void CambiarPrecio_PrecioValido_ActualizaPrecio()
    {
        var producto = CrearProducto();
        producto.CambiarPrecio(2000m);
        Assert.Equal(2000m, producto.Precio);
    }

    [Theory] 
    [InlineData("ftp://ejemplo.cl/foto.jpg")]
    [InlineData("file:///C:/fotos/foto.jpg")]
    [InlineData("invalid-url")] 
    public void AgregarImagen_UrlInvalida_LanzaDomainException(string url)
    {
        var producto = CrearProducto();
        Assert.Throws<DomainException>(() =>
            producto.AgregarImagen(url));

    }

    [Fact]
    public void AgregarImagen_UrlValida_AgregaImagen()
    {
        var producto = CrearProducto();
        string url = "http://example.com/image.jpg";
        producto.AgregarImagen(url);
        Assert.Single(producto.Imagenes);
        Assert.Equal(url, producto.Imagenes.Single().Url);
    }

    [Fact]
    public void AgregarImagen_MaximoAlcanzado_LanzaDomainException()
    {
        var producto = CrearProducto();
        for (int i = 0; i < Producto.MaxImagenes; i++)
        {
            producto.AgregarImagen($"http://example.com/image{i}.jpg");
        }
        Assert.Throws<DomainException>(() =>
            producto.AgregarImagen("http://example.com/image14.jpg"));
        Assert.Equal(Producto.MaxImagenes, producto.Imagenes.Count);
    }


    [Fact]
    public void AgregarImagen_UrlRepetida_LanzaDomainException()
    {
        var producto = CrearProducto();
        producto.AgregarImagen("http://example.com/image.jpg");
        Assert.Throws<DomainException>(() =>
            producto.AgregarImagen("http://example.com/image.jpg"));
    }



    public static Producto CrearProducto()
        => new("SKU-1", "Producto test", "Descripción", 1000m, new Categoria("Categoría test", null));


}
