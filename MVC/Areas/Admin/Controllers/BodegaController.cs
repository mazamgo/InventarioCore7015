using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Modelos;
using Utilidades;

namespace MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Inventario)]
    public class BodegaController : Controller
    {
        //Instanciar nuestra Area de Trabajo
        private readonly IUnidadTrabajo _unidadTrabajo;

        //constructor para Iniciarlizar el area de trabajo.
        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id) 
        {
            Bodega bodega = new Bodega();

            if(id == null) 
            {
                //Bodega Nueva.
                bodega.Estado = true;
                return View(bodega);            
            }
            else 
            {
                //Obtener los datos de Bodega a Modificar.
                bodega = await _unidadTrabajo.Bodega.get(id.GetValueOrDefault());

                if(bodega == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(bodega);
                }
            
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Upsert (Bodega bodega)
        {
            if(ModelState.IsValid)
            {
                if(bodega.Id == 0)
                {
                    await _unidadTrabajo.Bodega.Add(bodega);
                    TempData[DS.Exitosa] = "Bodega Creada Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                    TempData[DS.Exitosa] = "Bodega Actualizada Exitosamente";
                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
            }

            TempData[DS.Error] = "Error al Guardar Bodega";
            return View(bodega);
        }
        
        #region Api

        //Metodo para obtener todos los datos y se va ocupar con el DataTable.
        public async Task<IActionResult> ObtenerTodos() 
        {
            var todos = await _unidadTrabajo.Bodega.get_all();
            return Json(new { data = todos });
        }

        public async Task<IActionResult> Delete(int id) 
        {
            var bodegaDb = await _unidadTrabajo.Bodega.get(id);
            if(bodegaDb == null)
            {
                return Json(new {success = false, messaje = "Error al borrar Bodega"});
            }

            _unidadTrabajo.Bodega.Delete(bodegaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitosamente" });

        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Bodega.get_all();
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
