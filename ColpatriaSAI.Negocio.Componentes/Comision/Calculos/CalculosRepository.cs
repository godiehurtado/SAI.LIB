using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using System.Threading;
using System.Data;
using Entidades = ColpatriaSAI.Negocio.Entidades;
using Componentes = ColpatriaSAI.Negocio.Componentes;
using System.Data.EntityClient;
using System.Data.SqlClient;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Entidades.CustomEntities;
using ColpatriaSAI.Negocio.Componentes.PackagesExecutionService;
using ColpatriaSAI.Negocio.Componentes.ETLs;
using ColpatriaSAI.Negocio.Componentes.Utilidades;
using System.Diagnostics;
using ColpatriaSAI.Negocio.Entidades.Informacion;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Calculos
{
    public class CalculosRepository
    {

        private SAI_Entities _dbcontext = new SAI_Entities();
        private InfoAplicacion _info;
        private int idProcesoCF, idProcesoCV;

        #region Calculos e Insercion Talentos
        public List<ResultadosCalculos> CalculoTalentos(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int), string Username = default(string))
        {
            return new CalculosTalentos().CalculoTalentos(pAnio, pMes, pIdModelo, pAsesor, Username);
        }

        #endregion

        #region Calculos e Insercion Netos

        public List<ResultadosCalculos> CalculosNetos(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int), string Username = default(string))
        {
            return new CalculosNetos().CalculoNetos(pAnio, pMes, pIdModelo, pAsesor, Username);
        }

        #endregion

        #region Calculos de Comision

        #region Comision Variable
        internal List<Entidades.ComisionVariableAsesor> ObtenerComisionVariablePorLiquidacion(int liquidacionComisionId)
        {
            return _dbcontext.ComisionVariableAsesors
                .Include("Participante")
                .Where(cva => cva.liquidacioncomision_id == liquidacionComisionId).ToList();
            //.Join(_dbcontext.Participantes,
            //        x => x.participante_id,
            //        y => y.id,
            //        (x, y) => new Entidades.ComisionVariableAsesor()
            //        {
            //            id = x.id,
            //            liquidacioncomision_id = x.liquidacioncomision_id,
            //            Participante = y,
            //            rangoXInferior = x.rangoXInferior,
            //            rangoXSuperior = x.rangoXSuperior,
            //            rangoYInferior = x.rangoYInferior,
            //            rangoYsuperior = x.rangoYsuperior,
            //            talentosnetos = x.talentosnetos,
            //            talentosnuevos = x.talentosnuevos,
            //            participante_id = x.participante_id,
            //            comisionvariable = x.comisionvariable
            //        }).ToList();
        }
        #endregion

        #region Comision Fija
        internal List<Entidades.ResultadoProcedimientos.ComisionFijaFacturacion> ObtenerComisionFijaFacturacionPorLiquidacion(int liquidacionComisionId)
        {
            List<Entidades.ResultadoProcedimientos.ComisionFijaFacturacion> resultados = new List<Entidades.ResultadoProcedimientos.ComisionFijaFacturacion>();
            String cmdtxt = "dbo.ObtenerComisionFijaFacturacionPorLiquidacion";
            DataSet retVal = new DataSet();
            SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
            SqlCommand cmdReport = new SqlCommand(cmdtxt, sqlConn);
            cmdReport.CommandType = CommandType.StoredProcedure;
            cmdReport.Parameters.Add("liquidacioncomision_id", SqlDbType.Int);
            cmdReport.Parameters["liquidacioncomision_id"].Value = liquidacionComisionId;
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            DataTable lResults = retVal.Tables[0];

            foreach (DataRow lrow in lResults.Rows)
            {
                Entidades.ResultadoProcedimientos.ComisionFijaFacturacion tmp = new Entidades.ResultadoProcedimientos.ComisionFijaFacturacion();
                tmp.Compania = lrow["Compania"].ToString();
                tmp.Contrato = lrow["numeronegociopadre"].ToString();
                tmp.SubContrato = lrow["numeronegocio"].ToString();
                tmp.Documento = lrow["documento"].ToString();
                tmp.edad = Convert.ToInt32(lrow["edad"]);
                tmp.EstadoBeneficiario = lrow["EstadoBeneficiario"].ToString();
                tmp.Factura = lrow["numerofactura"].ToString();
                tmp.PlanDetalle = lrow["PlanDetalle"].ToString();
                tmp.ProductoDetalle = lrow["ProductoDetalle"].ToString();
                tmp.Clave = Convert.ToInt32(lrow["clave"]);
                tmp.TipoContrato = lrow["TipoContrato"].ToString();
                tmp.FechaFactura = Convert.ToDateTime(lrow["fechaexpedicionfactura"]);
                #region C44102 EHBV SE INCLUYE EL CAMPO PERIODO
                /// <summary>
                /// Determina el periodo de facturacion
                /// </summary>
                tmp.Periodo = Convert.ToDateTime(lrow["Periodo"]);
                #endregion C44102 EHBV SE INCLUYE EL CAMPO PERIODO
                tmp.Canal = lrow["Canal"].ToString();
                tmp.SubCanal = lrow["SubCanal"].ToString();
                tmp.TalentosNetos = Convert.ToInt32(lrow["talentosnetos"]);
                tmp.TalentosNuevos = Convert.ToInt32(lrow["talentosnuevos"]);
                tmp.IdNovedad = lrow["id_Novedad_BH"].ToString();
                tmp.Novedad = lrow["descripcionNovedad"].ToString();
                tmp.PorcentajeComisionFijaSinExcepcion = !DBNull.Value.Equals(lrow["porcentajeComisionFijaSinExcepcion"]) ? Convert.ToDecimal(lrow["porcentajeComisionFijaSinExcepcion"]) : 0;
                tmp.PorcentajeComisionFijaConExcepcion = !DBNull.Value.Equals(lrow["factor"]) ? Convert.ToDecimal(lrow["factor"]) : 0;
                tmp.ValorComisionExcepcion = !DBNull.Value.Equals(lrow["ValorComisionFijaConExcepcion"]) ? Convert.ToDecimal(lrow["ValorComisionFijaConExcepcion"]) : 0;
                tmp.PorcentajeComisionVariable = !DBNull.Value.Equals(lrow["comisionvariable"]) ? Convert.ToDecimal(lrow["comisionvariable"]) : 0;
                tmp.PorcentajeComisionTotal = Convert.ToDecimal(lrow["PorcentajeComisionTotal"]);
                tmp.PorcentajeParticipacion = !DBNull.Value.Equals(lrow["porcentajeparticipacion"]) ? Convert.ToDecimal(lrow["porcentajeparticipacion"]) : 0;tmp.ValorComisionFija = Convert.ToDecimal(lrow["ValorComisionFijaSinExcepcion"]);
                tmp.ValorComisionVariable = Convert.ToDecimal(lrow["valorcomisionvariable"]);
                tmp.TipoDocumento = lrow["tipoDocumento"].ToString().Trim();
                tmp.ValorFactura = Convert.ToDecimal(lrow["ValorFactura"]);
                tmp.ValorComisionTotal = Convert.ToDecimal(lrow["ValorComisionTotal"]);
                tmp.TipoExcepcion = lrow["TipoExcepcion"].ToString();
                
                resultados.Add(tmp);
            }
            return resultados;
        }

        internal List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> ObtenerComisionFijaRecaudosPorLiquidacion(int liquidacionComisionId)
        {
            List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> resultados = new List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos>();
            String cmdtxt = "dbo.ObtenerComisionFijaRecaudosPorLiquidacion";
            DataSet retVal = new DataSet();
            SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
            SqlCommand cmdReport = new SqlCommand(cmdtxt, sqlConn);
            cmdReport.CommandType = CommandType.StoredProcedure;
            cmdReport.CommandTimeout = 0;
            cmdReport.Parameters.Add("liquidacioncomision_id", SqlDbType.Int);
            cmdReport.Parameters["liquidacioncomision_id"].Value = liquidacionComisionId;
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            DataTable lResults = retVal.Tables[0];

            foreach (DataRow lrow in lResults.Rows)
            {
                Entidades.ResultadoProcedimientos.ComisionFijaRecaudos tmp = new Entidades.ResultadoProcedimientos.ComisionFijaRecaudos();

                tmp.Compania = lrow["Compania"].ToString();
                tmp.Contrato = lrow["numeronegociopadre"].ToString();
                tmp.SubContrato = lrow["numeronegocio"].ToString();
                tmp.Documento = lrow["documento"].ToString();
                tmp.edad = Convert.ToInt32(lrow["edad"]);
                tmp.EstadoBeneficiario = lrow["EstadoBeneficiario"].ToString();
                tmp.Factura = lrow["numerofactura"].ToString();
                tmp.PlanDetalle = lrow["PlanDetalle"].ToString();
                tmp.ProductoDetalle = lrow["ProductoDetalle"].ToString();
                tmp.Clave = Convert.ToInt32(lrow["clave"]);
                tmp.TipoContrato = lrow["TipoContrato"].ToString();
                tmp.FechaRecaudo = Convert.ToDateTime(lrow["fecharecaudo"]);
                tmp.Canal = lrow["Canal"].ToString();
                tmp.SubCanal = lrow["SubCanal"].ToString();
                tmp.TalentosNetos = Convert.ToInt32(lrow["talentosnetos"]);
                tmp.TalentosNuevos = Convert.ToInt32(lrow["talentosnuevos"]);
                tmp.PorcentajeComisionFijaSinExcepcion = !DBNull.Value.Equals(lrow["porcentajeComisionFijaSinExcepcion"]) ? Convert.ToDecimal(lrow["porcentajeComisionFijaSinExcepcion"]) : 0;
                tmp.PorcentajeComisionFijaConExcepcion = !DBNull.Value.Equals(lrow["porcentajeComisionFijaConExcepcion"]) ? Convert.ToDecimal(lrow["porcentajeComisionFijaConExcepcion"]) : 0;
                tmp.ValorComisionVariable = Convert.ToDecimal(lrow["valorcomisionvariable"]);
                tmp.ValorComisionExcepcion = !DBNull.Value.Equals(lrow["ValorComisionFijaConExcepcion"]) ? Convert.ToDecimal(lrow["ValorComisionFijaConExcepcion"]) : 0;
                tmp.ValorComisionFija = !DBNull.Value.Equals(lrow["ValorComisionFijaSinExcepcion"]) ? Convert.ToDecimal(lrow["ValorComisionFijaSinExcepcion"]) : 0;
                tmp.PorcentajeComisionVariable = !DBNull.Value.Equals(lrow["comisionvariable"]) ? Convert.ToDecimal(lrow["comisionvariable"]) : 0;
                tmp.PorcentajeComisionTotal = Convert.ToDecimal(lrow["PorcentajeComisionTotal"]);
                tmp.PorcentajeParticipacion = !DBNull.Value.Equals(lrow["porcentajeparticipacion"]) ? Convert.ToDecimal(lrow["porcentajeparticipacion"]) : 0;
                tmp.TipoExcepcion = lrow["TipoExcepcion"].ToString().Trim();
                tmp.TipoDocumento = lrow["tipoDocumento"].ToString().Trim();
                tmp.ValorFactura = Convert.ToDecimal(lrow["distribucionrecaudoasesor"]);
                tmp.ValorComisionTotal = Convert.ToDecimal(lrow["ValorComisionTotal"]);
                tmp.FechaFactura = lrow["FechaFactura"].ToString().Trim();//C44102 EHBV 
                tmp.Periodo = lrow["Periodo"] != null ? lrow["Periodo"].ToString().Trim() : string.Empty;//C44102 EHBV 
                tmp.Concepto = lrow["TIPO_COMISION"].ToString().Trim();
                tmp.Recibo = lrow["Recibo"].ToString().Trim();
                tmp.ParticipacionUsurio = !DBNull.Value.Equals(lrow["participacionusuario"]) ? Convert.ToDecimal(lrow["participacionusuario"]) : 0;
                resultados.Add(tmp);
            }
            return resultados;
        }

        internal List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> ObtenerComisionFijaRecaudosPorLiquidacion604(int liquidacionComisionId)
        {
            List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> resultados = new List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos>();
            String cmdtxt = "dbo.ObtenerComisionFijaRecaudosPorLiquidacionSinMarcaDePago";
            DataSet retVal = new DataSet();
            SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
            SqlCommand cmdReport = new SqlCommand(cmdtxt, sqlConn);
            cmdReport.CommandType = CommandType.StoredProcedure;
            cmdReport.CommandTimeout = 0;
            cmdReport.Parameters.Add("liquidacioncomision_id", SqlDbType.Int);
            cmdReport.Parameters["liquidacioncomision_id"].Value = liquidacionComisionId;
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            DataTable lResults = retVal.Tables[0];

            foreach (DataRow lrow in lResults.Rows)
            {
                Entidades.ResultadoProcedimientos.ComisionFijaRecaudos tmp = new Entidades.ResultadoProcedimientos.ComisionFijaRecaudos();

                tmp.Compania = lrow["Compania"].ToString();
                tmp.Contrato = lrow["numeronegociopadre"].ToString();
                tmp.SubContrato = lrow["numeronegocio"].ToString();
                tmp.Documento = lrow["documento"].ToString();
                tmp.edad = Convert.ToInt32(lrow["edad"]);
                tmp.EstadoBeneficiario = lrow["EstadoBeneficiario"].ToString();
                tmp.Factura = lrow["numerofactura"].ToString();
                tmp.PlanDetalle = lrow["PlanDetalle"].ToString();
                tmp.ProductoDetalle = lrow["ProductoDetalle"].ToString();
                tmp.Clave = Convert.ToInt32(lrow["clave"]);
                tmp.TipoContrato = lrow["TipoContrato"].ToString();
                tmp.FechaRecaudo = Convert.ToDateTime(lrow["fecharecaudo"]);
                tmp.Canal = lrow["Canal"].ToString();
                tmp.SubCanal = lrow["SubCanal"].ToString();
                tmp.TalentosNetos = Convert.ToInt32(lrow["talentosnetos"]);
                tmp.TalentosNuevos = Convert.ToInt32(lrow["talentosnuevos"]);
                tmp.PorcentajeComisionFijaSinExcepcion = !DBNull.Value.Equals(lrow["porcentajeComisionFijaSinExcepcion"]) ? Convert.ToDecimal(lrow["porcentajeComisionFijaSinExcepcion"]) : 0;
                tmp.PorcentajeComisionFijaConExcepcion = !DBNull.Value.Equals(lrow["porcentajeComisionFijaConExcepcion"]) ? Convert.ToDecimal(lrow["porcentajeComisionFijaConExcepcion"]) : 0;
                tmp.ValorComisionVariable = Convert.ToDecimal(lrow["valorcomisionvariable"]);
                tmp.ValorComisionExcepcion = !DBNull.Value.Equals(lrow["ValorComisionFijaConExcepcion"]) ? Convert.ToDecimal(lrow["ValorComisionFijaConExcepcion"]) : 0;
                tmp.ValorComisionFija = !DBNull.Value.Equals(lrow["ValorComisionFijaSinExcepcion"]) ? Convert.ToDecimal(lrow["ValorComisionFijaSinExcepcion"]) : 0;
                tmp.PorcentajeComisionVariable = !DBNull.Value.Equals(lrow["comisionvariable"]) ? Convert.ToDecimal(lrow["comisionvariable"]) : 0;
                tmp.PorcentajeComisionTotal = Convert.ToDecimal(lrow["PorcentajeComisionTotal"]);
                tmp.PorcentajeParticipacion = !DBNull.Value.Equals(lrow["porcentajeparticipacion"]) ? Convert.ToDecimal(lrow["porcentajeparticipacion"]) : 0;
                tmp.TipoExcepcion = lrow["TipoExcepcion"].ToString().Trim();
                tmp.TipoDocumento = lrow["tipoDocumento"].ToString().Trim();
                tmp.ValorFactura = Convert.ToDecimal(lrow["distribucionrecaudoasesor"]);
                tmp.ValorComisionTotal = Convert.ToDecimal(lrow["ValorComisionTotal"]);
                tmp.FechaFactura = lrow["FechaFactura"].ToString().Trim();//C44102 EHBV 
                tmp.Periodo = lrow["Periodo"] != null ? lrow["Periodo"].ToString().Trim() : string.Empty;//C44102 EHBV 
                tmp.Concepto = lrow["TIPO_COMISION"].ToString().Trim();
                tmp.Recibo = lrow["Recibo"].ToString().Trim();
                tmp.ParticipacionUsurio = !DBNull.Value.Equals(lrow["participacionusuario"]) ? Convert.ToDecimal(lrow["participacionusuario"]) : 0;
                resultados.Add(tmp);
            }
            return resultados;
        }

        internal ResultadoOperacionBD PreCalcularComisionSegunModelo(int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();
            try
            {
                String cmdtxt = "dbo.PreCalcularComisionSegunModelo";
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
                SqlCommand cmdReport = new SqlCommand(cmdtxt, sqlConn);
                cmdReport.CommandType = CommandType.StoredProcedure;
                cmdReport.Parameters.Add("modelo_id", SqlDbType.Int);
                cmdReport.Parameters["modelo_id"].Value = modeloId;
                cmdReport.Parameters.Add("anio", SqlDbType.SmallInt);
                cmdReport.Parameters["anio"].Value = anio;
                cmdReport.Parameters.Add("mes", SqlDbType.TinyInt);
                cmdReport.Parameters["mes"].Value = mes;
                cmdReport.Parameters.Add("liquidacioncomision_id", SqlDbType.Int);
                cmdReport.Parameters["liquidacioncomision_id"].Value = liquidacionComisionId;
                cmdReport.Parameters.Add("tipoliquidacion_id", SqlDbType.TinyInt);
                cmdReport.Parameters["tipoliquidacion_id"].Value = tipoLiquidacionId;
                using (sqlConn)
                {
                    cmdReport.Connection.Open();
                    res.RegistrosAfectados = cmdReport.ExecuteNonQuery();
                    cmdReport.Connection.Close();
                }
                res.Resultado = ResultadoOperacion.Exito;
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
            }
            return res;
        }
        #endregion

        #region Liquidacion Comision
        internal ResultadoOperacionBD LiquidarComisionSegunModelo(int modeloId, short anio, byte mes, int liquidacionComisionId)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();
            try
            {
                String cmdtxt = "dbo.LiquidarComision";
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
                SqlCommand cmdReport = new SqlCommand(cmdtxt, sqlConn);
                cmdReport.CommandType = CommandType.StoredProcedure;
                cmdReport.Parameters.Add("modelo_id", SqlDbType.Int);
                cmdReport.Parameters["modelo_id"].Value = modeloId;
                cmdReport.Parameters.Add("anio", SqlDbType.SmallInt);
                cmdReport.Parameters["anio"].Value = anio;
                cmdReport.Parameters.Add("mes", SqlDbType.TinyInt);
                cmdReport.Parameters["mes"].Value = mes;
                cmdReport.Parameters.Add("liquidacioncomision_id", SqlDbType.Int);
                cmdReport.Parameters["liquidacioncomision_id"].Value = liquidacionComisionId;
                using (sqlConn)
                {
                    cmdReport.Connection.Open();
                    res.RegistrosAfectados = cmdReport.ExecuteNonQuery();
                    cmdReport.Connection.Close();
                }
                res.Resultado = ResultadoOperacion.Exito;
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
            }
            return res;
        }

        internal List<LiquidacionComision> ObtenerHistoricoLiquidaciones()
        {
            return _dbcontext.LiquidacionComisions.Include("ModeloComision")
                .Include("EstadoLiquidacion")
                .Where(x=>x.estadoliquidacion_id != 3)
                //.OrderByDescending(x => x.fecha)
                .ToList();
        }
        #endregion

        #endregion

        #region levantamiento ETL CF y CV
        /// <summary>
        /// 2023-11-26 DAHG: Se comentarea método para conservar BK de versió antigua
        /// Este metodo ejecuta la ETL de Extraccion CF para tipo ejec 1 y si el tipo de ejec es 2
        /// ejecuta etl de extraccion CV y despeus de levantar las ETL ejecuta SP PreCalcularComisionSegunModelo
        /// </summary>
        /// <param name="parmEtl">Parametros para levantar la ETL</param>
        /// <param name="modeloId"></param>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <param name="liquidacionComisionId"></param>
        /// <param name="tipoLiquidacionId"></param>
        /// <param name="usuario"></param>
        /// <param name="tipoEjec">1- Ejecuta ETL CF. 2- Ejecuta ETL CF y ETL CV </param>
        /// <returns></returns>
        //internal ResultadoOperacionBD ExtractCf_CV(string idApp,Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec)
        //{
        //    ResultadoOperacionBD res = new ResultadoOperacionBD();
        //    string rta01 = String.Empty;
        //    int idliquidacion = 0;
        //    int rta02 = 0;
        //    string rta03 = String.Empty;
        //    int rta04 = 0;    

        //    #region PASO 01 - Inicio Liquidación
        //    try
        //    {                
        //        SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
        //        SqlConnection sqlConn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["IntegrationServer"].ConnectionString);

        //        using (sqlConn)
        //        using (sqlConn2)
        //        {
        //            string cmdtxt = "dbo.SAI_ExtraccionCFyCV_Paso01";
        //            SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
        //            cmdReport1.CommandType = CommandType.StoredProcedure;

        //            cmdReport1.Parameters.Add("emodelo_id", SqlDbType.Int);
        //            cmdReport1.Parameters["emodelo_id"].Value = modeloId;

        //            cmdReport1.Parameters.Add("eanio", SqlDbType.SmallInt);
        //            cmdReport1.Parameters["eanio"].Value = anio;

        //            cmdReport1.Parameters.Add("emes", SqlDbType.TinyInt);
        //            cmdReport1.Parameters["emes"].Value = mes;

        //            cmdReport1.Parameters.Add("etipoliquidacion_id", SqlDbType.TinyInt);
        //            cmdReport1.Parameters["etipoliquidacion_id"].Value = tipoLiquidacionId;

        //            cmdReport1.Parameters.Add("eusuario", SqlDbType.VarChar);
        //            cmdReport1.Parameters["eusuario"].Value = usuario;

        //            cmdReport1.Parameters.Add("codExtraccion", SqlDbType.VarChar);
        //            cmdReport1.Parameters["codExtraccion"].Value = parametrosEtlCF["Homologaciones"];

        //            cmdReport1.Parameters.Add("idLiquidacion", SqlDbType.Int);
        //            cmdReport1.Parameters["idLiquidacion"].Direction = ParameterDirection.Output;

        //            string parmEtlCF = "";

        //            List<PackagesExecutionService.Variable> variablesCF = new List<PackagesExecutionService.Variable>();

        //            foreach (KeyValuePair<string, object> pair in parametrosEtlCF)
        //            {
        //                variablesCF.Add(new PackagesExecutionService.Variable() { Key = pair.Key, Value = pair.Value });
        //                parmEtlCF = parmEtlCF + pair.Key + ": " + pair.Value + "; ";
        //            }

        //            cmdReport1.Parameters.Add("parametrosEtlCF", SqlDbType.VarChar);
        //            cmdReport1.Parameters["parametrosEtlCF"].Value = parmEtlCF;


        //            cmdReport1.CommandTimeout = 0;//Timeout infinito
        //            cmdReport1.Connection.Open();
        //            var rta = cmdReport1.ExecuteNonQuery();
        //            idliquidacion = int.Parse(cmdReport1.Parameters["idLiquidacion"].Value.ToString());
        //            res.Resultado = ResultadoOperacion.Exito;

        //            #endregion
        //            #region PASO 02 - Ejecución ETL Comisión Fija

        //            ETLsRemotas etlsRemotasNeg = new ETLsRemotas();
        //            List<ETLRemota> etlsRemotas = etlsRemotasNeg.ListarETLsRemotasporTipo(4);
        //            ETLRemota eTLRemota = etlsRemotas.Where(x => x.nombre == "ExtraccionesCF").FirstOrDefault();

        //            using (PackagesExecutionServiceClient client = new PackagesExecutionServiceClient())
        //            {
        //                DTSResponse dtsResponse = client.ExecuteFromPackageFile(idApp, eTLRemota.packageFileName, eTLRemota.packageConfigFileName, variablesCF.ToArray());
        //                rta02 = (!dtsResponse.Fail ? 0 : 7);
        //            }

        //            #endregion
        //            #region PASO 02 - Respuesta Ejecución ETL Comisión Fija

        //            cmdtxt = "dbo.SAI_ExtraccionCFyCV_Paso02";
        //            cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
        //            cmdReport1.CommandType = CommandType.StoredProcedure;

        //            cmdReport1.Parameters.Add("respuesta", SqlDbType.Int);
        //            cmdReport1.Parameters["respuesta"].Value = rta02;

        //            cmdReport1.Parameters.Add("idLiquidacion", SqlDbType.Int);
        //            cmdReport1.Parameters["idLiquidacion"].Value = idliquidacion;

        //            cmdReport1.Parameters.Add("tipoEjec", SqlDbType.TinyInt);
        //            cmdReport1.Parameters["tipoEjec"].Value = tipoEjec;

        //            string parmEtlCV = "";

        //            List<PackagesExecutionService.Variable> variablesCV = new List<PackagesExecutionService.Variable>();

        //            foreach (KeyValuePair<string, object> pair in parametrosEtlCV)
        //            {
        //                variablesCV.Add(new PackagesExecutionService.Variable() { Key = pair.Key, Value = pair.Value });
        //                parmEtlCV = parmEtlCV + pair.Key + ": " + pair.Value + "; ";
        //            }

        //            cmdReport1.Parameters.Add("parametrosEtlCV", SqlDbType.VarChar);
        //            cmdReport1.Parameters["parametrosEtlCV"].Value = parmEtlCV;

        //            cmdReport1.CommandTimeout = 0;//Timeout infinito
        //            rta = cmdReport1.ExecuteNonQuery();

        //            res.Resultado = ResultadoOperacion.Exito;

        //    #endregion
        //    #region PASO 03 - Ejecución ETL Comisión Variable
        //            if (tipoEjec == 2)
        //            {
        //                ETLRemota eTLRemotaCV = etlsRemotas.Where(x => x.nombre == "ExtraccionesCV").FirstOrDefault();

        //                using (PackagesExecutionServiceClient client = new PackagesExecutionServiceClient())
        //                {
        //                    DTSResponse dtsResponse = client.ExecuteFromPackageFile(idApp, eTLRemotaCV.packageFileName, eTLRemotaCV.packageConfigFileName, variablesCV.ToArray());
        //                    rta02 = (!dtsResponse.Fail ? 0 : 7);
        //                }

        //            }
        //    #endregion
        //    #region PASO 04 - Calculo Comisión

        //            cmdtxt = "dbo.SAI_ExtraccionCFyCV_Paso03";
        //            cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
        //            cmdReport1.CommandType = CommandType.StoredProcedure;

        //            cmdReport1.Parameters.Add("respuesta", SqlDbType.Int);
        //            cmdReport1.Parameters["respuesta"].Value = rta04;

        //            cmdReport1.Parameters.Add("emodelo_id", SqlDbType.Int);
        //            cmdReport1.Parameters["emodelo_id"].Value = modeloId;

        //            cmdReport1.Parameters.Add("eanio", SqlDbType.SmallInt);
        //            cmdReport1.Parameters["eanio"].Value = anio;

        //            cmdReport1.Parameters.Add("emes", SqlDbType.TinyInt);
        //            cmdReport1.Parameters["emes"].Value = mes;

        //            cmdReport1.Parameters.Add("idLiquidacion", SqlDbType.Int);
        //            cmdReport1.Parameters["idLiquidacion"].Value = idliquidacion;

        //            cmdReport1.Parameters.Add("etipoliquidacion_id", SqlDbType.TinyInt);
        //            cmdReport1.Parameters["etipoliquidacion_id"].Value = tipoLiquidacionId;

        //            cmdReport1.Parameters.Add("tipoEjec", SqlDbType.TinyInt);
        //            cmdReport1.Parameters["tipoEjec"].Value = tipoEjec;

        //            cmdReport1.CommandTimeout = 0;//Timeout infinito
        //            rta = cmdReport1.ExecuteNonQuery();
        //            cmdReport1.Connection.Close();
        //        }
        //        res.Resultado = ResultadoOperacion.Exito;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.RegistrosAfectados = 0;
        //        res.MensajeError = "Error: " + ex.Message;
        //        res.Resultado = ResultadoOperacion.Error;
        //    }
        //    #endregion
        //    return res;
        //}

        /// <summary>
        /// 2023-11-26 DAHG: Nueva versión del método para ejecutar la extracción sin modelo, y que esta pueda ser reusada por todos los modelos de liquidación
        /// </summary>
        /// <param name="parmEtl">Parametros para levantar la ETL</param>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <param name="liquidacionComisionId"></param>
        /// <param name="tipoLiquidacionId"></param>
        /// <param name="usuario"></param>
        /// <param name="tipoEjec">1- Ejecuta ETL CF. 2- Ejecuta ETL CF y ETL CV </param>
        /// <returns></returns>
        internal ResultadoOperacionBD ExtractCf_CV(string idApp, Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, short anio, byte mes, byte dia, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec, InfoAplicacion info)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();
            string rta01 = String.Empty;
            int idExtraccion = 0;
            int rta02 = 0;
            string rta03 = String.Empty;
            int rta04 = 0;
            _info = info;

            #region PASO 01 - Inicio Liquidación
            try
            {
                ProcesoLiquidacion procesoCF = new ProcesoLiquidacion()
                {
                    tipo = 7,
                    liquidacion_id = 38,
                    fechaInicio = DateTime.Now,
                    estadoProceso_id = 20
                };
                idProcesoCF = Proceso.registrarProceso(procesoCF);

                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
                SqlConnection sqlConn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["IntegrationServer"].ConnectionString);

                using (sqlConn)
                using (sqlConn2)
                {
                    string cmdtxt = "dbo.SAI_ExtraccionCFyCV_Paso01";
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("eanio", SqlDbType.SmallInt);
                    cmdReport1.Parameters["eanio"].Value = anio;

                    cmdReport1.Parameters.Add("emes", SqlDbType.TinyInt);
                    cmdReport1.Parameters["emes"].Value = mes;

                    cmdReport1.Parameters.Add("edia", SqlDbType.TinyInt);
                    cmdReport1.Parameters["edia"].Value = dia;

                    cmdReport1.Parameters.Add("etipoliquidacion_id", SqlDbType.TinyInt);
                    cmdReport1.Parameters["etipoliquidacion_id"].Value = tipoLiquidacionId;

                    cmdReport1.Parameters.Add("eusuario", SqlDbType.VarChar);
                    cmdReport1.Parameters["eusuario"].Value = usuario;

                    cmdReport1.Parameters.Add("codExtraccion", SqlDbType.VarChar);
                    cmdReport1.Parameters["codExtraccion"].Value = parametrosEtlCF["Homologaciones"];

                    cmdReport1.Parameters.Add("idExtraccion", SqlDbType.Int);
                    cmdReport1.Parameters["idExtraccion"].Direction = ParameterDirection.Output;

                    string parmEtlCF = "";

                    List<PackagesExecutionService.Variable> variablesCF = new List<PackagesExecutionService.Variable>();

                    foreach (KeyValuePair<string, object> pair in parametrosEtlCF)
                    {
                        variablesCF.Add(new PackagesExecutionService.Variable() { Key = pair.Key, Value = pair.Value });
                        parmEtlCF = parmEtlCF + pair.Key + ": " + pair.Value + "; ";
                    }

                    cmdReport1.Parameters.Add("parametrosEtlCF", SqlDbType.VarChar);
                    cmdReport1.Parameters["parametrosEtlCF"].Value = parmEtlCF;


                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    cmdReport1.Connection.Open();
                    var rta = cmdReport1.ExecuteNonQuery();
                    idExtraccion = int.Parse(cmdReport1.Parameters["idExtraccion"].Value.ToString());
                    res.Resultado = ResultadoOperacion.Exito;

                    #endregion
                    #region PASO 02 - Ejecución ETL Comisión Fija

                    ETLsRemotas etlsRemotasNeg = new ETLsRemotas();
                    List<ETLRemota> etlsRemotas = etlsRemotasNeg.ListarETLsRemotasporTipo(4);
                    ETLRemota eTLRemota = etlsRemotas.Where(x => x.nombre == "ExtraccionesCF").FirstOrDefault();

                    using (PackagesExecutionServiceClient client = new PackagesExecutionServiceClient())
                    {
                        client.BeginExecuteFromPackageFile(idApp, eTLRemota.packageFileName, eTLRemota.packageConfigFileName, variablesCF.ToArray(), GetDataCallback_ExtraccionesCF,client);
                    }

                    #endregion
                    #region PASO 02 - Respuesta Ejecución ETL Comisión Fija

                    cmdtxt = "dbo.SAI_ExtraccionCFyCV_Paso02";
                    cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("idExtraccion", SqlDbType.Int);
                    cmdReport1.Parameters["idExtraccion"].Value = idExtraccion;

                    cmdReport1.Parameters.Add("tipoEjec", SqlDbType.TinyInt);
                    cmdReport1.Parameters["tipoEjec"].Value = tipoEjec;

                    string parmEtlCV = "";

                    List<PackagesExecutionService.Variable> variablesCV = new List<PackagesExecutionService.Variable>();

                    foreach (KeyValuePair<string, object> pair in parametrosEtlCV)
                    {
                        variablesCV.Add(new PackagesExecutionService.Variable() { Key = pair.Key, Value = pair.Value });
                        parmEtlCV = parmEtlCV + pair.Key + ": " + pair.Value + "; ";
                    }

                    cmdReport1.Parameters.Add("parametrosEtlCV", SqlDbType.VarChar);
                    cmdReport1.Parameters["parametrosEtlCV"].Value = parmEtlCV;

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    rta = cmdReport1.ExecuteNonQuery();

                    res.Resultado = ResultadoOperacion.Exito;

                    #endregion
                    #region PASO 03 - Ejecución ETL Comisión Variable
                    if (tipoEjec == 2)
                    {
                        ProcesoLiquidacion procesoCV = new ProcesoLiquidacion()
                        {
                            tipo = 7,
                            liquidacion_id = 39,
                            fechaInicio = DateTime.Now,
                            estadoProceso_id = 20
                        };
                        idProcesoCV = Proceso.registrarProceso(procesoCV);

                        ETLRemota eTLRemotaCV = etlsRemotas.Where(x => x.nombre == "ExtraccionesCV").FirstOrDefault();

                        using (PackagesExecutionServiceClient client = new PackagesExecutionServiceClient())
                        {
                            client.BeginExecuteFromPackageFile(idApp, eTLRemotaCV.packageFileName, eTLRemotaCV.packageConfigFileName, variablesCV.ToArray(), GetDataCallback_ExtraccionesCV, client);
                        }

                    }
                    #endregion
                   
                    cmdReport1.Connection.Close();
                }
                res.Resultado = ResultadoOperacion.Exito;
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
                if(idProcesoCF > 0)
                    Proceso.eliminarProceso(idProcesoCF);
                if (idProcesoCV > 0)
                    Proceso.eliminarProceso(idProcesoCV);
            }
            
            return res;
        }

        private void GetDataCallback_ExtraccionesCF(IAsyncResult asyncResult)
        {
            try
            {
                PackagesExecutionServiceClient client = (PackagesExecutionServiceClient)asyncResult.AsyncState;

                DTSResponse result = client.EndExecuteFromPackageFile(asyncResult);

                LoggingUtil logging = new LoggingUtil();

                ETLsRemotas etlsRemotasNeg = new ETLsRemotas();
                List<ETLRemota> etlsRemotas = etlsRemotasNeg.ListarETLsRemotasporTipo(4);
                ETLRemota eTLRemota = etlsRemotas.Where(x => x.nombre == "ExtraccionesCF").FirstOrDefault();

                if (result.Fail)
                    logging.Error(string.Join(", ", result.DtsErrors), LoggingUtil.Prioridad.Alta, "ETLRemotasSAI", TraceEventType.Error, _info);
                else
                    logging.Auditoria(String.Format("La ETL {0}, se ejecutó satisfactoriamente", eTLRemota.packageFileName), LoggingUtil.Prioridad.Baja, "ETLRemotasSAI", _info);
                Proceso.eliminarProceso(idProcesoCF);
            }
            catch (Exception)
            {
                if (idProcesoCF > 0)
                    Proceso.eliminarProceso(idProcesoCF);
            }

        }

        private void GetDataCallback_ExtraccionesCV(IAsyncResult asyncResult)
        {
            try
            {
                PackagesExecutionServiceClient client = (PackagesExecutionServiceClient)asyncResult.AsyncState;

                DTSResponse result = client.EndExecuteFromPackageFile(asyncResult);

                LoggingUtil logging = new LoggingUtil();

                ETLsRemotas etlsRemotasNeg = new ETLsRemotas();
                List<ETLRemota> etlsRemotas = etlsRemotasNeg.ListarETLsRemotasporTipo(4);
                ETLRemota eTLRemota = etlsRemotas.Where(x => x.nombre == "ExtraccionesCV").FirstOrDefault();

                if (result.Fail)
                    logging.Error(string.Join(", ", result.DtsErrors), LoggingUtil.Prioridad.Alta, "ETLRemotasSAI", TraceEventType.Error, _info);
                else
                    logging.Auditoria(String.Format("La ETL {0}, se ejecutó satisfactoriamente", eTLRemota.packageFileName), LoggingUtil.Prioridad.Baja, "ETLRemotasSAI", _info);
                Proceso.eliminarProceso(idProcesoCV);
            }
            catch (Exception)
            {
                if (idProcesoCV > 0)
                    Proceso.eliminarProceso(idProcesoCV);
            }

        }

        /// <summary>
        /// 2023-11-26 DAHG: Método para ejecutar la liquidación de comisión por modelo
        /// </summary>
        /// <param name="extraccionId"></param>
        /// <param name="usuario"></param>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        internal ResultadoOperacionBD LiquidarComision(int extraccionId, string usuario, int modeloId)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();

            try
            {
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
                
                using (sqlConn)
                {
                    string cmdtxt = "PreCalcularComisionSegunModelo";
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("@modelo_id", SqlDbType.Int);
                    cmdReport1.Parameters["@modelo_id"].Value = modeloId;

                    cmdReport1.Parameters.Add("@extraccion_id", SqlDbType.Int);
                    cmdReport1.Parameters["@extraccion_id"].Value = extraccionId;

                    cmdReport1.Parameters.Add("eusuario", SqlDbType.VarChar);
                    cmdReport1.Parameters["eusuario"].Value = usuario;

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    cmdReport1.Connection.Open();
                    var rta = cmdReport1.ExecuteNonQuery();
                    res.Resultado = ResultadoOperacion.Exito;

                    cmdReport1.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
            }

            return res;
        }

        /// <summary>
        /// 2023-11-26 DAHG: Método para cargar la información de una extracción historica ya existente
        /// </summary>
        /// <param name="extraccionId"></param>
        /// <returns></returns>
        internal ResultadoOperacionBD CargarExtraccionHistorico(int extraccionId)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();

            try
            {
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;

                using (sqlConn)
                {
                    string cmdtxt = "MAC_SP_CargarExtraccionHistorico";
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("@extraccion_id", SqlDbType.Int);
                    cmdReport1.Parameters["@extraccion_id"].Value = extraccionId;

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    cmdReport1.Connection.Open();
                    var rta = cmdReport1.ExecuteNonQuery();
                    res.Resultado = ResultadoOperacion.Exito;

                    cmdReport1.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
            }

            return res;
        }

        /// <summary>
        /// 2023-11-27 DAHG: Método para validar proceso de extracción por fecha
        /// </summary>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <param name="dia"></param>
        /// <returns></returns>
        internal ExtraccionComision ValidarExtraccion(int anio, int mes, int dia)
        {
            ExtraccionComision extraccionComision = new ExtraccionComision()
            {
                id = 0,
                usuario = "",
                fecha = DateTime.MinValue,
                estadoExtraccion_id = 0,
                año = 0,
                mes = 0,
                dia = 0,
                tipoLiquidacion = 0,
                CodigoExt = ""
            };
            try
            {
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;

                using (sqlConn)
                {
                    string cmdtxt = "MAC_SP_CONSULTAR_EXTRACCION";
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("@eanio", SqlDbType.SmallInt);
                    cmdReport1.Parameters["@eanio"].Value = anio;

                    cmdReport1.Parameters.Add("@emes", SqlDbType.TinyInt);
                    cmdReport1.Parameters["@emes"].Value = mes;

                    cmdReport1.Parameters.Add("@edia", SqlDbType.TinyInt);
                    cmdReport1.Parameters["@edia"].Value = dia;

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    cmdReport1.Connection.Open();
                    SqlDataReader rta = cmdReport1.ExecuteReader();

                    while (rta.Read())
                    {
                            extraccionComision = new ExtraccionComision()
                            {
                                id = int.Parse(rta["id"].ToString()),
                                usuario = rta["usuario"].ToString(),
                                fecha = DateTime.Parse(rta["fecha"].ToString()),
                                estadoExtraccion_id = int.Parse(rta["estadoExtraccion_id"].ToString()),
                                año = int.Parse(rta["año"].ToString()),
                                mes = int.Parse(rta["mes"].ToString()),
                                dia = int.Parse(rta["dia"].ToString()),
                                tipoLiquidacion = int.Parse(rta["tipoLiquidacion"].ToString()),
                                CodigoExt = rta["CodigoExt"].ToString()
                            };
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return extraccionComision;
        }

        /// <summary>
        /// 2023-11-27 DAHG: Método para validar el último proceso de extracción
        /// </summary>
        /// <returns></returns>
        internal ExtraccionComision ValidarUltimaExtraccion()
        {
            ExtraccionComision extraccionComision = new ExtraccionComision()
            {
                id = 0,
                usuario = "",
                fecha = DateTime.MinValue,
                estadoExtraccion_id = 0,
                año = 0,
                mes = 0,
                dia = 0,
                tipoLiquidacion = 0,
                CodigoExt = ""
            };
            try
            {
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;

                using (sqlConn)
                {
                    string cmdtxt = "MAC_SP_CONSULTARULTIMA_EXTRACCION";
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    cmdReport1.Connection.Open();
                    SqlDataReader rta = cmdReport1.ExecuteReader();

                    while (rta.Read())
                    {
                        extraccionComision = new ExtraccionComision()
                        {
                            id = int.Parse(rta["id"].ToString()),
                            usuario = rta["usuario"].ToString(),
                            fecha = DateTime.Parse(rta["fecha"].ToString()),
                            estadoExtraccion_id = int.Parse(rta["estadoExtraccion_id"].ToString()),
                            año = int.Parse(rta["año"].ToString()),
                            mes = int.Parse(rta["mes"].ToString()),
                            dia = int.Parse(rta["dia"].ToString()),
                            tipoLiquidacion = int.Parse(rta["tipoLiquidacion"].ToString()),
                            CodigoExt = rta["CodigoExt"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return extraccionComision;
        }

        internal List<int> ValLiqPendientes()
        {
            List<int> filtEnProceso = new List<int>();
            try
            {
                filtEnProceso = (from dd in _dbcontext.LiquidacionComisions
                                 where dd.estadoliquidacion_id == 4 //Generando a través de ETL
                                 select dd.id).ToList();
            }
            catch (Exception)
            {
                return null;
            }
            LiquidacionComision abc = new LiquidacionComision();


            return filtEnProceso;
        }

        /// <summary>
        /// Obtiene un periodo  de calculo de comisiòn basado en un año y me epsecifico
        /// </summary>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        internal List<string[]> obtenerPeriodoCalcComision(int anio, int mes)
        {
            List<string[]> filtPeriodos = new List<string[]>();
            try
            {
                StringBuilder lSentencia = new StringBuilder();
                lSentencia.Append(" SELECT TOP 1 [id],[fechainicio],[fechafin]");
                lSentencia.Append(" FROM [SAI].[dbo].[PeriodoCalculoComision]");
                lSentencia.Append(" where fechafin >= CAST('" + anio + "-" + mes + "-01' as datetime) and fechafin< DATEADD(MONTH,1,CAST('" + anio + "-" + mes + "-01' as datetime))");
                
                DataSet retVal = new DataSet();
                EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
                SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
                using (cmdReport)
                {
                    daReport.Fill(retVal);
                }

                filtPeriodos = (from item in retVal.Tables[0].AsEnumerable()
                                    select new string[] { item["id"].ToString(), item["fechainicio"].ToString(), item["fechafin"].ToString() }).ToList();


            }
            catch (Exception)
            {
                return null;
            }



            return filtPeriodos;
        }
        #endregion

        #region Pago - Levantamiento ETL GP y BH
        /// <summary>
        /// Ejecuta el sp LiquidacionComision, levanta la etl de envío a GP y  BH
        /// </summary>
        /// <param name="liquidacionComisionId"></param>
        /// <returns></returns>
        internal ResultadoOperacionBD LiquidarComisiones(string idApp, int liquidacionComisionId)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();
            string rta01 = String.Empty;
            int idliquidacion = liquidacionComisionId;
            int rta02 = 0;
            string rta031 = String.Empty;
            string rta032 = String.Empty;
            string rta033 = String.Empty;
            int rta04 = 0;

            try
            {
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
                SqlConnection sqlConn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["IntegrationServer"].ConnectionString);

                using (sqlConn)
                using (sqlConn2)
                {
                    #region PASO 01 - Cálculo Liquidación y Parámetros ETL GP
                    String cmdtxt = "dbo.SAI_LevantarEtlGpyBH_Paso001";
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("@idLiquidacion", SqlDbType.Int);
                    cmdReport1.Parameters["@idLiquidacion"].Value = liquidacionComisionId;

                    cmdReport1.Parameters.Add("@codExtraccion", SqlDbType.NVarChar, 32);
                    cmdReport1.Parameters["@codExtraccion"].Direction = ParameterDirection.Output;
                    
                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    cmdReport1.Connection.Open();
                    var rta = cmdReport1.ExecuteNonQuery();
                    rta01 = cmdReport1.Parameters["@codExtraccion"].Value.ToString();
                    res.Resultado = ResultadoOperacion.Exito;
                    #endregion

                    #region PASO 02 - Ejecución ETL GP

                    List<PackagesExecutionService.Variable> variablesGp = new List<PackagesExecutionService.Variable>();
                    variablesGp.Add(new PackagesExecutionService.Variable() { Key = "CODIGOEXTRACCION", Value = rta01 });

                    
                    ETLsRemotas etlsRemotasNeg = new ETLsRemotas();
                    List<ETLRemota> etlsRemotas = etlsRemotasNeg.ListarETLsRemotasporTipo(4);
                    ETLRemota eTLRemota = etlsRemotas.Where(x => x.nombre == "EtlGp").FirstOrDefault();

                    using (PackagesExecutionServiceClient client = new PackagesExecutionServiceClient())
                    {
                        DTSResponse dtsResponse = client.ExecuteFromPackageFile(idApp, eTLRemota.packageFileName, eTLRemota.packageConfigFileName, variablesGp.ToArray());
                        rta02 = (!dtsResponse.Fail ? 0 : 7);
                    }

                    res.Resultado = ResultadoOperacion.Exito;
                    #endregion

                    #region PASO 03 - Parámetros ETL BH
                    cmdtxt = "dbo.SAI_LevantarEtlGpyBH_Paso002";
                    cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("idLiquidacion", SqlDbType.Int);
                    cmdReport1.Parameters["idLiquidacion"].Value = liquidacionComisionId;

                    cmdReport1.Parameters.Add("respuesta", SqlDbType.Int);
                    cmdReport1.Parameters["respuesta"].Value = rta02;
                    
                    cmdReport1.Parameters.Add("@panio", SqlDbType.Int);
                    cmdReport1.Parameters["@panio"].Direction = ParameterDirection.Output;

                    cmdReport1.Parameters.Add("@codExtraccion", SqlDbType.VarChar,32);
                    cmdReport1.Parameters["@codExtraccion"].Direction = ParameterDirection.Output;

                    cmdReport1.Parameters.Add("@pmes", SqlDbType.Int);
                    cmdReport1.Parameters["@pmes"].Direction = ParameterDirection.Output;

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    rta = cmdReport1.ExecuteNonQuery();
                    rta031 = cmdReport1.Parameters["@panio"].Value.ToString();
                    rta032 = cmdReport1.Parameters["@codExtraccion"].Value.ToString();
                    rta033 = cmdReport1.Parameters["@pmes"].Value.ToString();

                    res.Resultado = ResultadoOperacion.Exito;
                    #endregion

                    #region PASO 04 - Ejecución ETL BH

                    List<PackagesExecutionService.Variable> variablesBh = new List<PackagesExecutionService.Variable>();
                    variablesGp.Add(new PackagesExecutionService.Variable() { Key = "anio", Value = rta031 });
                    variablesGp.Add(new PackagesExecutionService.Variable() { Key = "CodigoExtraccion", Value = rta032 });
                    variablesGp.Add(new PackagesExecutionService.Variable() { Key = "mes", Value = rta033 });

                    ETLRemota eTLRemotaBh = etlsRemotas.Where(x => x.nombre == "EtlDatosBh").FirstOrDefault();

                    using (PackagesExecutionServiceClient client = new PackagesExecutionServiceClient())
                    {
                        DTSResponse dtsResponse = client.ExecuteFromPackageFile(idApp, eTLRemotaBh.packageFileName, eTLRemota.packageConfigFileName, variablesBh.ToArray());
                        rta04 = (!dtsResponse.Fail ? 0 : 7);
                    }

                    res.Resultado = ResultadoOperacion.Exito;
                    #endregion

                    #region PASO 05 - Finalización del proceso de Pago
                    cmdtxt = "dbo.SAI_LevantarEtlGpyBH_Paso003";
                    cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;

                    cmdReport1.Parameters.Add("idLiquidacion", SqlDbType.Int);
                    cmdReport1.Parameters["idLiquidacion"].Value = liquidacionComisionId;

                    cmdReport1.Parameters.Add("respuesta", SqlDbType.Int);
                    cmdReport1.Parameters["respuesta"].Value = rta04;                    

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    rta = cmdReport1.ExecuteNonQuery();
                    cmdReport1.Connection.Close();
                    res.Resultado = ResultadoOperacion.Exito;
                    #endregion
                }
                res.Resultado = ResultadoOperacion.Exito;                    
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
            }
            return res;
        }

        #endregion


        #region Anular y Reprocesar Liquidacion

        public bool ValidaAnularLiquidacion(int idLiquidacion)
        {

            string sentencia = "select estadoliquidacion_id from LiquidacionComision where id=" + idLiquidacion;

            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(sentencia.ToString(), sqlConn);
            cmdReport.CommandType = CommandType.Text;

            SqlDataAdapter dacons = new SqlDataAdapter(cmdReport);
            DataSet dtconsulta = new DataSet();

            dacons.Fill(dtconsulta);
            sqlConn.Close();
            int estado_liq;
            bool resp = true;

            if (dtconsulta.Tables[0].Rows.Count > 0)
            {

                DataRow lRowLiquidacionComision = dtconsulta.Tables[0].Rows[0];
                estado_liq = Convert.ToInt32(lRowLiquidacionComision["estadoliquidacion_id"].ToString());

                if (estado_liq != 1)
                {
                    resp = false;
                }
                else
                {
                    resp = true;
                }
            }


            return resp;

        }

        internal ResultadoOperacionBD ExtractAnulacion(string idApp, Dictionary<string, object> parametrosETLAnular, int idLiquidacion)
        {
            string rta01 = String.Empty;
            int rta02 = 0;
            ResultadoOperacionBD res = new ResultadoOperacionBD();
          
            try
            {
                #region PASO 01 - Parametros Etl Anular
                String cmdtxt = "dbo.SAI_Anular";
                SqlConnection sqlConn = (SqlConnection)((EntityConnection)_dbcontext.Connection).StoreConnection;
                SqlConnection sqlConn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["IntegrationServer"].ConnectionString);

                using (sqlConn)
                using (sqlConn2)
                {
                    SqlCommand cmdReport1 = new SqlCommand(cmdtxt, sqlConn);
                    cmdReport1.CommandType = CommandType.StoredProcedure;                

                    cmdReport1.Parameters.Add("idLiquidacion", SqlDbType.Int);
                    cmdReport1.Parameters["idLiquidacion"].Value = idLiquidacion;

                    string ParametrosEtl = "";

                    List<PackagesExecutionService.Variable> variablesAnular = new List<PackagesExecutionService.Variable>();

                    foreach (KeyValuePair<string, object> pair in parametrosETLAnular)
                    {
                        variablesAnular.Add(new PackagesExecutionService.Variable() { Key = pair.Key, Value = pair.Value });
                        ParametrosEtl = ParametrosEtl + pair.Key + ": " + pair.Value + "; ";
                    }

                    cmdReport1.Parameters.Add("parametrosEtlAnular", SqlDbType.VarChar);
                    cmdReport1.Parameters["parametrosEtlAnular"].Value = ParametrosEtl;                

                    cmdReport1.CommandTimeout = 0;//Timeout infinito
                    cmdReport1.Connection.Open();
                    var rta = cmdReport1.ExecuteNonQuery();
                    res.Resultado = ResultadoOperacion.Exito;

                    #endregion
                    #region PASO 02 - Ejecución ETL Anular

                    ETLsRemotas etlsRemotasNeg = new ETLsRemotas();
                    List<ETLRemota> etlsRemotas = etlsRemotasNeg.ListarETLsRemotasporTipo(4);
                    ETLRemota eTLRemota = etlsRemotas.Where(x => x.nombre == "Anular").FirstOrDefault();

                    using (PackagesExecutionServiceClient client = new PackagesExecutionServiceClient())
                    {
                        DTSResponse dtsResponse = client.ExecuteFromPackageFile(idApp, eTLRemota.packageFileName, eTLRemota.packageConfigFileName, variablesAnular.ToArray());
                        rta02 = (!dtsResponse.Fail ? 0 : 7);
                    }

                    res.Resultado = ResultadoOperacion.Exito;

               #endregion
                }
                res.Resultado = ResultadoOperacion.Exito;
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
            }
            return res;

        }

        #endregion

        # region reprocesar liquidacion

        internal ResultadoOperacionBD ReprocesarLiquidacion(string idApp, Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, Dictionary<string, object> parametrosEtlAnulacion, int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec, InfoAplicacion info)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();

            try
            {
                res = ExtractAnulacion(idApp, parametrosEtlAnulacion, liquidacionComisionId);
                res = ExtractCf_CV(idApp, parametrosEtlCF, parametrosEtlCV, anio, mes, 1, liquidacionComisionId, tipoLiquidacionId, usuario, tipoEjec, info);
            }
            catch (Exception ex)
            {
                res.RegistrosAfectados = 0;
                res.MensajeError = "Error: " + ex.Message;
                res.Resultado = ResultadoOperacion.Error;
            }
            


            return res;
        
        }

        internal ResultadoOperacionBD ActualizaEstadoReprocesar(int idLiquidacion)
        {
            ResultadoOperacionBD res = new ResultadoOperacionBD();
          
            string sentencia = "update LiquidacionComision set estadoliquidacion_id=5 where id=" + idLiquidacion+"";
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(sentencia.ToString(), sqlConn);
            cmdReport.CommandType = CommandType.Text;

            cmdReport.CommandTimeout = 0;

            cmdReport.Connection.Open();
            var rta = cmdReport.ExecuteNonQuery();
            cmdReport.Connection.Close();

            res.Resultado = ResultadoOperacion.Exito;
            return res;
        }

        //public  ResultadoOperacionBD ReprocesarLiquidacionMAC(int idLiquidacion)
        //{
        //    ResultadoOperacionBD res = new ResultadoOperacionBD();

        //    string sentencia = "EXEC ReprocesarLiquidacionMAC_LOG '" + idLiquidacion + "'," + 0 + "";
        //    EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
        //    SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
        //    SqlCommand cmdReport = new SqlCommand(sentencia.ToString(), sqlConn);
        //    cmdReport.CommandType = CommandType.Text;

        //    cmdReport.CommandTimeout = 0;

        //    cmdReport.Connection.Open();
        //    var rta = cmdReport.ExecuteNonQuery();

        //    cmdReport.Connection.Close();

        //    res.Resultado = ResultadoOperacion.Exito;
        //    return res;
        //}

        internal List<int> ValReprocesoLiq()
        {
            List<int> filtEnProceso = new List<int>();
            try
            {
                filtEnProceso = (from dd in _dbcontext.LiquidacionComisions
                                 where dd.estadoliquidacion_id == 5 //Reprocesando a través de ETL
                                 select dd.id).ToList();
            }
            catch (Exception)
            {
                return null;
            }
            LiquidacionComision abc = new LiquidacionComision();


            return filtEnProceso;
        }
       
        #endregion

    }
}
