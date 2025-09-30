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
    class Dashboard
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<ColpatriaSAI.Negocio.Entidades.Dashboard> TraerDashboard()
        {
            List<ColpatriaSAI.Negocio.Entidades.Dashboard> listDashboard = contexto.Dashboards.ToList();
            return listDashboard;
        }

        public List<ColpatriaSAI.Negocio.Entidades.TipoPanel> TraerDashboardxPanel()
        {
            List<ColpatriaSAI.Negocio.Entidades.TipoPanel> listDashboard = contexto.TipoPanels.Include("Dashboards").Where(d => d.visible == true).ToList();
            return listDashboard.Where(x=>x.Dashboards.Count() > 0).ToList();
        }

        public int ObtenerDatosDashboard()
        {
            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "getDashboard";
            command.CommandType = CommandType.StoredProcedure;

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
    }
}
