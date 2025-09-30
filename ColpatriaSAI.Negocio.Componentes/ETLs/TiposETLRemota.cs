using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.ETLs
{
    public class TiposETLRemota
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<TipoETLRemota> ListarTiposETLRemota()
        {
            return tabla.TipoETLRemotas.ToList();
        }
    }
}