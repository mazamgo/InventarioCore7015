using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;
using Modelos;
using Utilidades;

namespace MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriaController : Controller
    {
        //Instanciar nuestra Area de Trabajo
        private readonly IUnidadTrabajo _unidadTrabajo;

        //constructor para Iniciarlizar el area de trabajo.
        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id) 
        {
            Categoria categoria = new Categoria();

            if(id == null) 
            {
                //categoria Nueva.
                categoria.Estado = true;
                return View(categoria);            
            }
            else 
            {
                //Obtener los datos de Bodega a Modificar.
                categoria = await _unidadTrabajo.Categoria.get(id.GetValueOrDefault());

                if(categoria == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(categoria);
                }
            
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Upsert (Categoria categoria)
        {
            if(ModelState.IsValid)
            {
                if(categoria.Id == 0)
                {
                    await _unidadTrabajo.Categoria.Add(categoria);
                    TempData[DS.Exitosa] = "Categoria Creada Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                    TempData[DS.Exitosa] = "Categoria Actualizada Exitosamente";
                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
            }

            TempData[DS.Error] = "Error al Guardar Bodega";
            return View(categoria);
        }
        
        #region Api

        //Metodo para obtener todos los datos y se va ocupar con el DataTable.
        public async Task<IActionResult> ObtenerTodos() 
        {
            var todos = await _unidadTrabajo.Categoria.get_all();
            return Json(new { data = todos });
        }

        public async Task<IActionResult> Delete(int id) 
        {
            var categoriaDb = await _unidadTrabajo.Categoria.get(id);
            if(categoriaDb == null)
            {
                return Json(new {success = false, messaje = "Error al borrar Categoria"});
            }

            _unidadTrabajo.Categoria.Delete(categoriaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitosamente" });

        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Categoria.get_all();
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
