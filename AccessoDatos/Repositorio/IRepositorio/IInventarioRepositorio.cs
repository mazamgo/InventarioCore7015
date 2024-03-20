using Microsoft.AspNetCore.Mvc.Rendering;
using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio.IRepositorio
{
    public interface IInventarioRepositorio : IRepositorio<Inventario>
    {
        //Esto se realiza pq cada objeto tiene sus propias propiedades.
        void Actualizar(Inventario inventario);
        IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj);
      

    }
}
