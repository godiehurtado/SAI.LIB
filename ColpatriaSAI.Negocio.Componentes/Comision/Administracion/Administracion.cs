using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ColpatriaSAI.Datos;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Administracion
{
    public class Administracion
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<ConfigParametros> ObtenerParametros(String[] parametros)
        {
            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select * from SAI_COMISIONES_CONFIG ");
            lSentencia.Append(" where id in ("+String.Join(",",parametros)+")");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var Parametros = from item in retVal.Tables[0].AsEnumerable()
                             select new ConfigParametros()
                                {
                                    id = Convert.ToInt32(item["id"].ToString()),
                                    parametro = item["parametro"].ToString(),
                                    valor = item["valor"].ToString(),
                                    descripcion = item["descripcion"].ToString(),
                                };


            return Parametros.ToList();
            
        }

        public ResultadoOperacionBD EditarParametro(Int32 id, string valor,string usuario)
        {
            ResultadoOperacionBD resultadoOp = new ResultadoOperacionBD();

            try
            {
              


                StringBuilder lSentencia2 = new StringBuilder();
                lSentencia2.Append("select * from SAI_COMISIONES_CONFIG ");
                lSentencia2.Append(" where id in (" + String.Join(",", id) + ")");

                DataSet retVal = new DataSet();
                EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                SqlCommand cmdReport = new SqlCommand(lSentencia2.ToString(), sqlConn);
                SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
                using (cmdReport)
                {
                    daReport.Fill(retVal);
                }

                var Val = from item in retVal.Tables[0].AsEnumerable()
                          select new ConfigParametros()
                          {
                              id = Convert.ToInt32(item["id"].ToString()),
                              parametro = item["parametro"].ToString(),
                              valor = item["valor"].ToString(),
                              descripcion = item["descripcion"].ToString(),
                          };

                string pValorAntiguo = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        INICIO =id,
                        Valor = Val.ToList().First().valor.ToString(),
                         FIN = Val.ToList().First().valor.ToString()

                    }); ;


                string pValorANuevo = Utilidades.Auditoria.CrearDescripcionAuditoria(
                   new
                   {
                       INICIO = id,
                       Valor = valor,
                       FIN = valor

                   }); ;


                // Log Auditoria
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, usuario, tablasAuditadas.SAI_COMISIONES_CONFIG,
               SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, pValorANuevo);

                StringBuilder lSentencia3 = new StringBuilder();
                lSentencia3.Append("update SAI_COMISIONES_CONFIG set valor='" + valor + "'");
                lSentencia3.Append(" where id =" + id);

                DataSet retVal2 = new DataSet();
                EntityConnection entityConn2 = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn2 = (SqlConnection)entityConn2.StoreConnection;
                sqlConn2.Open();
                SqlCommand cmdReport2 = new SqlCommand(lSentencia3.ToString(), sqlConn2);

                resultadoOp.RegistrosAfectados = cmdReport2.ExecuteNonQuery();
                if (resultadoOp.RegistrosAfectados < 0)
                    resultadoOp.MensajeError = "No se encontró el id del parámetro a editar";
                sqlConn.Close();
                sqlConn2.Close();
            }
            catch (Exception ex)
            {
                resultadoOp.MensajeError = ex.Message;
            }
            
            return resultadoOp;

        }

        public List<parmbhExtractNoveades> ObtenerListNovedades(String[] parametros)
        {
            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select * from Bh_Extract_Novedades ");
            if (parametros.Length > 0)

            lSentencia.Append(" where NOV_NCODE in (" + String.Join(",", parametros) + ")");
            lSentencia.Append(" order by NOV_CDESCRIPTION asc");
            

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var Parametros = from item in retVal.Tables[0].AsEnumerable()
                             select new parmbhExtractNoveades()
                             {
                                 NovNcode = Convert.ToInt32(item["NOV_NCODE"].ToString()),
                                 NovCDescription = item["NOV_CDESCRIPTION"].ToString(),
                                 NovCtType = item["NOV_CTYPE"].ToString(),
                                 Nca_Cdescription = item["NCA_CDESCRIPTION"].ToString(),
                             };


            return Parametros.ToList();

        }

        public List<parmbhExtractNoveades> ObtenerListNovedadesAdd(String[] parametros)
        {
            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select * from Bh_Extract_Novedades ");
            if (parametros.Length > 0)
                lSentencia.Append(" where NOV_NCODE not in (" + String.Join(",", parametros) + ")");
            lSentencia.Append(" order by NOV_CDESCRIPTION asc");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var Parametros = from item in retVal.Tables[0].AsEnumerable()
                             select new parmbhExtractNoveades()
                             {
                                 NovNcode = Convert.ToInt32(item["NOV_NCODE"].ToString()),
                                 NovCDescription = item["NOV_CDESCRIPTION"].ToString(),
                                 NovCtType = item["NOV_CTYPE"].ToString(),
                                 Nca_Cdescription = item["NCA_CDESCRIPTION"].ToString(),
                             };


            return Parametros.ToList();

        }



    }
}
