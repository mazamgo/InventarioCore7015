﻿using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoDatos.Repositorio.IRepositorio
{
    public interface ICarroCompraRepositorio : IRepositorio<CarroCompra>
    {
        //Esto se realiza pq cada objeto tiene sus propias propiedades.
        void Actualizar(CarroCompra carroCompra);
    }
}
