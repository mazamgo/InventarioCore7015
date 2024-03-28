using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Modelos;
using Modelos.ViewModels;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using Stripe.Identity;
using System.Security.Claims;
using Utilidades;

namespace MVC.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private string _webUrl;

        [BindProperty]
        public CarroCompraVM carroCompraVM { get; set; }

        public CarroController(IUnidadTrabajo unidadTrabajo, IConfiguration configuracion)
        {
            _unidadTrabajo = unidadTrabajo;
            _webUrl = configuracion.GetValue<string>("DomainUrls:WEB_URL");
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

        public async Task<IActionResult> mas(int carroId)
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

            if (carroCompras.Cantidad == 1)
            {
                //Removemos el Registro del Carro de compras y Actualizamos la session.
                var carrolista = await _unidadTrabajo.CarroCompra.get_all(
                    c => c.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId);

                var numeroProductos = carrolista.Count();
                _unidadTrabajo.CarroCompra.Delete(carroCompras);

                await _unidadTrabajo.Guardar();

                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);
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


        public async Task<IActionResult> Proceder()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM = new CarroCompraVM()
            {
                Orden = new Modelos.Orden(),
                CarroCompraLista = await _unidadTrabajo.CarroCompra.get_all(
                    c => c.UsuarioAplicacionId == claim.Value, incluirPropiedades: "Producto"),
                Compania = await _unidadTrabajo.Compania.get_Firts()
            };

            carroCompraVM.Orden.TotalOrden = 0;
            carroCompraVM.Orden.UsuarioAplicacion = await _unidadTrabajo.UsuarioAplicacion.get_Firts(
                u => u.Id == claim.Value);

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio;
                carroCompraVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            carroCompraVM.Orden.NombresCliente = carroCompraVM.Orden.UsuarioAplicacion.Nombres + " " +
                                                 carroCompraVM.Orden.UsuarioAplicacion.Apellidos;
            carroCompraVM.Orden.Telefono = carroCompraVM.Orden.UsuarioAplicacion.PhoneNumber;
            carroCompraVM.Orden.Direccion = carroCompraVM.Orden.UsuarioAplicacion.Direccion;
            carroCompraVM.Orden.Pais = carroCompraVM.Orden.UsuarioAplicacion.Pais;
            carroCompraVM.Orden.Ciudad = carroCompraVM.Orden.UsuarioAplicacion.Ciudad;

            //Controlar Stock
            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                //Capturar el Stock de cada Producto
                var producto = await _unidadTrabajo.bodegaProducto.get_Firts(
                    b => b.ProductoId == lista.ProductoId && b.BodegaId == carroCompraVM.Compania.BodegaVentaId);

                if (lista.Cantidad > producto.Cantidad)
                {
                    TempData[DS.Error] = "La Cantidad del Producto " + lista.Producto.Descripcion + "Excede al Stock actual (" + producto.Cantidad + ")";
                    return RedirectToAction("Index");
                }

            }

            return View(carroCompraVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proceder(CarroCompraVM carroCompraVM)
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.get_all(
                c => c.UsuarioAplicacionId == claim.Value, incluirPropiedades: "Producto"
                );

            carroCompraVM.Compania = await _unidadTrabajo.Compania.get_Firts();
            carroCompraVM.Orden.TotalOrden = 0;
            carroCompraVM.Orden.UsuarioAplicacionId = claim.Value;
            carroCompraVM.Orden.FechaOrden = DateTime.Now;

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio;
                carroCompraVM.Orden.TotalOrden += (lista.Producto.Precio * lista.Cantidad);
            }
            
            //controlar el stock
            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                //Capturar el Stock de cada Producto
                var producto = await _unidadTrabajo.bodegaProducto.get_Firts(
                    b => b.ProductoId == lista.ProductoId && b.BodegaId == carroCompraVM.Compania.BodegaVentaId);

                if (lista.Cantidad > producto.Cantidad)
                {
                    TempData[DS.Error] = "La Cantidad del Producto " + lista.Producto.Descripcion + "Excede al Stock actual (" + producto.Cantidad + ")";
                    return RedirectToAction("Index");
                }
            }

            carroCompraVM.Orden.EstadoOrden = DS.EstadoPendiente;
            carroCompraVM.Orden.EstadoPago = DS.PagoEstadoPendiente;

            await _unidadTrabajo.Orden.Add(carroCompraVM.Orden);
            await _unidadTrabajo.Guardar();

            //Grabar el Detalle.
            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle() 
                { 
                    ProductoId = lista.ProductoId,
                    OrdenId = carroCompraVM.Orden.Id,
                    Precio = lista.Precio,
                    Cantidad = lista.Cantidad

                };

                await _unidadTrabajo.OrdenDetalle.Add(ordenDetalle);
                await _unidadTrabajo.Guardar();
            }

            //Stripe
            var usuario = await _unidadTrabajo.UsuarioAplicacion.get_Firts(u => u.Id == claim.Value);

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = _webUrl + $"Inventario/Carro/OrdenConfirmacion?id={carroCompraVM.Orden.Id}",
                CancelUrl = _webUrl + "Inventario/Carro/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = usuario.Email
            };

            //Quede en el minuto 04:00 de nombre 142. Stripe en Proceder Post.
            foreach(var lista in carroCompraVM.CarroCompraLista)
            {
                var sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(lista.Precio * 100), //$20 => 200
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = lista.Producto.Descripcion
                        }
                        
                    },
                    Quantity = lista.Cantidad
                };

                options.LineItems.Add(sessionLineItem);
            }

            //var service = new Stripe.BillingPortal.SessionService();
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            _unidadTrabajo.Orden.ActualizarPagoStripeId(carroCompraVM.Orden.Id,session.Id,session.PaymentIntentId);
            await _unidadTrabajo.Guardar();
            Response.Headers.Add("Location", session.Url); //Redirecciona a Stripe
            return new StatusCodeResult(303);
                
            //return RedirectToAction("OrdenConfirmaicion", new {id = carroCompraVM.Orden.Id});          

        }


        public async Task<IActionResult> OrdenConfirmacion(int id)
        {
            var orden = await _unidadTrabajo.Orden.get_Firts(o => o.Id == id, incluirPropiedades:"UsuarioAplicacion");

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(orden.SessionId);

            var carroCompra = await _unidadTrabajo.CarroCompra.get_all(
               u => u.UsuarioAplicacionId == orden.UsuarioAplicacionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unidadTrabajo.Orden.ActualizarPagoStripeId(id,session.Id,session.PaymentIntentId); 
                _unidadTrabajo.Orden.ActualizaEstado(id,DS.EstadoAprobado, DS.PagoEstadoAprobado);  
                await _unidadTrabajo.Guardar();

                //Disminuir el Stock de la Bodega de Venta.
                var compania = await _unidadTrabajo.Compania.get_Firts();
                foreach(var lista in carroCompra)
                {
                    var bodegaProducto = new BodegaProducto();
                    bodegaProducto = await _unidadTrabajo.bodegaProducto.get_Firts(
                        b => b.ProductoId == lista.ProductoId && b.BodegaId == compania.BodegaVentaId);

                    await _unidadTrabajo.KardexInventario.RegistrarKardex(bodegaProducto.Id, "Salida"
                        , "Venta - Orden#" + id, bodegaProducto.Cantidad, lista.Cantidad, orden.UsuarioAplicacionId);

                    bodegaProducto.Cantidad -= lista.Cantidad;
                    await _unidadTrabajo.Guardar();
                }

            }
            //Borramos el Carro de compra y la session del carro de Compras.
           

            List<CarroCompra> carroCompraLista = carroCompra.ToList();
            _unidadTrabajo.CarroCompra.DeleteRange(carroCompraLista);

            await _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompras,0);

            return View(id);
        }


    }
}
