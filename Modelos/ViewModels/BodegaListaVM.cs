using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.ViewModels
{
	public class BodegaListaVM
	{
		[Key]
		public int Id { get; set; }
		
		[MaxLength(60, ErrorMessage = "Nombre no debe ser mayor a 60 caracteres.")]
		public string Nombre { get; set; }
		
		[MaxLength(100, ErrorMessage = "Descripcion no debe ser mayor a 100 caracteres.")]
		public string Descripcion { get; set; }
		
		public bool Estado { get; set; }
	}
}
