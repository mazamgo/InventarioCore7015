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
    public class InventarioConfiguracion : IEntityTypeConfiguration<Inventario>
    {
        public void Configure(EntityTypeBuilder<Inventario> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.BodegaId).IsRequired();
            builder.Property(x => x.UsuarioAplicacionId).IsRequired();
            builder.Property(x => x.FechaInicial).IsRequired();
            builder.Property(x => x.FechaFinal).IsRequired();
            builder.Property(x => x.Estado).IsRequired();

            /*Relaciones */

            builder.HasOne(x => x.UsuarioAplicacion).WithMany()
            .HasForeignKey(x=>x.UsuarioAplicacionId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.UsuarioAplicacion).WithMany()
            .HasForeignKey(x => x.UsuarioAplicacionId)
            .OnDelete(DeleteBehavior.NoAction);          

        }

    }
}
