﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio.IRepositorio
{
    public  interface IUnidadTrabajo : IDisposable
    {
        IBodegaRepositorio Bodega {  get; }
        ICategoriaRepositorio Categoria { get; }
        IMarcaRepositorio Marca { get; }
        IProductoRepositorio Producto { get; }
        IUsuarioAplicacionRepositorio UsuarioAplicacion { get; }
        IBodegaProductoRepositorio bodegaProducto { get; }
        IInventarioRepositorio Inventario { get; }
        IInventarioDetalleRepositorio InventarioDetalle { get; }
        IKardexInventarioRepositorio KardexInventario { get; }

        Task Guardar(); //Metodo Asincronico.

    }
}
