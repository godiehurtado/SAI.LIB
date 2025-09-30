using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Administracion
{
    public class AdminComision:IAdminComision     
    {
        public List<ConfigParametros> ObtenerParametros(String[] parametros)
        {
            return new Administracion().ObtenerParametros(parametros);
        }
    }
}
