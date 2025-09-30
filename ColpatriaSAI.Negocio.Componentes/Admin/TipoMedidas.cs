using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class TipoMedidas
    {

        private SAI_Entities contexto = new SAI_Entities();

        public List<Entidades.TipoMedida> ListarTipoMedidas()
        {
            return contexto.TipoMedidas.Where(t => t.id > 0).OrderBy(e => e.nombre).ToList();
        }
    }
}
