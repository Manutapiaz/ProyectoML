using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Domain.Entities;

public class Categoria
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }

    private Categoria()
    {
        
    }
    public Categoria(string nombreCategoria)
    {
        if (string.IsNullOrEmpty(nombreCategoria)) throw new ArgumentException("Categoria no puede venir vacia");
        this.Nombre = nombreCategoria;
    }


}
