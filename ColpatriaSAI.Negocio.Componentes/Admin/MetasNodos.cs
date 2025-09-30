using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class MetasNodos
    {

        private SAI_Entities contexto = new SAI_Entities();

        public List<Entidades.MetaxNodo> ListarMetasNodos()
        {
            return contexto.MetaxNodoes.ToList();
        }

        public List<Entidades.MetaxNodo> ListarMetasxJerarquiaId(int idJerarquia)
        {
            return contexto.MetaxNodoes.Include("Meta").Where(m => m.jerarquiaDetalle_id == idJerarquia).ToList();
        }

        public int InsertarMetaNodo(Entidades.MetaxNodo metaNodo, string Username)
        {
            //VALIDAMOS QUE EL REGISTRO NO ESTE EN LA BASE DE DATOS
            var metasNodo = contexto.MetaxNodoes.Where(m => m.jerarquiaDetalle_id == metaNodo.jerarquiaDetalle_id && m.meta_id == metaNodo.meta_id && m.anio == metaNodo.anio).ToList();
            if (metasNodo == null || metasNodo.Count() == 0)
            {
                contexto.MetaxNodoes.AddObject(metaNodo);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.MetaxNodo,
    SegmentosInsercion.Personas_Y_Pymes, null, metaNodo);
                return contexto.SaveChanges();
            }
            else
                return 0;
        }

        public int EliminarMetaNodo(int idMetaNodo, string Username)
        {
            Entidades.MetaxNodo metaNodo = contexto.MetaxNodoes.Where(m => m.id == idMetaNodo).FirstOrDefault();
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.MetaxNodo,
    SegmentosInsercion.Personas_Y_Pymes, metaNodo, null);
            contexto.MetaxNodoes.DeleteObject(metaNodo);
            return contexto.SaveChanges();

        }

    }

}


