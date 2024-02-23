using System;
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

        Task Guardar(); //Metodo Asincronico.

    }
}
