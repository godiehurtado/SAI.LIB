using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using ColpatriaSAI.Negocio.Entidades;
using System.Globalization;
using System.Threading;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class ReporteAsesores
    {

        private SAI_Entities _dbcontext = new SAI_Entities();


        /// <summary>
        /// Metodo para reazlizar la consulta de reporte de asesores
        /// </summary>
        /// <param name="pFechaPeriodo"></param>
        /// <param name="pClavesAsesor"></param>
        /// <param name="pCanalVenta"></param>
        /// <param name="pSubCanal"></param>
        /// <param name="pNombreGerente"></param>
        /// <param name="pNombreDirectores"></param>
        /// <param name="pPorcentajeFija"></param>
        /// <param name="pPorcentajeVariable"></param>
        /// <param name="pPorcentajeTotal"></param>
        /// <param name="pTipoPlan"></param>
        /// <param name="pTipoContrato"></param>
        /// <param name="pEstado"></param>
        /// <param name="pEdadDesde"></param>
        /// <param name="pEdadhasta"></param>
        /// <param name="modelo"></param>
        /// <returns></returns>
        /// 

        public List<ReporteAsesoresClass> GetReporteAsesores(int año = default(int), int mes = default(int), string pClavesAsesor = default(string), int pCanalVenta = default(int),
            int pSubCanal = default(int), int IdGerente = default(int), int IdDirector = default(int), int modelo = default (int), int sucursal = default(int))
        {
            CultureInfo ci = new CultureInfo("ES-co");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            List<ReporteAsesoresClass> lResult = new List<ReporteAsesoresClass>();
            
            DataSet DatasetR = new DataSet();

            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmd = new SqlCommand("MAC_ReporteAsesores", sqlConn);
            cmd.CommandTimeout = 6000;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@año", Convert.ToInt32(año));
            cmd.Parameters.Add("@mes", Convert.ToInt32(mes));
            cmd.Parameters.Add("@clave", Convert.ToString(pClavesAsesor));
            cmd.Parameters.Add("@modelo_id", Convert.ToInt32(modelo));
            cmd.Parameters.Add("@sucursal_id", Convert.ToInt32(sucursal));
            cmd.Parameters.Add("@lidercomercial_id", Convert.ToInt32(IdDirector));
            cmd.Parameters.Add("@lideroficina_id", Convert.ToInt32(IdGerente));
            cmd.Parameters.Add("@canal_id", Convert.ToInt32(pCanalVenta));
            cmd.Parameters.Add("@canaldetalle_id", Convert.ToInt32(pSubCanal));

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
                    lResult.Add(new ReporteAsesoresClass()
                    {
                        CLAVE = (lRow["CLAVE"] == DBNull.Value ? string.Empty : lRow["CLAVE"].ToString()),
                        INTERMEDIARIO = (lRow["INTERMEDIARIO"] == DBNull.Value ? string.Empty : lRow["INTERMEDIARIO"].ToString()),
                        SUCURSAL = (lRow["SUCURSAL"] == DBNull.Value ? string.Empty : lRow["SUCURSAL"].ToString()),
                        TOTAL_RECAUDO = (lRow["TOTAL_RECAUDO"] == DBNull.Value ? string.Empty : lRow["TOTAL_RECAUDO"].ToString()),
                        PORCENTAJE_PROMEDIO_COMISION_FIJA = (lRow["PROMEDIO_COMISION_FIJA"] == DBNull.Value ? string.Empty : lRow["PROMEDIO_COMISION_FIJA"].ToString()),
                        TOTAL_VALOR_COMISION_FIJA = (lRow["TOTAL_VALOR_COMISION_FIJA"] == DBNull.Value ? string.Empty : lRow["TOTAL_VALOR_COMISION_FIJA"].ToString()),
                        TOTAL_USUARIO_NUEVOS = (lRow["TOTAL_USUARIO_NUEVOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_USUARIO_NUEVOS"].ToString()),
                        TOTAL_EQUIVALENCIAS_X_USUARIO_NUEVOS = (lRow["TOTAL_EQUIVALENCIAS_X_USUARIO_NUEVOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_EQUIVALENCIAS_X_USUARIO_NUEVOS"].ToString()),
                        CANTIDAD_USUARIOS_FINAL = (lRow["CANTIDAD_USUARIOS_FINAL"] == DBNull.Value ? string.Empty : lRow["CANTIDAD_USUARIOS_FINAL"].ToString()),
                        CANTIDAD_USUARIOS_INICIAL = (lRow["CANTIDAD_USUARIOS_INICIAL"] == DBNull.Value ? string.Empty : lRow["CANTIDAD_USUARIOS_INICIAL"].ToString()),
                        TOTAL_USUARIOS_NETOS = (lRow["TOTAL_USUARIOS_NETOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_USUARIOS_NETOS"].ToString()),
                        TOTAL_EQUIVALENCIAS_X_NETOS = (lRow["TOTAL_EQUIVALENCIAS_X_NETOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_EQUIVALENCIAS_X_NETOS"].ToString()),
                        PORCENTAJE_COMISION_VARIABLE_DEL_MES = (lRow["SOBRECOMISION_DEL_MES"] == DBNull.Value ? string.Empty : lRow["SOBRECOMISION_DEL_MES"].ToString()),
                        PORCENTAJE_PROMEDIO_COMISION_VARIABLE_PAGADA = (lRow["PROMEDIO_SOBRECOMISION_PAGADA"] == DBNull.Value ? string.Empty : Math.Round(Double.Parse(lRow["PROMEDIO_SOBRECOMISION_PAGADA"].ToString())).ToString()),
                        TOTAL_VALOR_COMISION_VARIABLE_PAGADA = (lRow["TOTAL_VALOR_SOBRECOMISION_PAGADA"] == DBNull.Value ? string.Empty : lRow["TOTAL_VALOR_SOBRECOMISION_PAGADA"].ToString()),
                        PORCENTAJE_PROMEDIO_COMISION_TOTAL = (lRow["PROMEDIO_COMISION_TOTAL"] == DBNull.Value ? string.Empty : lRow["PROMEDIO_COMISION_TOTAL"].ToString()),
                        VALOR_COMISION_TOTAL = (lRow["VALOR_COMISION_TOTAL"] == DBNull.Value ? string.Empty : lRow["VALOR_COMISION_TOTAL"].ToString()),
                    });
                }
                return lResult;
            }

            return lResult;
        }


        public List<ReporteAsesoresClass> GetReporteLiderComercial(int año = default(int), int mes = default(int), string pClavesAsesor = default(string), int pCanalVenta = default(int),
            int pSubCanal = default(int), int IdGerente = default(int), int IdDirector = default(int), int modelo = default (int), int sucursal = default(int))
        {

            List<ReporteAsesoresClass> lResult = new List<ReporteAsesoresClass>();

            DataSet DatasetR = new DataSet();

            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmd = new SqlCommand("MAC_ReporteAsesores", sqlConn);
            cmd.CommandTimeout = 6000;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@año", Convert.ToInt32(año));
            cmd.Parameters.Add("@mes", Convert.ToInt32(mes));
            cmd.Parameters.Add("@clave", Convert.ToString(pClavesAsesor));
            cmd.Parameters.Add("@modelo_id", Convert.ToInt32(modelo));
            cmd.Parameters.Add("@sucursal_id", Convert.ToInt32(sucursal));
            cmd.Parameters.Add("@lidercomercial_id", Convert.ToInt32(IdDirector));
            cmd.Parameters.Add("@lideroficina_id", Convert.ToInt32(IdGerente));
            cmd.Parameters.Add("@canal_id", Convert.ToInt32(pCanalVenta));
            cmd.Parameters.Add("@canaldetalle_id", Convert.ToInt32(pSubCanal));

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
                    lResult.Add(new ReporteAsesoresClass()
                    {
                        CLAVE = (lRow["CLAVE"] == DBNull.Value ? string.Empty : lRow["CLAVE"].ToString()),
                        INTERMEDIARIO = (lRow["INTERMEDIARIO"] == DBNull.Value ? string.Empty : lRow["INTERMEDIARIO"].ToString()),
                        SUCURSAL = (lRow["SUCURSAL"] == DBNull.Value ? string.Empty : lRow["SUCURSAL"].ToString()),
                        TOTAL_RECAUDO = (lRow["TOTAL_RECAUDO"] == DBNull.Value ? string.Empty : lRow["TOTAL_RECAUDO"].ToString()),
                        PORCENTAJE_PROMEDIO_COMISION_FIJA = (lRow["PROMEDIO_COMISION_FIJA"] == DBNull.Value ? string.Empty : lRow["PROMEDIO_COMISION_FIJA"].ToString()),
                        TOTAL_VALOR_COMISION_FIJA = (lRow["TOTAL_VALOR_COMISION_FIJA"] == DBNull.Value ? string.Empty : lRow["TOTAL_VALOR_COMISION_FIJA"].ToString()),
                        TOTAL_USUARIO_NUEVOS = (lRow["TOTAL_USUARIO_NUEVOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_USUARIO_NUEVOS"].ToString()),
                        TOTAL_EQUIVALENCIAS_X_USUARIO_NUEVOS = (lRow["TOTAL_EQUIVALENCIAS_X_USUARIO_NUEVOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_EQUIVALENCIAS_X_USUARIO_NUEVOS"].ToString()),
                        CANTIDAD_USUARIOS_FINAL = (lRow["CANTIDAD_USUARIOS_FINAL"] == DBNull.Value ? string.Empty : lRow["CANTIDAD_USUARIOS_FINAL"].ToString()),
                        CANTIDAD_USUARIOS_INICIAL = (lRow["CANTIDAD_USUARIOS_INICIAL"] == DBNull.Value ? string.Empty : lRow["CANTIDAD_USUARIOS_INICIAL"].ToString()),
                        TOTAL_USUARIOS_NETOS = (lRow["TOTAL_USUARIOS_NETOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_USUARIOS_NETOS"].ToString()),
                        TOTAL_EQUIVALENCIAS_X_NETOS = (lRow["TOTAL_EQUIVALENCIAS_X_NETOS"] == DBNull.Value ? string.Empty : lRow["TOTAL_EQUIVALENCIAS_X_NETOS"].ToString()),
                        PORCENTAJE_COMISION_VARIABLE_DEL_MES = (lRow["SOBRECOMISION_DEL_MES"] == DBNull.Value ? string.Empty : lRow["SOBRECOMISION_DEL_MES"].ToString()),
                        PORCENTAJE_PROMEDIO_COMISION_VARIABLE_PAGADA = (lRow["PROMEDIO_SOBRECOMISION_PAGADA"] == DBNull.Value ? string.Empty : Math.Round(Double.Parse(lRow["PROMEDIO_SOBRECOMISION_PAGADA"].ToString())).ToString()),
                        TOTAL_VALOR_COMISION_VARIABLE_PAGADA = (lRow["TOTAL_VALOR_SOBRECOMISION_PAGADA"] == DBNull.Value ? string.Empty : lRow["TOTAL_VALOR_SOBRECOMISION_PAGADA"].ToString()),
                        PORCENTAJE_PROMEDIO_COMISION_TOTAL = (lRow["PROMEDIO_COMISION_TOTAL"] == DBNull.Value ? string.Empty : lRow["PROMEDIO_COMISION_TOTAL"].ToString()),
                        VALOR_COMISION_TOTAL = (lRow["VALOR_COMISION_TOTAL"] == DBNull.Value ? string.Empty : lRow["VALOR_COMISION_TOTAL"].ToString()),
                    });
                }
                return lResult;
            }

            return lResult;
        }  // fin metodo getasesorescomerciales


        public int ValidaDirector(string IdDocumento)
        {
            // CONSULTO EL ID DEL DIRECTOR 

            string consultaid = "SELECT * FROM PARTICIPANTE WHERE DOCUMENTO='" + IdDocumento + "'";

            EntityConnection entityConn1 = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn1 = (SqlConnection)entityConn1.StoreConnection;
            SqlCommand cmdcons = new SqlCommand(consultaid, sqlConn1);
            cmdcons.CommandType = CommandType.Text;

            SqlDataAdapter dacons = new SqlDataAdapter(cmdcons);

            DataSet dtconsulta = new DataSet();

            dacons.Fill(dtconsulta);
            sqlConn1.Close();
            int idDir;

            if (dtconsulta.Tables[0].Rows.Count > 0)
            {

                DataRow lRowpartcipante = dtconsulta.Tables[0].Rows[0];
                idDir = (lRowpartcipante["ID"] == DBNull.Value ? 0 : Convert.ToInt32(lRowpartcipante["ID"].ToString()));

            }
            else {

                idDir = 0;
            }
                        
            return idDir;
        }

        public Dictionary<string,string> GetInfoPagoAsesorPortal(int idLiquidacion, string clave)
        {
            try{
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;

                using (sqlConn)
                {
                    String cmdtxt = "dbo.MAC_Reporte_Asesor_Portal";
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    sqlConn.Open();
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("liquidacion_id", SqlDbType.Int);
                    cmdReport1.Parameters["liquidacion_id"].Value = idLiquidacion;

                    cmdReport1.Parameters.Add("clave", SqlDbType.NVarChar, 20);
                    cmdReport1.Parameters["clave"].Value = clave;

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    SqlDataReader rta = cmdReport1.ExecuteReader();

                    Dictionary<string, string> intermediario = new Dictionary<string, string>();

                    while (rta.Read())
                    {
                        intermediario.Add("Canal",rta["Canal"].ToString());
                        intermediario.Add("Clave",rta["Clave"].ToString());
                        intermediario.Add("Fecha_Final",DateTime.Parse(rta["Fecha_Final"].ToString()).ToShortDateString());
                        intermediario.Add("Fecha_Inicio", DateTime.Parse(rta["Fecha_Inicio"].ToString()).ToShortDateString());
                        intermediario.Add("Fecha_Reporte",rta["Fecha_Reporte"].ToString());
                        intermediario.Add("Intermediario",rta["Intermediario"].ToString());
                        intermediario.Add("Lider_Comercial",rta["Lider_Comercial"].ToString());
                        intermediario.Add("Localidad",rta["Localidad"].ToString());
                        intermediario.Add("Porcentaje_Comision_Fija","%" + Math.Round(Double.Parse(rta["Porcentaje_Comision_Fija"].ToString()),2).ToString());
                        intermediario.Add("Porcentaje_Comision_Variable","%" + Math.Round(Double.Parse(rta["Porcentaje_Comision_Variable"].ToString()),2).ToString());
                        intermediario.Add("Porcentaje_Total_Comision", "%" + Math.Round(Double.Parse(rta["Porcentaje_Total_Comision"].ToString()), 2).ToString());
                        intermediario.Add("Regional",rta["Regional"].ToString());
                        intermediario.Add("Total_Netos",rta["Total_Netos"].ToString());
                        intermediario.Add("Total_Talentos_Netos",rta["Total_Talentos_Netos"].ToString());
                        intermediario.Add("Total_Talentos_Nuevos",rta["Total_Talentos_Nuevos"].ToString());
                        intermediario.Add("Total_Usuarios_Nuevos",rta["Total_Usuarios_Nuevos"].ToString());
                        intermediario.Add("Total_Usuarios_Vigentes_Actual",rta["Total_Usuarios_Vigentes_Actual"].ToString());
                        intermediario.Add("Total_Usuarios_Vigentes_Anterior",rta["Total_Usuarios_Vigentes_Anterior"].ToString());
                        intermediario.Add("Total_Valor_Comision_Fija",Double.Parse(rta["Total_Valor_Comision_Fija"].ToString()).ToString("C2"));
                        intermediario.Add("Total_Valor_Comision_Variable", Double.Parse(rta["Total_Valor_Comision_Variable"].ToString()).ToString("C2"));
                        intermediario.Add("Valor_Total_Comision",Double.Parse(rta["Valor_Total_Comision"].ToString()).ToString("C2"));
                        intermediario.Add("Valor_Total_Recaudo", Double.Parse(rta["Valor_Total_Recaudo"].ToString()).ToString("C2"));
                        break;
                    }
                    rta.Close();
                    cmdReport1.Connection.Close();
                    return intermediario;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public Dictionary<int, string> GetLiquidacionesPortalAsesor()
        {
            Dictionary<int, string> dirliquidaciones = new Dictionary<int, string>();
            List<LiquidacionComision> liquidaciones = (from li in _dbcontext.LiquidacionComisions
                                                     where li.estadoliquidacion_id == 2
                                                     select li).ToList();
            foreach (LiquidacionComision liqui in liquidaciones)
            {
                dirliquidaciones.Add(liqui.id, "Liquidación " + liqui.id + " - " + liqui.fecha.ToShortDateString());
            }
            return dirliquidaciones;
        }
    }
}
