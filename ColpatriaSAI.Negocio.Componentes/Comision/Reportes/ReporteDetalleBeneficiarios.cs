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
    public class ReporteDetalleBeneficiarios
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<ReporteDetalleBeneficiariosClass> GetReporte(int anio = default(int), int mes = default(int), string ClavesAsesor = default(string), string IdDirector = default(string), string IdGerente = default(string)
            , int CanalVentasId = default(int), int SubCanalId = default(int), int TipoPlan = default(int), int TipoContrato = default(int), int modelo = default(int), int sucursal = default(int)
            , string numerocontrato = default(string), int estadoBeneficiario = default(int) )
        {
            List<ReporteDetalleBeneficiariosClass> lResult = new List<ReporteDetalleBeneficiariosClass>();
            DataSet DatasetR = new DataSet();

            try
            {
                EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                SqlCommand cmd = new SqlCommand("MAC_ReporteDetalleBeneficiarios", sqlConn);
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
                cmd.Parameters.Add("@contrato", Convert.ToString(numerocontrato));
                cmd.Parameters.Add("@estado", Convert.ToInt32(estadoBeneficiario));

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

                        lResult.Add(new ReporteDetalleBeneficiariosClass()
                        {
                            CLAVE = (lRow["CLAVE"] == DBNull.Value ? string.Empty : lRow["CLAVE"].ToString()),
                            SUCURSAL = (lRow["SUCURSAL"] == DBNull.Value ? string.Empty : lRow["SUCURSAL"].ToString()),
                            PRODUCTO = (lRow["PRODUCTO"] == DBNull.Value ? string.Empty : lRow["PRODUCTO"].ToString()),
                            PLAN = (lRow["PLAN"] == DBNull.Value ? string.Empty : lRow["PLAN"].ToString()),
                            TIPO_CONTRATO = (lRow["TIPO_CONTRATO"] == DBNull.Value ? string.Empty : lRow["TIPO_CONTRATO"].ToString()),
                            CONTRATO = (lRow["CONTRATO"] == DBNull.Value ? string.Empty : lRow["CONTRATO"].ToString()),
                            DOCUMENTO = (lRow["DOCUMENTO"] == DBNull.Value ? string.Empty : lRow["DOCUMENTO"].ToString()),
                            NOMBRE = (lRow["NOMBRE"] == DBNull.Value ? string.Empty : lRow["NOMBRE"].ToString()),
                            APELLIDO = (lRow["APELLIDO"] == DBNull.Value ? string.Empty : lRow["APELLIDO"].ToString()),
                            EDAD = (lRow["EDAD"] == DBNull.Value ? string.Empty : lRow["EDAD"].ToString()),
                            ESTADO = (lRow["ESTADO"] == DBNull.Value ? string.Empty : lRow["ESTADO"].ToString())
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
    }
}
