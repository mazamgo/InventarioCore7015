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
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty]
        public OrdenDetalleVM ordenDetalleVM { get; set; }

        public OrdenController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detalle (int id)
        {
            ordenDetalleVM = new OrdenDetalleVM()
            {
                Orden = await _unidadTrabajo.Orden.get_Firts(o => o.Id == id, incluirPropiedades: "UsuarioAplicacion"),
                OrdenDetalleLista =  await _unidadTrabajo.OrdenDetalle.get_all(d => d.OrdenId == id, incluirPropiedades:"Producto")

            };

            return View(ordenDetalleVM);    
        }

        [Authorize(Roles =DS.Role_Admin)]
        public async Task<IActionResult> Procesar(int id)
        {
            var orden = await _unidadTrabajo.Orden.get_Firts(o => o.Id == id);

            orden.EstadoOrden = DS.EstadoEnProceso;
            await _unidadTrabajo.Guardar();
            TempData[DS.Exitosa] = "Orden cambiada a Estado en Proceso";
            return RedirectToAction("Detalle", new {id = id});

        }
        [HttpPost]
        [Authorize(Roles = DS.Role_Admin)]
        public async Task<IActionResult> EnviarOrden(OrdenDetalleVM ordenDetalleVM)
        {
            var orden = await _unidadTrabajo.Orden.get_Firts(o => o.Id == ordenDetalleVM.Orden.Id);

            orden.EstadoOrden = DS.EstadoEnviado;
            orden.Carrier = ordenDetalleVM.Orden.Carrier;
            orden.NumeroEnvio = ordenDetalleVM.Orden.NumeroEnvio;
            orden.FechaEnvio = DateTime.Now;

            await _unidadTrabajo.Guardar();
            TempData[DS.Exitosa] = "Orden cambiada a Estado Enviado";
            return RedirectToAction("Detalle", new { id = ordenDetalleVM.Orden.Id });

        }



        #region

        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenLista(string estado)
        {
            var  claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Orden> todos;

            //Validar el Rol del usuario.
            if (User.IsInRole(DS.Role_Admin))
            {
                todos = await _unidadTrabajo.Orden.get_all(incluirPropiedades: "UsuarioAplicacion");
            }
            else
            {
                todos = await _unidadTrabajo.Orden.get_all( o =>o.UsuarioAplicacionId == claim.Value,incluirPropiedades: "UsuarioAplicacion");
            }
            //Validar el Estado.
            switch (estado)
            {
                case "aprobado":
                    todos = todos.Where(o => o.EstadoOrden == DS.EstadoAprobado);
                    break;
                case "completado":
                    todos = todos.Where(o => o.EstadoOrden == DS.EstadoEnviado);
                    break;
                default:
                    break;
            }

            return Json(new { data = todos } );
        }

        #endregion


    }
}
