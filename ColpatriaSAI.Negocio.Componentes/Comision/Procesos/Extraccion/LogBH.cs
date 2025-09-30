using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Negocio.Entidades.CustomEntities;
using ColpatriaSAI.Datos;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Procesos.Extraccion
{
    public class LogBH
    {


        private SAI_Entities _dbcontext = new SAI_Entities();

        private string GetCadenasConsulta(string pCadena)
        {
            StringBuilder lCadenaConvertida = new StringBuilder();
            var pCadenas = pCadena.Split(',');

            foreach (var item in pCadenas)
            {
                lCadenaConvertida.AppendFormat("'{0}',", item);
            }

            return lCadenaConvertida.ToString().Trim(',');
        }

        private DataTable GetResults(string lSentencia)
        {
            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia, sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            return retVal.Tables[0];
        }

        public int ConsultarAdicional(string pSentencia)
        {
            int result = 0;

            result = _dbcontext.ExecuteStoreQuery<int?>(pSentencia, null).Select(i => (i == null) ? 0 : Convert.ToInt32(i)).FirstOrDefault();
            return result;
        }

        public List<LogProcesoBH> GetLog()
        {
            List<LogProcesoBH> lResult = new List<LogProcesoBH>();

            return lResult;
        }
    }
}
