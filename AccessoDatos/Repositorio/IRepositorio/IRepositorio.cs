using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        //Task para que nuestro metodos sean asincronos, el metodo Borrar no pueden ser asincronos.

        Task<T> get(int id);

        Task<IEnumerable<T>> get_all(
            Expression<Func<T,bool>> filtro = null, //Filtro
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, //Ordenar
            string incluirPropiedades = null, //Enlazar otros objetos
            bool isTracking = true //Modificar un objeto o una lista de objetos
            );

        Task<T> get_Firts( Expression<Func<T,bool>> filtro = null,
            string incluirPropiedades = null, 
            bool isTracking = true);

        Task Add( T entidad );

        void Delete( T entidad );

        void DeleteRange( IEnumerable<T> entidad );



    }
}
