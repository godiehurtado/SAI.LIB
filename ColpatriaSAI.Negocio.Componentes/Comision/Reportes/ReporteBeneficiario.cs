using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ColpatriaSAI.Datos;
using System.Data.EntityClient;
using System.Data.SqlClient;
using Entidades = ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class ReporteBeneficiario
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<ReporteBeneficiarioClass> GetReporte(int anio = default(int), int mes = default(int), string ClavesAsesor = default(string), string IdDirector = default(string), string IdGerente = default(string)
            , int CanalVentasId = default(int), int SubCanalId = default(int), int TipoPlan = default(int), int TipoContrato = default(int), int modelo = default(int), int sucursal = default(int))
        {
            List<ReporteBeneficiarioClass> lResult = new List<ReporteBeneficiarioClass>();
            DataSet DatasetR = new DataSet();
            
            try
            {
                EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                SqlCommand cmd = new SqlCommand("MAC_ReporteBeneficiario", sqlConn);
                cmd.CommandTimeout = 6000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@año", Convert.ToInt32(anio));
                cmd.Parameters.Add("@mes", Convert.ToInt32(mes));
                cmd.Parameters.Add("@clave", Convert.ToString(ClavesAsesor));
                cmd.Parameters.Add("@modelo_id", Convert.ToInt32(modelo));
                cmd.Parameters.Add("@sucursal_id", Convert.ToInt32(sucursal));
                cmd.Parameters.Add("@lidercomercial_id", Convert.ToInt32(IdDirector));
                cmd.Parameters.Add("@lideroficina_id", Convert.ToInt32(IdGerente));
                cmd.Parameters.Add("@canal_id", Convert.ToInt32(CanalVentasId));
                cmd.Parameters.Add("@canaldetalle_id", Convert.ToInt32(SubCanalId));
                cmd.Parameters.Add("@plandetalle_id", Convert.ToInt32(TipoPlan));
                cmd.Parameters.Add("@tipocontrato_id", Convert.ToInt32(TipoContrato));

                SqlDataAdapter daReport = new SqlDataAdapter(cmd);
                daReport.Fill(DatasetR);
                sqlConn.Close();

                if (DatasetR.Tables[0].Rows.Count <= 0)
                {
                    return lResult;
                }
                else
                {
                    foreach (DataRow lRow in DatasetR.Tables[0].Rows)
                    {

                        lResult.Add(new ReporteBeneficiarioClass()
                        {
                            CLAVE = (lRow["CLAVE"] == DBNull.Value ? string.Empty : lRow["CLAVE"].ToString()),
                            SUCURSAL = (lRow["SUCURSAL"] == DBNull.Value ? string.Empty : lRow["SUCURSAL"].ToString()),
                            PRODUCTO = (lRow["PRODUCTO"] == DBNull.Value ? string.Empty : lRow["PRODUCTO"].ToString()),
                            PLAN = (lRow["PLAN"] == DBNull.Value ? string.Empty : lRow["PLAN"].ToString()),
                            TIPO_CONTRATO = (lRow["TIPO_CONTRATO"] == DBNull.Value ? string.Empty : lRow["TIPO_CONTRATO"].ToString()),
                            BENEFICIARIOS_NUEVOS = (lRow["BENEFICIARIOS_NUEVOS"] == DBNull.Value ? string.Empty : lRow["BENEFICIARIOS_NUEVOS"].ToString()),
                            EQUIVALENCIAS_POR_NUEVOS = (lRow["EQUIVALENCIAS_POR_NUEVOS"] == DBNull.Value ? string.Empty : lRow["EQUIVALENCIAS_POR_NUEVOS"].ToString()),
                            BENEFICIARIOS_AÑO_ACTUAL = (lRow["BENEFICIARIOS_AÑO_ACTUAL"] == DBNull.Value ? string.Empty : lRow["BENEFICIARIOS_AÑO_ACTUAL"].ToString()),
                            BENEFICIARIOS_AÑO_ANTERIOR = (lRow["BENEFICIARIOS_AÑO_ANTERIOR"] == DBNull.Value ? string.Empty : lRow["BENEFICIARIOS_AÑO_ANTERIOR"].ToString()),                            
                            TOTAL_BENEFICIARIOS_NETOS = (lRow["TOTAL_BENEFICIARIOS_NETOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_BENEFICIARIOS_NETOS"].ToString()),
                            TOTAL_EQUIVALENCIAS_POR_NETOS = (lRow["TOTAL_EQUIVALENCIAS_POR_NETOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_EQUIVALENCIAS_POR_NETOS"].ToString())
                        });
                    }
                    return lResult;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lResult;
        }

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
    }
}
