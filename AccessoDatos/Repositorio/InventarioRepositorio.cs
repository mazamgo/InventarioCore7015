using AccesoDatos.Data;
using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Modelos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AccessoDatos.Repositorio
{
    public class InventarioRepositorio : Repositorio<Inventario>, IInventarioRepositorio
    {

        private readonly ApplicationDbContext _db;

        //ctor + tab + tab crea el constructor.
        public InventarioRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Inventario inventario)
        {
            var InventarioDB = _db.Inventarios.FirstOrDefault(b => b.Id == inventario.Id);
            if (InventarioDB != null) 
            {
                InventarioDB.BodegaId = inventario.BodegaId;
                InventarioDB.FechaFinal = inventario.FechaFinal;
                InventarioDB.Estado = inventario.Estado;

                
                _db.SaveChanges();
            
            }
        }

        IEnumerable<SelectListItem> IInventarioRepositorio.ObtenerTodosDropdownList(string obj)
        {
            if(obj == "Bodega") 
            {
                return _db.Bodegas.Where(x => x.Estado == true).Select(x => new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.Id.ToString()
                });
            }
            return null;
        }
    }
}
