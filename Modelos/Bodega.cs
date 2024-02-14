using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos
{
    public class Bodega
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Nombre es requerido.")]
        [MaxLength(60,ErrorMessage = "Nombre no debe ser mayor a 60 caracteres.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Descripcio es requerido.")]
        [MaxLength(100, ErrorMessage = "Descripcion no debe ser mayor a 100 caracteres.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Estado es requerido.")]
        public bool Estado { get; set; }
    }
}
