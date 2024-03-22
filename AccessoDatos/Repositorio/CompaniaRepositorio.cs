using AccesoDatos.Data;
using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio
{
    public class CompaniaRepositorio : Repositorio<Compania>, ICompaniaRepositorio
    {

        private readonly ApplicationDbContext _db;

        //ctor + tab + tab crea el constructor.
        public CompaniaRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Actualizar(Compania compania)
        {
            var companiaBD = _db.Companias.FirstOrDefault(b => b.Id == compania.Id);
            if (companiaBD != null) 
            {
                companiaBD.Nombre = compania.Nombre;
                companiaBD.Descripcion = compania.Descripcion;
                companiaBD.Pais = compania.Pais;
                companiaBD.Ciudad = compania.Ciudad;
                companiaBD.Direccion = compania.Direccion;
                companiaBD.Telefono= compania.Telefono;
                companiaBD.BodegaVentaId = compania.BodegaVentaId;
                companiaBD.ActualizadoPorId = compania.ActualizadoPorId;
                companiaBD.FechaActualizacion = compania.FechaActualizacion;
               
                _db.SaveChanges();
            
            }
        }

        //public Task Add(Bodega entidad)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Delete(Bodega entidad)
        //{
        //    throw new NotImplementedException();
        //}

        //public void DeleteRange(IEnumerable<Bodega> entidad)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Bodega> get(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<Bodega>> get_all(Expression<Func<Bodega, bool>> filtro = null, Func<IQueryable<Bodega>, IOrderedQueryable<Bodega>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Bodega> get_Firts(Expression<Func<Bodega, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
