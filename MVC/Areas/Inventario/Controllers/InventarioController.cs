using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Modelos;
using Modelos.ViewModels;
using Rotativa.AspNetCore;
using System.Security.Claims;
using Utilidades;

namespace MVC.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Inventario)]
    public class InventarioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty] //Este modelo lo vamos ha esta utilizando en todo el proyecto y Debito a que va estar constantemente cambiando su valor.
        public InventarioVM inventarioVM { get; set; }

        public InventarioController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult NuevoInventario()
        {
            inventarioVM = new InventarioVM()
            {
                Inventario = new Modelos.Inventario(),
                BodegaLista = _unidadTrabajo.Inventario.ObtenerTodosDropdownList("Bodega")
            };

            inventarioVM.Inventario.Estado = false;
            
            //Obtener el usr desde la session.
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            inventarioVM.Inventario.UsuarioAplicacionId = claim.Value;
            inventarioVM.Inventario.FechaInicial = DateTime.Now;
            inventarioVM.Inventario.FechaFinal = DateTime.Now;
            inventarioVM.Inventario.Estado = true;

            
            return View(inventarioVM)
;        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> NuevoInventario(InventarioVM inventarioVM)
        {
            // Limpiar el estado del modelo para que solo valide el objeto Inventario
            ModelState.Clear();

            // Agregar solo el estado del modelo Inventario
            foreach (var property in typeof(InventarioVM).GetProperties())
            {
                if (ModelState.ContainsKey(property.Name))
                {
                    ModelState[property.Name].Errors.Clear();
                }
            }
            
            if (TryValidateModel(inventarioVM.Inventario.BodegaId))
            {
				if (ModelState.IsValid)
				{
					inventarioVM.Inventario.FechaInicial = DateTime.Now;
					inventarioVM.Inventario.FechaFinal = DateTime.Now;
					await _unidadTrabajo.Inventario.Add(inventarioVM.Inventario);
					await _unidadTrabajo.Guardar();

					return (RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id }));
				}

				inventarioVM.BodegaLista = _unidadTrabajo.Inventario.ObtenerTodosDropdownList("Bodega");
				return View(inventarioVM);

			}

			inventarioVM.BodegaLista = _unidadTrabajo.Inventario.ObtenerTodosDropdownList("Bodega");
			return View(inventarioVM);


		}

        public async Task<IActionResult> DetalleInventario(int id)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.get_Firts(x => x.Id == id,incluirPropiedades:"Bodega");
            inventarioVM.InventarioDetalles = await _unidadTrabajo.InventarioDetalle.get_all(x => x.InventarioId == id, incluirPropiedades:"Producto,Producto.Marca");

            return View(inventarioVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetalleInventario(int InventarioId, int productoId, int cantidadId)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.get_Firts(i => i.Id == InventarioId);
            var bodegaProducto =  await _unidadTrabajo.bodegaProducto.get_Firts(b => b.Id == productoId && b.BodegaId == inventarioVM.Inventario.BodegaId);

            var detalle = await _unidadTrabajo.InventarioDetalle.get_Firts(d => d.InventarioId == InventarioId && d.ProductoId == productoId);
            
            if(detalle == null)
            {
                inventarioVM.InventarioDetalle = new Modelos.InventarioDetalle();
                inventarioVM.InventarioDetalle.ProductoId = productoId;
                inventarioVM.InventarioDetalle.InventarioId = InventarioId;

                if(bodegaProducto != null)
                {
                    inventarioVM.InventarioDetalle.StockAnterior = bodegaProducto.Cantidad;
                }
                else
                {
                    inventarioVM.InventarioDetalle.StockAnterior = 0;
				}
				
                inventarioVM.InventarioDetalle.Cantidad = cantidadId;
                await _unidadTrabajo.InventarioDetalle.Add(inventarioVM.InventarioDetalle);
                await _unidadTrabajo.Guardar();

			}
            else
            {
                detalle.Cantidad += cantidadId;
                await _unidadTrabajo.Guardar();
            }
            return RedirectToAction("DetalleInventario", new { id = InventarioId });

        }

        public async Task<IActionResult> Mas (int id)
        {
            inventarioVM = new InventarioVM();
            var detalle = await _unidadTrabajo.InventarioDetalle.get(id);
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.get(detalle.InventarioId);

            detalle.Cantidad += 1;
            await _unidadTrabajo.Guardar();
            return RedirectToAction("DetalleInventario", new {id = inventarioVM.Inventario.Id});

        }

        public async Task<IActionResult> Menos(int id)
        {
            inventarioVM = new InventarioVM();
            var detalle = await _unidadTrabajo.InventarioDetalle.get(id);
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.get(detalle.InventarioId);

            if(detalle.Cantidad == 1)
            {
                _unidadTrabajo.InventarioDetalle.Delete(detalle);
                await _unidadTrabajo.Guardar();
            }
            else
            {
                detalle.Cantidad -= 1;
                await _unidadTrabajo.Guardar();               
            }
            return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });

        }


        public async Task<IActionResult> GenerarStock(int id)
        {
            var inventario = await _unidadTrabajo.Inventario.get(id);
            var detalleLista = await _unidadTrabajo.InventarioDetalle.get_all(d => d.InventarioId == id);
            //Obtener el usr desde la session.
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            foreach (var item in detalleLista)
            {
                var bodegaProducto = new BodegaProducto();
                bodegaProducto = await _unidadTrabajo.bodegaProducto.get_Firts(b => b.ProductoId == item.ProductoId &&
                                          b.BodegaId == inventario.BodegaId);    //El isTraking: false es para consultar registro, no para actualizar.                           
                if(bodegaProducto != null)
                {
                    //El Registro de Stock existe, hay que actualizar las cantidades
                    await _unidadTrabajo.KardexInventario.RegistrarKardex(bodegaProducto.Id,"Entrada","Registro de Inventario",bodegaProducto.Cantidad, item.Cantidad, claim.Value);
                    bodegaProducto.Cantidad += item.Cantidad;
                    await _unidadTrabajo.Guardar();
                }
                else
                {
                    //El Registro de Stock no existe, hay que crearlo.
                    bodegaProducto = new BodegaProducto();
                    bodegaProducto.BodegaId = inventario.BodegaId;
                    bodegaProducto.ProductoId = item.ProductoId;
                    bodegaProducto.Cantidad = item.Cantidad;    
                    await _unidadTrabajo.bodegaProducto.Add(bodegaProducto);
                    await _unidadTrabajo.Guardar();
                    await _unidadTrabajo.KardexInventario.RegistrarKardex(bodegaProducto.Id, "Entrada", "Registro de Inventario Inicial", 0, item.Cantidad, claim.Value);

                }
            
            }

            //Actualizar la Cabecera de Inventario.
            inventario.Estado = true;
            inventario.FechaFinal= DateTime.Now;
            await _unidadTrabajo.Guardar();
            TempData[DS.Exitosa] = "Stock Generado con Exitoso.";
            return RedirectToAction("Index");
            
        }

        public IActionResult KardexProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KardexProducto(string fechaInicioId, string fechaFinalId, int productoId)
        {
            //Vamos a redireccionar a otro Metodo.
            return RedirectToAction("KardexProductoResultado", new {fechaInicioId,fechaFinalId,productoId });             
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> KardexProductoResultado(string fechaInicioId, string fechaFinalId, int productoId) 
        { 
            KardexInventarioVM kardexInventarioVM = new KardexInventarioVM();
            kardexInventarioVM.Producto = new Producto();
            kardexInventarioVM.Producto = await _unidadTrabajo.Producto.get(productoId);

            kardexInventarioVM.FechaInicio = DateTime.Parse(fechaInicioId); // 00:00:00
            kardexInventarioVM.FechaFinal = DateTime.Parse(fechaFinalId).AddHours(23).AddMinutes(59);           

            kardexInventarioVM.KardexInventarioLista = await _unidadTrabajo.KardexInventario.get_all(
                k => k.BodegaProducto.ProductoId == productoId && 
                (k.FechaRegistro >= kardexInventarioVM.FechaInicio &&
                k.FechaRegistro <= kardexInventarioVM.FechaFinal),
                incluirPropiedades: "BodegaProducto,BodegaProducto.Producto,BodegaProducto.Bodega",
                orderBy: o => o.OrderBy( o => o.FechaRegistro)
                );                     

            return View(kardexInventarioVM);
        }


        public async Task<IActionResult> ImprimirKardex(DateTime fechaInicio, DateTime fechaFinal, int productoId)
        {
            KardexInventarioVM kardexInventarioVM = new KardexInventarioVM();
            kardexInventarioVM.Producto = new Producto();
            kardexInventarioVM.Producto = await _unidadTrabajo.Producto.get(productoId);

            kardexInventarioVM.FechaInicio = fechaInicio; // 00:00:00
            kardexInventarioVM.FechaFinal = fechaFinal;

            kardexInventarioVM.KardexInventarioLista = await _unidadTrabajo.KardexInventario.get_all(
                k => k.BodegaProducto.ProductoId == productoId &&
                (k.FechaRegistro >= kardexInventarioVM.FechaInicio &&
                k.FechaRegistro <= kardexInventarioVM.FechaFinal),
                incluirPropiedades: "BodegaProducto,BodegaProducto.Producto,BodegaProducto.Bodega",
                orderBy: o => o.OrderBy(o => o.FechaRegistro)
                );

            return new ViewAsPdf("ImprimirKardex", kardexInventarioVM)
            {
                FileName = "KardexProducto.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
            };
            //return View(kardexInventarioVM);
        }


        #region API




        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.bodegaProducto.get_all(incluirPropiedades: "Bodega,Producto");
          
            return Json(new {data = todos});
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProducto (string term)
        {
            if(!string.IsNullOrEmpty(term))
            {
                var listaProductos = await _unidadTrabajo.Producto.get_all(p => p.Estado == true);
                var data = listaProductos.Where(p => p.NumeroSerie.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                                                p.Descripcion.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

                return Ok(data);
            }

            return Ok();
        }

        #endregion


    }
}
