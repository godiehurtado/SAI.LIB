using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Ajustes
{
    public class AjustesConcursos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<DetallePagosRegla> ListarPagosConcurso(int liquidacionRegla_id)
        {
            return tabla.DetallePagosReglas.Where(dpr => dpr.liquidacionRegla_id == liquidacionRegla_id).ToList();
        }

        public int ActualizarPagosConcurso(String usuario, List<DetallePagosRegla> listaPagosConcurso, string Username)
        {
            List<DetallePagosRegla> listaPagosConcurso_actual = tabla.DetallePagosReglas.ToList();

            foreach (DetallePagosRegla detallePago in listaPagosConcurso)
            {
                var detalleP = listaPagosConcurso_actual.Where(pc => pc.id == detallePago.id).First();
                var pValorAntiguo = detalleP;
                detalleP.valorAjuste = detallePago.valorAjuste;
                if (detalleP.totalAjuste == null)
                    detalleP.totalAjuste = detallePago.valorAjuste;
                else
                    detalleP.totalAjuste += detallePago.valorAjuste;

                AuditoriaAjuste auditAjuste = new AuditoriaAjuste();
                auditAjuste.descripcion = detallePago.AuditoriaAjustes.First().descripcion;
                auditAjuste.valor = detallePago.valorAjuste.Value;
                auditAjuste.usuario = usuario;
                auditAjuste.fecha = DateTime.Now;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.AuditoriaAjuste,
    SegmentosInsercion.Personas_Y_Pymes, null, auditAjuste);
                detalleP.AuditoriaAjustes.Add(auditAjuste);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.DetallePagosRegla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, detalleP);
            }

            return tabla.SaveChanges();
        }
    }
}