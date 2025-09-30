using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class LiquidacionMonedas
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<LiquidacionMoneda> ListarLiquidacionesMoneda(int tipo)
        {
            return contexto.LiquidacionMonedas.Include("Moneda").Include("Compania").Include("Participante").Where(e => e.tipo == tipo).ToList();
        }

        public int GuardarLiquidacionMoneda(LiquidacionMoneda liquidacionMoneda, string Username)
        {
            contexto.LiquidacionMonedas.AddObject(liquidacionMoneda);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.LiquidacionMoneda,
    SegmentosInsercion.Personas_Y_Pymes, null, liquidacionMoneda);
            return contexto.SaveChanges();
        }

        public int BorrarColquinesManuales(DateTime fechaCargue, string Username)
        {
            int result = 1;

            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "EliminarColquinesManuales";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("fechaCargue", fechaCargue));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); }

            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LiquidacionMoneda,
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
