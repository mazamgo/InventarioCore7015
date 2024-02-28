using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;
using Modelos;
using Modelos.ViewModels;
using Utilidades;

namespace MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        //Instanciar nuestra Area de Trabajo
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;   

        //constructor para Iniciarlizar el area de trabajo.
        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownLista("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownLista("Marca"),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownLista("Producto")
            };

            if (id == null)
            {
                // Crear nuevo Producto
                productoVM.Producto.Estado = true;

                return View(productoVM);
            }
            else
            {
                productoVM.Producto = await _unidadTrabajo.Producto.get(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }

        //Metodo Post para el guardado de Producto.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM) 
        { 
            if(!ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(productoVM.Producto.Id == 0)
                {
                    //Crear.
                    string upload = webRootPath + DS.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString(); //Grabar la imagen como ID unico.
                    string extension = Path.GetExtension(files[0].FileName);

                    using(var fileStream = new FileStream(Path.Combine(upload, fileName + extension),FileMode.Create)) 
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Add(productoVM.Producto);

                }
                else
                {
                    //Actualizar el registro.
                    var objProducto = await _unidadTrabajo.Producto.get_Firts(p => p.Id == productoVM.Producto.Id, isTracking: false);
                    if(files.Count > 0)
                    {
						string upload = webRootPath + DS.ImagenRuta;
						string fileName = Guid.NewGuid().ToString(); //Grabar la imagen como ID unico.
						string extension = Path.GetExtension(files[0].FileName);

                        //Borrar la imagen anterior.
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);

                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload,fileName+extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productoVM.Producto.ImagenUrl = fileName + extension;
					}
                    else
                    {
                        //Caso contrario no se carga una nueva imagen.
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }

                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);

                }

                TempData[DS.Exitosa] = "Transaccion Exitosa!";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }

            //Error, si el modelo no es valido.
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownLista("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownLista("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownLista("Producto");
            return View(productoVM);

        }
              
        #region Api

        //Metodo para obtener todos los datos y se va ocupar con el DataTable.
        public async Task<IActionResult> ObtenerTodos() 
        {
            var todos = await _unidadTrabajo.Producto.get_all(incluirPropiedades:"Categoria,Marca");
            return Json(new { data = todos });
        }

        public async Task<IActionResult> Delete(int id) 
        {
            var productoaDb = await _unidadTrabajo.Producto.get(id);
            if(productoaDb == null)
            {
                return Json(new {success = false, messaje = "Error al borrar Producto"});
            }

            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, productoaDb.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }

            _unidadTrabajo.Producto.Delete(productoaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrada exitosamente" });

        }

        [ActionName("ValidarSerie")]
        public async Task<IActionResult> ValidarSerie(string serie, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Producto.get_all();
            if (id == 0)
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim() && b.Id != id);
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
