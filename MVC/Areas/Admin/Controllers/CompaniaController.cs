using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos;
using Modelos.ViewModels;
using System.Security.Claims;
using Utilidades;

namespace MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]

    public class CompaniaController : Controller
    {
        public readonly IUnidadTrabajo _unidadTrabajo;

        public CompaniaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public async Task<ActionResult> Upsert()
        {
            CompaniaVM companiaVM = new CompaniaVM()
            {
                Compania = new Modelos.Compania(),
                BodegaLista = _unidadTrabajo.Inventario.ObtenerTodosDropdownList("Bodega")
                
            };

            companiaVM.Compania = await _unidadTrabajo.Compania.get_Firts();

            if (companiaVM.Compania == null)
            {
                companiaVM.Compania = new Modelos.Compania();
            }

            return View(companiaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompaniaVM companiaVM)
        {

            // Limpiar el estado del modelo para que solo valide el objeto Inventario
            ModelState.Clear();

            // Agregar solo el estado del modelo Inventario
            foreach (var property in typeof(CompaniaVM).GetProperties())
            {
                if (ModelState.ContainsKey(property.Name))
                {
                    ModelState[property.Name].Errors.Clear();
                }
            }

            if (TryValidateModel(companiaVM.Compania.Id))
            {
                if (ModelState.IsValid)
                {
                    TempData[DS.Exitosa] = "Compania grabada Exitosamente";
                    var claimIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    if (companiaVM.Compania.Id == 0) //Crear la compania
                    {
                        companiaVM.Compania.CreadoPorId = claim.Value;
                        companiaVM.Compania.ActualizadoPorId = claim.Value;
                        companiaVM.Compania.FechaCreacion = DateTime.Now;
                        companiaVM.Compania.FechaActualizacion = DateTime.Now;
                        await _unidadTrabajo.Compania.Add(companiaVM.Compania);
                    }
                    else //Actualizar Compania
                    {
                        companiaVM.Compania.ActualizadoPorId = claim.Value;
                        companiaVM.Compania.FechaActualizacion = DateTime.Now;
                        _unidadTrabajo.Compania.Actualizar(companiaVM.Compania);

                    }

                    await _unidadTrabajo.Guardar();
                    return RedirectToAction("Index", "Home", new { area = "Inventario" });
                }

            }

            TempData[DS.Error] = "Error al Grabar Compania";
            companiaVM.BodegaLista = _unidadTrabajo.Inventario.ObtenerTodosDropdownList("Bodega");
            return View(companiaVM);
        }


    }




}
