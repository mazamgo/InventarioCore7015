using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.ViewModels
{
	public class InventarioDetalleVM
	{
		[Key]
		public int Id { get; set; }

		public int InventarioId { get; set; }
		[ForeignKey("InventarioId")]
		public Inventario Inventario { get; set; }

		public int ProductoId { get; set; }
		[ForeignKey("ProductoId")]
		public Producto Producto { get; set; }

		public int StockAnterior { get; set; }

		public int Cantidad
		{
			get; set;
		}
	}
}
