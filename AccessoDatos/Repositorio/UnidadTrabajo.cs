using AccesoDatos.Data;
using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        //contructor para iniciarlizar el ApplicationDbContext
        public UnidadTrabajo(ApplicationDbContext db)
        {
           _db = db;
          Bodega = new BodegaRepositorio(db);
          Categoria = new CategoriaRepositorio(db);
          Marca = new MarcaRepositorio(db);
          Producto = new ProductoRepositorio(db);
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
