using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Ajustes
{
    public class AjustesFranquicias
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<DetallePagosFranquicia> ListarPagosFranquicia(int liquidacionFranquicia_id)
        {
            var pagoFranquicia = tabla.PagoFranquicias.Where(pf => pf.liquidacionFranquicia_id == liquidacionFranquicia_id).First();

            return tabla.DetallePagosFranquicias.Include("AuditoriaAjustes").Where(dpf => dpf.pagoFranquicia_id == pagoFranquicia.id).ToList();
        }

        public int ActualizarPagosFranquicia(String usuario, List<DetallePagosFranquicia> listaPagosFranquicia, string Username)
        {
            List<DetallePagosFranquicia> listaPagosFranquicia_actual = tabla.DetallePagosFranquicias.ToList();

            foreach (DetallePagosFranquicia detallePago in listaPagosFranquicia)
            {
                var detalleP = listaPagosFranquicia_actual.Where(pf => pf.id == detallePago.id).First();
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
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.DetallePagosFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, detalleP);
            }

            return tabla.SaveChanges();
        }
    }
}