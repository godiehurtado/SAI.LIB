using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class PeriodosCierre
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<PeriodoCierre> ListarPeriodos()
        {
            return contexto.PeriodoCierres.Include("Compania").ToList();
        }

        public List<PeriodoCierre> ListarPeriodosPorCompania(int idCompania)
        {
            return contexto.PeriodoCierres.Include("Compania").Where(x => x.compania_id == idCompania).ToList();
        }

        public int InsertarPeriodoCierre(PeriodoCierre periodo, string Username)
        {
            contexto.PeriodoCierres.AddObject(periodo);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.PeriodoCierre,
    SegmentosInsercion.Personas_Y_Pymes, null, periodo);
            return contexto.SaveChanges();

        }
        public int EliminarPeriodoCierre(int idPeriodo, string Username)
        {
            PeriodoCierre periodo = contexto.PeriodoCierres.Where(e => e.id == idPeriodo).FirstOrDefault();
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.PeriodoCierre,
    SegmentosInsercion.Personas_Y_Pymes, periodo, null);
            contexto.PeriodoCierres.DeleteObject(periodo);
            return contexto.SaveChanges();

        }

        public List<PeriodoCierre> TraerPeriodoCierrePorId(int idPeriodo)
        {
            List<PeriodoCierre> periodo = contexto.PeriodoCierres.Where(e => e.id == idPeriodo).ToList();
            return periodo;
        }

        public int ActualizarPeriodoCierre(PeriodoCierre periodo, string Username)
        {
            var periodoCierre = contexto.PeriodoCierres.Where(e => e.id == periodo.id).FirstOrDefault();
            var pValorAntiguo = periodoCierre;

            periodoCierre.fechaInicio = periodo.fechaInicio;
            periodoCierre.fechaFin = periodo.fechaFin;
            periodoCierre.fechaCierre = periodo.fechaCierre;
            periodoCierre.compania_id = periodo.compania_id;
            periodoCierre.anioCierre = periodo.anioCierre;
            periodoCierre.mesCierre = periodo.mesCierre;
            periodoCierre.estado = periodo.estado;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PeriodoCierre,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, periodoCierre);
            return contexto.SaveChanges();

        }

        public int ActualizarEstadoPeriodoCierre(int idPeriodo, int idEstado, string Username)
        {
            PeriodoCierre periodoCierre = contexto.PeriodoCierres.Where(e => e.id == idPeriodo).FirstOrDefault();
            var pValorAntiguo = periodoCierre;
            periodoCierre.estado = idEstado;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PeriodoCierre,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, periodoCierre);
            return contexto.SaveChanges();
        }

        public int SPPeriodoCierre(int companiaId, int mesCierre, int anioCierre)
        {

            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "SAI_AbrirMesCerrado";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("compania", companiaId));
            command.Parameters.Add(new SqlParameter("mesCierre", mesCierre));
            command.Parameters.Add(new SqlParameter("anioCierre", anioCierre));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 240; }

            int result;

            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }

            return result;

        }

        public int DeleteReprocesos(int mesCierre, int añoCierre)
        {
            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection dbConnection = entityConnection.StoreConnection;
            DbCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = "SAI_DeleteReprocesos";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("mesCierre", mesCierre));
            cmd.Parameters.Add(new SqlParameter("añoCierre", añoCierre));

            bool openingConnection = cmd.Connection.State == ConnectionState.Closed;

            if (openingConnection) { 
                cmd.Connection.Open(); cmd.CommandTimeout = 240; 
            }

            int result;

            try
            {
                result = cmd.ExecuteNonQuery();
            }
            finally
            {
                if(openingConnection && cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Connection.Close();
                }
            }

            return result;

        }

    }
}
