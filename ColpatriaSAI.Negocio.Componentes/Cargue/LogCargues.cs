using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Cargue
{
    public class LogCargues
    {
        private static SAI_Entities tabla = new SAI_Entities();

        public static int InsertarLogCargue(LogCargue detalle)
        {
            tabla.LogCargues.AddObject(detalle);
            return tabla.SaveChanges();
        }

        public static List<LogCargue> ListarLogCargue(int cargue_id, int cargue_tipo)
        {
            return tabla.LogCargues.Where(l => l.cargue_id == cargue_id && l.cargue_tipo == cargue_tipo).ToList();
        }

        public static void BorrarLogCargue(int cargue_id, int cargue_tipo)
        {
            tabla.ExecuteStoreCommand("DELETE FROM LogCargue WHERE cargue_id = {0} AND cargue_tipo = {1}", cargue_id, cargue_tipo);
        }
    }
}