using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Segmentos
    {
        private SAI_Entities tabla = new SAI_Entities();


        public List<Segmento> ListarSegmento()
        {
            return tabla.Segmentoes.Where(s => s.id > 0).ToList();
        }


        public List<Segmento> ListarSegmentoesPorId(int idSegmento)
        {
            return tabla.Segmentoes.Where(Segmento => Segmento.id == idSegmento && Segmento.id > 0).ToList();
        }
        public int InsertarSegmento(Segmento segmento, string Username)
        {
            int resultado = 0;
            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Segmentoes.Where(Segmento => Segmento.nombre == segmento.nombre).ToList().Count() == 0)
            {
                tabla.Segmentoes.AddObject(segmento);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Segmento,
                      SegmentosInsercion.Personas_Y_Pymes, null, segmento);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }


        public int ActualizarSegmento(int id, Segmento segmento, string Username)
        {
            int resultado = 0;
            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Segmentoes.Where(Segmento => Segmento.nombre == segmento.nombre).ToList().Count() == 0)
            {
                var segmentoActual = this.tabla.Segmentoes.Where(Segmento => Segmento.id == id && Segmento.id > 0).First();
                var pValorAntiguo = segmentoActual;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Segmento,
                      SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, segmentoActual);
                segmentoActual.nombre = segmento.nombre;
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public string EliminarSegmento(int id, Segmento segmento, string Username)
        {
            var segmentoActual = this.tabla.Segmentoes.Where(Segmento => Segmento.id == id && Segmento.id > 0).First();
            tabla.DeleteObject(segmentoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Segmento,
                      SegmentosInsercion.Personas_Y_Pymes, segmentoActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {

                return ex.Message;

            }

            return "";
        }
    }
}

