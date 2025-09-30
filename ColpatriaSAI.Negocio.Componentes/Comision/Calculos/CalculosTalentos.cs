using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Datos;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Calculos
{
    public class CalculosTalentos
    {

        private SAI_Entities _dbcontext = new SAI_Entities();


        public List<ResultadosCalculos> CalculoTalentos(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int), string Username = default(string))
        {
            List<ResultadosCalculos> pValores = new List<ResultadosCalculos>();
            List<ResultadosCalculos> pValoresTemp = new List<ResultadosCalculos>();
            int IDPeriodo = 0;
            string lCondiciones = string.Format("YEAR(FECHAINICIO) = {0} AND MONTH(FECHAINICIO) = {1}  AND YEAR(FECHAFIN) = {2} AND MONTH(FECHAFIN) = {1} ", (pAnio - 1), pMes, pAnio);
            IDPeriodo = ConsultarAdicional(string.Format("SELECT MAX(ID) FROM PERIODOCALCULOCOMISION WHERE {0}", lCondiciones));
            if (IDPeriodo <= 0)
            {
                IDPeriodo = ConsultaIdentity("PERIODOCALCULOCOMISION");
                StringBuilder lSentencia = new StringBuilder("INSERT INTO PERIODOCALCULOCOMISION (ID,FECHAINICIO,FECHAFIN) ");


                lSentencia.AppendFormat("VALUES ({0},'{1}-{2}-01','{3}-{2}-01')", IDPeriodo, (pAnio - 1), pMes, pAnio);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.PeriodoCalculoComision,
                    SegmentosInsercion.Personas_Y_Pymes, null, lSentencia.ToString());
                int r = _dbcontext.ExecuteStoreCommand(lSentencia.ToString(), null);
            }


            pValoresTemp = CalculoTalentosNoRestriccion(pAnio, pMes, pIdModelo, pAsesor);

            pValores.AddRange(pValoresTemp);

            return pValores;

        }

        #region CalculosTalentos

        List<ResultadosCalculos> CalculoTalentosNoRestriccion(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int))
        {
            List<ResultadosCalculos> result = new List<ResultadosCalculos>();
            StringBuilder lSentencia;

            lSentencia = new StringBuilder("SELECT COUNT(1) AS BENEFICIARIOS, FCVN.factor ");
            lSentencia.Append("FROM TB_REP_BENEFICIARIO B INNER JOIN PARTICIPANTE P ON (P.ID = B.PARTICIPANTE_ID) ");
            lSentencia.Append("INNER JOIN FACTORCOMISIONVARIABLENUEVO FCVN ON FCVN.COMPANIA_ID = B.COMPAÑIA_ID AND FCVN.RAMODETALLE_ID = B.RAMO_ID ");
            lSentencia.Append("AND (isnull(FCVN.PRODUCTODETALLE_ID, B.PRODUCTO_ID) = B.PRODUCTO_ID) ");
            lSentencia.Append("AND (isnull(FCVN.PLANDETALLE_ID,B.PLAN_ID ) = B.PLAN_ID) ");
            lSentencia.Append("AND (FCVN.TIPOCONTRATO_ID = B.TIPOCONTRATO_ID)");
            lSentencia.Append("AND B.ESTADOBENEFICIARIO_ID IN ('1') ");
            lSentencia.AppendFormat("AND (B.MES = {0} AND B.AÑO = {1}) ", pMes, pAnio);
            lSentencia.AppendFormat("AND B.PARTICIPANTE_ID = {0} AND FCVN.MODELO_ID = {1} ", pAsesor, pIdModelo);
            lSentencia.Append("INNER JOIN MODELOCOMISIONCANALDETALLE MCDT ON (MCDT.MODELO_ID = FCVN.MODELO_ID AND MCDT.CANALDETALLE_ID = P.CANALDETALLE_ID) ");
            lSentencia.Append("GROUP BY B.PARTICIPANTE_ID, FCVN.factor ");

            ResultadosCalculos vresultNuevosUsuario = new ResultadosCalculos() { CombinacionQuery = "# de Usuarios Nuevos" };
            vresultNuevosUsuario.resultados = new List<int>();
            vresultNuevosUsuario.Combinaciones = new List<string>();

            ResultadosCalculos vresultTalentos = new ResultadosCalculos() { CombinacionQuery = "No. Talentos por Usuarios Nuevos " };
            vresultTalentos.resultados = new List<int>();
            vresultTalentos.Combinaciones = new List<string>();

            DataTable lResults = new DataTable();
            lResults = GetResults(lSentencia.ToString());

            foreach (DataRow lrow in lResults.Rows)
            {
                int lbeneficiarios = int.Parse(lrow["BENEFICIARIOS"].ToString());
                int lFactor = int.Parse(lrow["FACTOR"].ToString());

                vresultNuevosUsuario.resultados.Add(lbeneficiarios);
                vresultTalentos.resultados.Add(lbeneficiarios * lFactor);
            }

            if (!vresultNuevosUsuario.resultados.Any())
            {
                vresultNuevosUsuario = new ResultadosCalculos() { CombinacionQuery = "No existen valores Nuevos para el filtro seleccionado " };


                vresultTalentos = new ResultadosCalculos() { CombinacionQuery = "" };

            }
            result.Add(vresultNuevosUsuario);
            result.Add(vresultTalentos);

            return result;
        }

        #endregion


        #region Metodos De Apoyo

        public int ConsultaIdentity(string pNombreTabla)
        {
            int result = 0;
            StringBuilder lSentencia = new StringBuilder(string.Format("Select MAX(ID) from {0}", pNombreTabla.Trim().ToUpper()));
            result = _dbcontext.ExecuteStoreQuery<int?>(lSentencia.ToString(), null).Select(i => (i == null) ? 0 : Convert.ToInt32(i)).FirstOrDefault();
            result += 1;
            return result;
        }

        public int ConsultarAdicional(string pSentencia)
        {
            int result = 0;

            result = _dbcontext.ExecuteStoreQuery<int?>(pSentencia, null).Select(i => (i == null) ? 0 : Convert.ToInt32(i)).FirstOrDefault();
            return result;
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

        #endregion
    }
}
