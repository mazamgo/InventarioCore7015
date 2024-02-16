using AccesoDatos.Data;
using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet; //propiedad

        //constructor: ctor + tab + tab
        public Repositorio(ApplicationDbContext db)
        {
            //iniciarlizar las dos propiedades que declaramos.
            _db = db;
            this.dbSet = _db.Set<T>();
        }
      
        public async Task Add(T entidad)
        {
           await dbSet.AddAsync(entidad);
        }

        public void Delete(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void DeleteRange(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }

        public async Task<T> get(int id)
        {
            return await dbSet.FindAsync(id); //solo trabaja con el ID de la tabla.
        }

        public async Task<IEnumerable<T>> get_all(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;

            if(filtro != null) 
            { 
                query = query.Where(filtro); //select * from where...
            }

            if(incluirPropiedades != null)
            {
                foreach(var incluirProp in incluirPropiedades.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries)) 
                { 
                    query = query.Include(incluirProp);
                }
            }

            if(orderBy != null)
            {
                query = orderBy(query);
            }

            if (!isTracking) 
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<T> get_Firts(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;

            if (filtro != null)
            {
                query = query.Where(filtro); //select * from where...
            }

            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);
                }
            }           

            if (!isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
