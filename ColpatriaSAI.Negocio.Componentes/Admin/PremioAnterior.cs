using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Web.Mvc;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class PremioAnterior
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<PremiosAnteriore> ListarPremioAnterior()
        {
            return tabla.PremiosAnteriores.ToList();
        }

        public List<PremiosAnteriore> ListarPremioAnteriorPorId(int idPAnterior)
        {
            return tabla.PremiosAnteriores.Where(pa => pa.id == idPAnterior).ToList();
        }

        public int InsertarPremioAnterior(PremiosAnteriore premioanterior, string Username)
        {
            int resultado = 0;

            if (tabla.PremiosAnteriores.Where(pa => pa.clave == premioanterior.clave && pa.año == premioanterior.año
                && pa.FASECOLDA == premioanterior.FASECOLDA && pa.LIMRA == premioanterior.LIMRA).ToList().Count() == 0)
            {
                tabla.PremiosAnteriores.AddObject(premioanterior);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.PremiosAnteriores,
                      SegmentosInsercion.Personas_Y_Pymes, null, premioanterior);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public int ActualizarPremioAnterior(int id, PremiosAnteriore premioanterior, string Username)
        {
            int resultado = 0;
            if (tabla.PremiosAnteriores.Where(pa => pa.clave == premioanterior.clave && pa.año == premioanterior.año
                && pa.FASECOLDA == premioanterior.FASECOLDA && pa.LIMRA == premioanterior.LIMRA).ToList().Count() == 0)
            {
                var pAnteriorActual = this.tabla.PremiosAnteriores.Where(pa => pa.id == id).First();
                var pValorAntiguo = pAnteriorActual;
                pAnteriorActual.clave = premioanterior.clave;
                pAnteriorActual.año = premioanterior.año;
                pAnteriorActual.FASECOLDA = premioanterior.FASECOLDA;
                pAnteriorActual.LIMRA = premioanterior.LIMRA;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PremiosAnteriores,
                      SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, pAnteriorActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public string EliminarPremioAnterior(int id, PremiosAnteriore premioanterior, string Username)
        {
            var pAnteriorActual = this.tabla.PremiosAnteriores.Where(pa => pa.id == id).First();
            tabla.DeleteObject(pAnteriorActual);

            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Compania,
                      SegmentosInsercion.Personas_Y_Pymes, pAnteriorActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        public int ActualizarPremioConsolidadoMes(string clave, int anio, int tipo, string Username)
        {
            int result = 0;
            {
                EntityConnection entityConnection = (EntityConnection)tabla.Connection;
                DbConnection storeConnection = entityConnection.StoreConnection;
                DbCommand command = storeConnection.CreateCommand();
                command.CommandText = "ActualizarPremioConsolidadoMes";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("clave", clave));
                command.Parameters.Add(new SqlParameter("anio", anio));
                command.Parameters.Add(new SqlParameter("tipo", tipo));

                bool openingConnection = command.Connection.State == ConnectionState.Closed;

                if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 1200; }
                try
                {
                    result = command.ExecuteNonQuery();
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PremiosAnteriores,
                      SegmentosInsercion.Personas_Y_Pymes, null, null);
                }
                finally
                {
                    if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
                }

            }
            return 0;
        }

        public string RetornarClavePremio(int id)
        {
            var q1 = (from pa in tabla.PremiosAnteriores
                      where (pa.id == id)
                      select pa.clave).First();

            string clave = q1;

            return clave;
        }

        public int RetornarAnioPremio(int id)
        {
            var q1 = (from pa in tabla.PremiosAnteriores
                      where (pa.id == id)
                      select pa.año).First();

            int anio = (int)q1;

            return anio;
        }
    }
}
