using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.ViewModels;
using System.Security.Claims;
using Utilidades;

namespace MVC.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty]
        public CarroCompraVM carroCompraVM { get; set; }

        public CarroController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM = new CarroCompraVM();
            carroCompraVM.Orden = new Modelos.Orden();
            carroCompraVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.get_all(
                u => u.UsuarioAplicacionId == claim.Value, incluirPropiedades: "Producto");


            carroCompraVM.Orden.TotalOrden = 0;
            carroCompraVM.Orden.UsuarioAplicacionId = claim.Value;

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                //Siempre mostrar el Precio actual del Producto.
                lista.Precio = lista.Producto.Precio;
                carroCompraVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            return View(carroCompraVM);
        }

        public async Task<IActionResult> mas (int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.get_Firts(
                c => c.Id == carroId);

            carroCompras.Cantidad += 1;

            await _unidadTrabajo.Guardar();

            return RedirectToAction("Index");

            
        }

        public async Task<IActionResult> menos(int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.get_Firts(
                c => c.Id == carroId);

            if(carroCompras.Cantidad == 1)
            {
                //Removemos el Registro del Carro de compras y Actualizamos la session.
                var carrolista = await _unidadTrabajo.CarroCompra.get_all(
                    c => c.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId);

                var numeroProductos = carrolista.Count();
                _unidadTrabajo.CarroCompra.Delete(carroCompras);

                await _unidadTrabajo.Guardar();

                HttpContext.Session.SetInt32(DS.ssCarroCompras,numeroProductos-1);
            } 
            else
            {
                carroCompras.Cantidad -= 1;
                await _unidadTrabajo.Guardar();
            }                      

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> remover(int carroId)
        {
            //Remueve el Registro del Carro  de Compras y Actualiza la session.
            var carroCompras = await _unidadTrabajo.CarroCompra.get_Firts(
                c => c.Id == carroId);

            var carrolista = await _unidadTrabajo.CarroCompra.get_all(
                   c => c.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId);

            var numeroProductos = carrolista.Count();
            _unidadTrabajo.CarroCompra.Delete(carroCompras);

            await _unidadTrabajo.Guardar();

            HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);

            return RedirectToAction("Index");
        }


    }
}
