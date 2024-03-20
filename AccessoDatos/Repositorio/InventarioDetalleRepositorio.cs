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
    public class InventarioDetalleRepositorio : Repositorio<InventarioDetalle>, IInventarioDetalleRepositorio
    {

        private readonly ApplicationDbContext _db;

        //ctor + tab + tab crea el constructor.
        public InventarioDetalleRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(InventarioDetalle inventarioDetalle)
        {
            var InventarioDetalleDB = _db.InventarioDetalles.FirstOrDefault(b => b.Id == inventarioDetalle.Id);
            if (InventarioDetalleDB != null) 
            {
                InventarioDetalleDB.StockAnterior = inventarioDetalle.StockAnterior;
                InventarioDetalleDB.Cantidad = inventarioDetalle.Cantidad;

                _db.SaveChanges();
            
            }
        }
     
       

    }
}
