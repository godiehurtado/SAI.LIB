using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class ReporteTransferencia
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

       
        public List<ReporteTrasnferenciasClass> GetReporteTransferencias(DateTime pFechaPeriodo, int pAsesorInicial = default(int), int pAsesorFinal = default(int),
        int pCanalVentas = default(int), int pSubCanal = default(int), int pDirector = default(int), int pGerOfic = default(int), int pGerReg = default(int),
        string pNumeroContrato = default(string), string pSubContrato = default(string), int pTipoPlan = default(int), int pTipoContrato = default(int), int pTipoTransferencia = default(int), int modelo = default(int), int sucursal = default(int))
        {
            List<ReporteTrasnferenciasClass> lResult = new List<ReporteTrasnferenciasClass>();


            string comando = "";
            string fechaInicio = pFechaPeriodo.Year+"-"+pFechaPeriodo.Month+"-"+"01";
            string fechafin = pFechaPeriodo.Year + "-"+pFechaPeriodo.Month + "-" + "30";
            comando = "WHERE TB_TRANS.fecha BETWEEN DATEADD(YEAR,-1,CAST('"+fechafin+"' AS date)) AND CAST('"+fechaInicio+"' AS date)";
            if (pAsesorInicial != 0)
            {
                comando = comando + "AND  PARTICIPANTES_OLD.CLAVE IN('" + pAsesorInicial + "')";
            }

            if (pCanalVentas != 0 || pCanalVentas != -1)
            {
                comando = comando + "AND (CANAL_NEW.id =" + pCanalVentas + " OR CANAL_OLD.id =" + pCanalVentas + " )";
            }
            if (pSubCanal != -1)
            {
                comando = comando + "AND (CANALDT_NEW.id =" + pSubCanal + " OR  CANALDT_OLD.id=" + pCanalVentas + ")";
            }
            if (pAsesorFinal != 0)
            {
                comando = comando + "AND PARTICIPANTES_NEW.CLAVE IN('" + pAsesorFinal + "')";
            }

            if (pDirector != 0)
            {
                comando = comando + "AND (DIREC_OLD.ID = " + pDirector + " OR DIREC_NEW.ID = " + pDirector + ")";
            }

            if (pGerOfic != 0)
            {
                comando = comando + "AND (GERENTE_OFC_NEW.ID = " + pGerOfic + " OR GERENTE_OFC_OLD.ID = " + pGerOfic + ")";
            }


            if (pGerReg != 0)
            {
                comando = comando + "AND (GERENTE_REGIONAL_NEW.ID = " + pGerReg + " OR GERENTE_REGIONAL_OLD.ID = " + pGerReg + ")";
            }

            if (pNumeroContrato != null)
            {
                comando = comando + "AND TB_REP.NUMERONEGOCIOPADRE = " + pNumeroContrato + "";
            }

            if (pSubContrato != null)
            {
                comando = comando + "AND TB_REP.NUMERONEGOCIO = " + pSubContrato + "";
            }

            if (pTipoPlan != -1)
            {
                comando = comando + "AND PLAN_DETALLE.ID = " + pTipoPlan + "";
            }

            if (pTipoContrato != -1)
            {
                comando = comando + "AND TIPO_CONTRATO.ID = " + pTipoContrato + "";
            }

            if (sucursal != -1)
            {
                comando = comando + "AND (PARTICIPANTES_OLD.LOCALIDAD_ID = " + sucursal + " OR PARTICIPANTES_NEW.LOCALIDAD_ID= " +sucursal + ")";
            }
            DataSet DatasetR = new DataSet();


            try
            {
                EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                SqlCommand cmd = new SqlCommand("SAI_Reporte_Transferencias", sqlConn);
                cmd.CommandTimeout = 6000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FILTRO", Convert.ToString(comando));
                cmd.Parameters.Add("@MODELO", Convert.ToInt32(modelo));
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

                        lResult.Add(new ReporteTrasnferenciasClass()
                        {
                            Fecha_Periodo = (pFechaPeriodo.Year - 1) + " - " + (pFechaPeriodo.Month + 1) + "hasta" + pFechaPeriodo.Year + " - " + pFechaPeriodo.Month,
                            Clave_Inicial = (lRow["CLAVE_ASESOR_OLD"] == DBNull.Value ? string.Empty : lRow["CLAVE_ASESOR_OLD"].ToString()),
                            Nombre_Asesor_Inicial = (lRow["ASESOR_OLD_NOMBRE"] == DBNull.Value ? string.Empty : lRow["ASESOR_OLD_NOMBRE"].ToString()),
                            Clave_Final = (lRow["CLAVE_ASESOR_NEW"] == DBNull.Value ? string.Empty : lRow["CLAVE_ASESOR_NEW"].ToString()),
                            Nombre_Asesor_Final = (lRow["ASESOR_NEW_NOMBRE"] == DBNull.Value ? string.Empty : lRow["ASESOR_NEW_NOMBRE"].ToString()),
                            Fecha_Transferencia = (lRow["FECHA_TRANSFERENCIA"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(lRow["FECHA_TRANSFERENCIA"].ToString())),
                            Canal_Ventas = (lRow["NOMBRE_CANAL_OLD"] == DBNull.Value ? string.Empty : lRow["NOMBRE_CANAL_OLD"].ToString()),
                            SubCanal = (lRow["NOMBRE_SUB_CANAL_OLD"] == DBNull.Value ? string.Empty : lRow["NOMBRE_SUB_CANAL_OLD"].ToString()),
                            sucursal_old = (lRow["SUCURSAL_OLD"] == DBNull.Value ? string.Empty : lRow["SUCURSAL_OLD"].ToString()),
                            Director = (lRow["NOMBRE_DIRECTOR"] == DBNull.Value ? string.Empty : lRow["NOMBRE_DIRECTOR"].ToString()),
                            Gerente_Oficina = (lRow["NOMBRE_GERENTE_OFICINIA"] == DBNull.Value ? string.Empty : lRow["NOMBRE_GERENTE_OFICINIA"].ToString()),
                            Gerente_Regional = (lRow["NOMBRE_GERENTE_REGIONAL"] == DBNull.Value ? string.Empty : lRow["NOMBRE_GERENTE_REGIONAL"].ToString()),

                            SubCanal_new = (lRow["NOMBRE_SUB_CANAL_NEW"] == DBNull.Value ? string.Empty : lRow["NOMBRE_SUB_CANAL_NEW"].ToString()),
                            sucursal_new = (lRow["SUCURSAL_NEW"] == DBNull.Value ? string.Empty : lRow["SUCURSAL_NEW"].ToString()),
                            Canal_Ventas_New = (lRow["NOMBRE_CANAL_NEW"] == DBNull.Value ? string.Empty : lRow["NOMBRE_CANAL_NEW"].ToString()),
                            Director_new = (lRow["NOMBRE_DIRECTOR_NEW"] == DBNull.Value ? string.Empty : lRow["NOMBRE_DIRECTOR_NEW"].ToString()),
                            Gerente_Oficina_new = (lRow["NOMBRE_GERENTE_REGIONAL_NEW"] == DBNull.Value ? string.Empty : lRow["NOMBRE_GERENTE_REGIONAL_NEW"].ToString()),
                            Gerente_Regional_new = (lRow["NOMBRE_GERENTE_REGIONAL"] == DBNull.Value ? string.Empty : lRow["NOMBRE_GERENTE_REGIONAL"].ToString()),
                            Contrato = (lRow["NUMERO_CONTRATO"] == DBNull.Value ? string.Empty : lRow["NUMERO_CONTRATO"].ToString()),
                            SubContrato = (lRow["SUB_CONTRATO"] == DBNull.Value ? string.Empty : lRow["SUB_CONTRATO"].ToString()),
                            Tipo_Plan = (lRow["PLAN_DETALLE"] == DBNull.Value ? string.Empty : lRow["PLAN_DETALLE"].ToString()),
                            Tipo_Contrato = (lRow["CONTRATO"] == DBNull.Value ? string.Empty : lRow["CONTRATO"].ToString()),
                            Tipo_Transferencia = (lRow["TIPO_TRANSFERENCIA"] == DBNull.Value ? string.Empty : lRow["TIPO_TRANSFERENCIA"].ToString()),
                            beneficiarios_trasn = (lRow["BENEFICIARIOS_TRASN"] == DBNull.Value ? 0 : int.Parse(lRow["BENEFICIARIOS_TRASN"].ToString()))

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
