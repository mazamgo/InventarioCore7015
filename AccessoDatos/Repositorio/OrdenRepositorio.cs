using AccesoDatos.Data;
using AccessoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio
{
    public class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        private readonly ApplicationDbContext _db;

        //ctor + tab + tab crea el constructor.
        public OrdenRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Orden orden)
        {
            _db.Update(orden);
        }

        void IOrdenRepositorio.ActualizaEstado(int id, string ordenEstado, string pagoEstado)
        {
            var ordenBD = _db.Ordenes.FirstOrDefault(o => o.Id == id);

            if(ordenBD != null)
            {
                ordenBD.EstadoOrden = ordenEstado;
                ordenBD.EstadoPago = pagoEstado;
            }
        }

        void IOrdenRepositorio.ActualizarPagoStripeId(int id, string sessionId, string transaccionId)
        {
            var ordenBD = _db.Ordenes.FirstOrDefault(o => o.Id == id);

            if (ordenBD != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    ordenBD.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(transaccionId))
                {
                    ordenBD.TransaccionId = transaccionId;
                    ordenBD.FechaPago = DateTime.Now;
                }
                
            }
        }
    }
}
