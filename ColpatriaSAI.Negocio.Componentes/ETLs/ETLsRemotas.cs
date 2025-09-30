using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.ETLs
{
    public class ETLsRemotas
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<ETLRemota> ListarETLsRemotas()
        {
            return tabla.ETLRemotas.ToList();
        }

        public ETLRemota ObtenerETLRemotaporId(int id)
        {
            return tabla.ETLRemotas.Where(e => e.id == id).FirstOrDefault();
        }

        public List<ETLRemota> ListarETLsRemotasporTipo(int tipoETLRemota_id)
        {
            return tabla.ETLRemotas.Where(e => e.tipoETLRemota_id == tipoETLRemota_id).ToList();
        }
    }
}