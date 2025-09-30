using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Productos
{
    public class LineaNegocios
    {
        private SAI_Entities tabla = new SAI_Entities();


        public List<LineaNegocio> ListarLineaNegocios()
        {
            return tabla.LineaNegocios.Where(l => l.id > 0).OrderBy(e => e.nombre).ToList();
        }


        public List<LineaNegocio> ListarLineanegociosPorId(int idLineaNegocio)
        {
            return tabla.LineaNegocios.Where(LineaNegocio => LineaNegocio.id == idLineaNegocio && LineaNegocio.id > 0).ToList();
        }

    }
}
