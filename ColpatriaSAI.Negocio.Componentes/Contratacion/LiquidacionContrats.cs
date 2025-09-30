using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using System.Web.Configuration;

namespace ColpatriaSAI.Negocio.Componentes.Contratacion
{
    public class LiquidacionContrats
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<LiquidacionContratacion> ListarLiquidacionContratacion()
        {
            LiquidacionContratacion liquidacionFranquicia = new LiquidacionContratacion();

            return tabla.LiquidacionContratacions.Include("EstadoLiquidacion").Where(e => e.estado != null).ToList();
        }

        public int InsertarLiquidacionContrat(LiquidacionContratacion liquiContrat, string Username)
        {
            tabla.LiquidacionContratacions.AddObject(liquiContrat);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquidacionContratacion,
    SegmentosInsercion.Personas_Y_Pymes, null, liquiContrat);
            tabla.SaveChanges();
            return liquiContrat.id;
        }

        public LiquidacionContratacion TraerLiquidacionContratacion(int idLiquidacionContratacion)
        {
            LiquidacionContratacion liquidacionContratacion = new LiquidacionContratacion();
            liquidacionContratacion = tabla.LiquidacionContratacions.Where(x => x.id == idLiquidacionContratacion).FirstOrDefault();
            return liquidacionContratacion;
        }

        public int InsertarLiquidacionContratMeta(LiquiContratMeta liquiContrat, string Username)
        {
            tabla.LiquiContratMetas.AddObject(liquiContrat);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquiContratMeta,
    SegmentosInsercion.Personas_Y_Pymes, null, liquiContrat);
            tabla.SaveChanges();
            return liquiContrat.id;
        }

        public int InsertarDetalleLiquidacionContratPP(DetalleLiquiContratPpacionPpante liquiContrat, string Username)
        {
            tabla.DetalleLiquiContratPpacionPpantes.AddObject(liquiContrat);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.DetalleLiquiContratPpacionPpante,
    SegmentosInsercion.Personas_Y_Pymes, null, liquiContrat);
            tabla.SaveChanges();
            return liquiContrat.id;
        }

        public int InsertarLiquidacionContratFP(LiquiContratFactorParticipante liquiContrat, string Username)
        {
            tabla.LiquiContratFactorParticipantes.AddObject(liquiContrat);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquiContratFactorParticipante,
    SegmentosInsercion.Personas_Y_Pymes, null, liquiContrat);
            tabla.SaveChanges();
            return liquiContrat.id;
        }

        public int InsertarLiquidacionContratPP(LiquiContratPpacionPpante liquiContrat, string Username)
        {
            tabla.LiquiContratPpacionPpantes.AddObject(liquiContrat);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquiContratPpacionPpante,
    SegmentosInsercion.Personas_Y_Pymes, null, liquiContrat);
            tabla.SaveChanges();
            return liquiContrat.id;
        }

        public int LiquidarContratacion(LiquidacionContratacion liquidacionContratacion, int idSegmento)
        {

            SqlConnection conexion = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString + "; Asynchronous Processing=True;");
            SqlCommand command = conexion.CreateCommand();
            command.CommandText = "LiquidarContratacion_Iniciar";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("idLiquidacion", liquidacionContratacion.id));
            command.Parameters.Add(new SqlParameter("FechaIniParam", liquidacionContratacion.fechaIni));
            command.Parameters.Add(new SqlParameter("FechaFinParam", liquidacionContratacion.fechaFin));
            command.Parameters.Add(new SqlParameter("idSegmentoParam", idSegmento));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 1200; }

            command.BeginExecuteNonQuery(delegate(IAsyncResult ar)
            {
                try { command.EndExecuteNonQuery(ar); }
                finally
                {
                    if (openingConnection && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }
                }
            }, null);


            return 1;
        }

        public int ActualizarLiquidacionContratacionEstado(int idLiquidacion, int idEstado, string Username)
        {
            int result = 0;

            LiquidacionContratacion liquidContratacion = tabla.LiquidacionContratacions.Where(e => e.id == idLiquidacion).FirstOrDefault();
            var pValorAntiguo = liquidContratacion;
            liquidContratacion.estado = idEstado;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LiquidacionContratacion,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, liquidContratacion);
            tabla.SaveChanges();

            return result;
        }

        public int EliminarLiquidacionContratacion(int idLiquidacion, string Username)
        {

            EntityConnection entityConnection = (EntityConnection)tabla.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "LiquidarContratacion_Eliminar";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("idLiquidacion", idLiquidacion));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 500; }

            int result;

            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LiquidacionContratacion,
    SegmentosInsercion.Personas_Y_Pymes, null, null);
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }

            return result;
        }

    }
}