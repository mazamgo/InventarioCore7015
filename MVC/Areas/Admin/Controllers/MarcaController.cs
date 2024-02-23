using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;
using Modelos;
using Utilidades;

namespace MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarcaController : Controller
    {
        //Instanciar nuestra Area de Trabajo
        private readonly IUnidadTrabajo _unidadTrabajo;

        //constructor para Iniciarlizar el area de trabajo.
        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id) 
        {
            Marca marca = new Marca();

            if(id == null) 
            {
                //categoria Nueva.
                marca.Estado = true;
                return View(marca);            
            }
            else 
            {
                //Obtener los datos de Bodega a Modificar.
                marca = await _unidadTrabajo.Marca.get(id.GetValueOrDefault());

                if(marca == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(marca);
                }
            
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Upsert (Marca marca)
        {
            if(ModelState.IsValid)
            {
                if(marca.Id == 0)
                {
                    await _unidadTrabajo.Marca.Add(marca);
                    TempData[DS.Exitosa] = "Marca Creada Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Marca.Actualizar(marca);
                    TempData[DS.Exitosa] = "Marca Actualizada Exitosamente";
                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
            }

            TempData[DS.Error] = "Error al Guardar Bodega";
            return View(marca);
        }
        
        #region Api

        //Metodo para obtener todos los datos y se va ocupar con el DataTable.
        public async Task<IActionResult> ObtenerTodos() 
        {
            var todos = await _unidadTrabajo.Marca.get_all();
            return Json(new { data = todos });
        }

        public async Task<IActionResult> Delete(int id) 
        {
            var marcaDb = await _unidadTrabajo.Marca.get(id);
            if(marcaDb == null)
            {
                return Json(new {success = false, messaje = "Error al borrar Marca"});
            }

            _unidadTrabajo.Marca.Delete(marcaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Marca borrada exitosamente" });

        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Marca.get_all();
            if (id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }
            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }


        #endregion

    }
}
