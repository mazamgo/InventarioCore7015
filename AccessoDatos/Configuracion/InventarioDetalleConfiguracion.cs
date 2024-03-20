using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Configuracion
{
    public class InventarioDetalleConfiguracion : IEntityTypeConfiguration<InventarioDetalle>
    {
        public void Configure(EntityTypeBuilder<InventarioDetalle> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.InventarioId).IsRequired();
            builder.Property(x => x.StockAnterior).IsRequired();
            builder.Property(x => x.Cantidad).IsRequired();

            /*Relaciones */

            builder.HasOne(x => x.Inventario).WithMany()
            .HasForeignKey(x=>x.InventarioId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Producto).WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.NoAction);          

        }

    }
}
