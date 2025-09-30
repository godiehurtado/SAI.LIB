using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;

namespace ColpatriaSAI.Negocio.Componentes.Produccion
{
    class Negocio
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<ColpatriaSAI.Negocio.Entidades.Negocio> ListarNegociosConCompanias()

        {


            return contexto.Negocios.Include("Compania").ToList();
        }

    }
}
