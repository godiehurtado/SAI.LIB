using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.Objects;

namespace ColpatriaSAI.Negocio.Componentes.Ajustes
{
    public class AjustesContratos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<LiquiContratFactorParticipante> ListarPagosContratos(int liquidacionContratacion_id)
        {
            return tabla.LiquiContratFactorParticipantes.Include("JerarquiaDetalle.Participante").Where(lcfp => lcfp.liqui_contrat_id == liquidacionContratacion_id).ToList();
        }

        public int ActualizarPagosContratos(String usuario, List<LiquiContratFactorParticipante> listaPagosContratos, string Username)
        {
            List<LiquiContratFactorParticipante> listaPagosContratos_actual = tabla.LiquiContratFactorParticipantes.ToList();

            foreach (LiquiContratFactorParticipante detallePago in listaPagosContratos)
            {
                var detalleP = listaPagosContratos_actual.Where(pc => pc.id == detallePago.id).First();
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
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LiquiContratFactorParticipante,
                    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, detalleP);
            }

            return tabla.SaveChanges();
        }
    }
}