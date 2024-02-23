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
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {

        private readonly ApplicationDbContext _db;

        //ctor + tab + tab crea el constructor.
        public CategoriaRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Categoria categoria)
        {
            var categoriaBD = _db.Categorias.FirstOrDefault(b => b.Id == categoria.Id);
            if (categoriaBD != null) 
            {
                categoriaBD.Nombre = categoria.Nombre;
                categoriaBD.Descripcion = categoria.Descripcion;
                categoriaBD.Estado = categoria.Estado;
                _db.SaveChanges();
            
            }
        }

        
    }
}
