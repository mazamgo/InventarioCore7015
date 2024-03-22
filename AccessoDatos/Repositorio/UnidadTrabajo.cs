using AccesoDatos.Data;
using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio
{
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly ApplicationDbContext _db;
        public IBodegaRepositorio Bodega { get; private set; }
        public ICategoriaRepositorio Categoria { get; private set; }
        public IMarcaRepositorio Marca {  get; private set; }
        public IProductoRepositorio Producto { get; private set; }
        public IUsuarioAplicacionRepositorio UsuarioAplicacion { get; private set; }
        public IBodegaProductoRepositorio bodegaProducto { get; private set; }
        public IInventarioRepositorio Inventario { get; private set; }
        public IInventarioDetalleRepositorio InventarioDetalle { get; private set; }
        public IKardexInventarioRepositorio KardexInventario { get; private set; }
        public ICompaniaRepositorio Compania { get; private set; }

        //contructor para iniciarlizar el ApplicationDbContext
        public UnidadTrabajo(ApplicationDbContext db)
        {
               _db = db;
              Bodega = new BodegaRepositorio(_db);
              Categoria = new CategoriaRepositorio(_db);
              Marca = new MarcaRepositorio(_db);
              Producto = new ProductoRepositorio(_db);
              UsuarioAplicacion = new UsuarioAplicacionRepositorio(_db);
              bodegaProducto = new BodegaProductoRepositorio(_db);
              Inventario = new InventarioRepositorio(_db);
              InventarioDetalle = new InventarioDetalleRepositorio(_db);
              KardexInventario = new KardexInventarioRepositorio(_db);
              Compania = new CompaniaRepositorio(_db);
        }

        //Este ya fue declarado arriba.
        //public IBodegaRepositorio bodega => throw new NotImplementedException();

        public void Dispose()
        {
            _db.Dispose(); //libera la memoria
        }

        //agregar async ya este metodo es asincrono.
        public async Task Guardar()
        {
            await _db.SaveChangesAsync();
        }
    }
}
