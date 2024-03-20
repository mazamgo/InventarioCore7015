using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos
{
    public class KardexInventario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BodegaProductoId { get; set; }
        [ForeignKey("BodegaProductoId")]
        public BodegaProducto BodegaProducto { get; set; }

        [Required]
        [MaxLength(100)]
        public string Tipo { get; set; } //Ent, Sal.

        [Required]
        public string Detalle { get; set; } //Descripcion

        [Required]
        public int StockAnterior { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public Double Costo { get; set; }

        [Required]
        public int Stock { get; set; } //Existencia con lo que queda en el momento.

        [Required]
        public Double Total { get; set; }  //Costo * Stock

        [Required]
        public string UsuarioAplicacionId { get; set; }
        [ForeignKey("UsuarioAplicacionId")]
        public UsuarioAplicacion UsuarioAplicacion { get; set; }

        public DateTime FechaRegistro { get; set; } //Fecha Registro de la Transaccion.





    }
}
