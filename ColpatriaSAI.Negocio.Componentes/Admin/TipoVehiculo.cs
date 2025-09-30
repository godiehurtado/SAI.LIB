using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class TipoVehiculo
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<Entidades.TipoVehiculo> ListarTipoVehiculos()
        {
            return contexto.TipoVehiculoes.Where(t => t.id >0).OrderBy(e=>e.Nombre).ToList();
        }

        public List<Entidades.TipoVehiculo> ListarTipoVehiculosporRamo(int ramo_id)
        {
            return contexto.TipoVehiculoes.Where(t => t.ramo_id == ramo_id).OrderBy(t => t.Nombre).ToList();
        }
    }
}
