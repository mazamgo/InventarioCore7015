using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos;
using Modelos.Especificaciones;
using Modelos.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using Utilidades;

namespace MVC.Areas.Inventario.Controllers
{
    [Area("Inventario")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty] //Para utilizar este mismo propiedad en varios metodos.
        public CarroCompraVM carroCompraVM {  get; set; }

        public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo)
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;
        }

        //Este metodo los convertimos en Asyncrono debido a la session para agregarle valor.
        public async Task<IActionResult> Index(int pageNumber = 1, string busqueda = "", string busquedaActual = "")
        {

            //Controlar session
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                //Agregar valor a la Session
                var carroLista = await _unidadTrabajo.CarroCompra.get_all(c => c.UsuarioAplicacionId == claim.Value);
                var numeroProductos = carroLista.Count(); //Numero de Registro.
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos);

            }

            if (!String.IsNullOrEmpty(busqueda))
            {
                pageNumber = 1;
            }
            else
            {
                busqueda = busquedaActual;
            }
            ViewData["BusquedaActual"] = busqueda;

            if (pageNumber < 1) pageNumber = 1;

            Parametros parametros = new Parametros()
            {
                PageNumber = pageNumber,
                PageSize    =   4
            };

            var resultado = _unidadTrabajo.Producto.ObtenerTodosPaginado(parametros);

            if (!String.IsNullOrEmpty(busqueda))
            {
                resultado = _unidadTrabajo.Producto.ObtenerTodosPaginado(parametros, p => p.Descripcion.Contains(busqueda));
            }

            ViewData["TotalPaginas"] = resultado.MetaData.TotalPages;
            ViewData["TotalRegistros"] = resultado.MetaData. TotalCount;
            ViewData["PageSize"] = resultado.MetaData.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Previo"] = "disabled"; // clase css para desactivar el boton
            ViewData["Siguiente"] = "";

            if (pageNumber > 1) { ViewData["Previo"] = ""; }
            if (resultado.MetaData.TotalPages <= pageNumber) { ViewData["Siguiente"] = "disabled"; }

            return View(resultado);


        }

        public async Task<IActionResult> Detalle(int id)
        {
            carroCompraVM = new CarroCompraVM();
            carroCompraVM.Compania = await _unidadTrabajo.Compania.get_Firts();
            carroCompraVM.Producto = await _unidadTrabajo.Producto.get_Firts(p => p.Id == id,
                incluirPropiedades: "Marca,Categoria");

            var bodegaProducto = await _unidadTrabajo.bodegaProducto.get_Firts(b => b.ProductoId == id &&
            b.BodegaId == carroCompraVM.Compania.BodegaVentaId);

            if(bodegaProducto == null)
            {
                carroCompraVM.Stock = 0;
            }
            else
            {
                carroCompraVM.Stock = bodegaProducto.Cantidad;
            }

            carroCompraVM.CarroCompra = new CarroCompra()
            {
                Producto = carroCompraVM.Producto,
                ProductoId = carroCompraVM.Producto.Id
            };

            return View(carroCompraVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detalle(CarroCompraVM carroCompraVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM.CarroCompra.UsuarioAplicacionId = claim.Value;
            CarroCompra carroBD = await _unidadTrabajo.CarroCompra.get_Firts(
                c => c.UsuarioAplicacionId == claim.Value && c.ProductoId == carroCompraVM.CarroCompra.ProductoId);

            if(carroBD == null)
            {
                await _unidadTrabajo.CarroCompra.Add(carroCompraVM.CarroCompra);
            }
            else
            {
                carroBD.Cantidad += carroCompraVM.CarroCompra.Cantidad;
                _unidadTrabajo.CarroCompra.Actualizar(carroBD);
            }

            await _unidadTrabajo.Guardar();
            TempData[DS.Exitosa] = "Producto agregado al Carro de Compras";

            //Agregar valor a la Session
            var carroLista = await _unidadTrabajo.CarroCompra.get_all(c => c.UsuarioAplicacionId == claim.Value);
            var numeroProductos = carroLista.Count(); //Numero de Registro.
            HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos);

            return RedirectToAction("Index");
        }





        //public async Task<IActionResult> Index()
        //{
        //    IEnumerable<Producto> productoLista = await _unidadTrabajo.Producto.get_all();
        //    return View(productoLista);
        //}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
