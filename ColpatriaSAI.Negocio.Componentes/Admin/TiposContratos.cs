using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class TiposContratos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Entidades.TipoContrato> ListarTipoContratos()
        {
            return tabla.TipoContratoes.ToList();
        }
    }
}
