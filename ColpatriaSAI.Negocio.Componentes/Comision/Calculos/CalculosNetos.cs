using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Negocio.Entidades;
using System.Data;
using ColpatriaSAI.Datos;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Calculos
{
    public class CalculosNetos
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<ResultadosCalculos> CalculoNetos(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int), string Username = default(string))
        {
            List<ResultadosCalculos> pValores = new List<ResultadosCalculos>();
            List<ResultadosCalculos> pValoresTemp = new List<ResultadosCalculos>();


            pValoresTemp = CalculoNetosAnioActual(pAnio, pMes, pIdModelo, pAsesor);
            if (pValoresTemp[0].resultados.Any())
            {
                pValores.AddRange(pValoresTemp);
            }

            if (!pValores.Any())
            {
                pValores.Add(new ResultadosCalculos() { CombinacionQuery = string.Format("Usuarios para el mes {0} del año {1}", pMes, pAnio), resultados = new List<int>(), Combinaciones = new List<string>() });
                pValores.Add(new ResultadosCalculos() { CombinacionQuery = string.Format("Usuarios para el mes {0} del año {1}", pMes, (pAnio - 1)), resultados = new List<int>(), Combinaciones = new List<string>() });
                pValores.Add(new ResultadosCalculos() { CombinacionQuery = "# de Usuarios Netos", resultados = new List<int>(), Combinaciones = new List<string>() });
            }

            return pValores;
        }

        #region CalculosNetos

        List<ResultadosCalculos> CalculoNetosAnioActual(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int), string Username = default(string))
        {
            int IDPeriodo = 0;
            string lCondiciones = string.Format("YEAR(FECHAINICIO) = {0} AND MONTH(FECHAINICIO) = {1} AND YEAR(FECHAFIN) = {2} AND MONTH(FECHAFIN) = {1} ", (pAnio - 1), pMes, pAnio);
            IDPeriodo = ConsultarAdicional(string.Format("SELECT MAX(ID) FROM PERIODOCALCULOCOMISION WHERE {0}", lCondiciones));
            if (IDPeriodo <= 0)
            {
                IDPeriodo = ConsultaIdentity("PERIODOCALCULOCOMISION");
                StringBuilder ls = new StringBuilder("INSERT INTO PERIODOCALCULOCOMISION (ID,FECHAINICIO,FECHAFIN) ");
                ls.AppendFormat("VALUES ({0},'01-{1}-{2}','31-{1}-{3}')", IDPeriodo, pMes, (pAnio - 1), pAnio);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.PeriodoCalculoComision,
                    SegmentosInsercion.Personas_Y_Pymes, null, ls.ToString());
                int r = _dbcontext.ExecuteStoreCommand(ls.ToString(), null);
            }

            ResultadosCalculos vResultNetoAnioActual = new ResultadosCalculos() { CombinacionQuery = string.Format("Usuarios para el mes {0} del año {1}", pMes, pAnio) };
            vResultNetoAnioActual.resultados = new List<int>();
            vResultNetoAnioActual.Combinaciones = new List<string>();

            ResultadosCalculos vResultNetoAnioAnterior = new ResultadosCalculos() { CombinacionQuery = string.Format("Usuarios para el mes {0} del año {1}", pMes, (pAnio - 1)) };
            vResultNetoAnioAnterior.resultados = new List<int>();
            vResultNetoAnioAnterior.Combinaciones = new List<string>();

            ResultadosCalculos vResultUsuarioNetos = new ResultadosCalculos() { CombinacionQuery = "# de Usuarios Netos " };
            vResultUsuarioNetos.resultados = new List<int>();
            vResultUsuarioNetos.Combinaciones = new List<string>();

            ResultadosCalculos vResultUsuariosPenalizados = new ResultadosCalculos() { CombinacionQuery = "# de Usuarios Penalizados " };
            vResultUsuariosPenalizados.resultados = new List<int>();
            vResultUsuariosPenalizados.Combinaciones = new List<string>();

            ResultadosCalculos vResultUsuariosConExcepcion = new ResultadosCalculos() { CombinacionQuery = "# de Excepciones " };
            vResultUsuariosConExcepcion.resultados = new List<int>();
            vResultUsuariosConExcepcion.Combinaciones = new List<string>();

            ResultadosCalculos vResultUsuariosTotales = new ResultadosCalculos() { CombinacionQuery = "# de Usuarios Netos Totales " };
            vResultUsuariosTotales.resultados = new List<int>();
            vResultUsuariosTotales.Combinaciones = new List<string>();

            ResultadosCalculos vResultTalentosNetos = new ResultadosCalculos() { CombinacionQuery = "Talentos Netos " };
            vResultTalentosNetos.resultados = new List<int>();
            vResultTalentosNetos.Combinaciones = new List<string>();

            ResultadosCalculos vResultTalentosNetosTotales = new ResultadosCalculos() { CombinacionQuery = "Talentos Netos Total " };
            vResultTalentosNetosTotales.resultados = new List<int>();
            vResultTalentosNetosTotales.Combinaciones = new List<string>();

            List<ResultadosCalculos> result = new List<ResultadosCalculos>();

            DataTable lResults = ObtenerUsuariosNetosPorAnioMesModeloAsesor(pAnio, pMes, pIdModelo, pAsesor);
            var Asesores = _dbcontext.Participantes.ToList();
            List<int> lbeneficiarios = new List<int>();
            List<int> lbeneficiarios2 = new List<int>();
            foreach (DataRow lrow in lResults.Rows)
            {
                int benecount = int.Parse(lrow["BENEFICIARIOS"].ToString());
                int lFactor = (lrow["FACTOR"] == DBNull.Value ? 0 : int.Parse(lrow["FACTOR"].ToString()));
                vResultNetoAnioActual.Combinaciones.Add(string.Format("({0})", lrow["TIPO"].ToString()));
                vResultTalentosNetos.resultados.Add(benecount * lFactor);
                vResultNetoAnioActual.resultados.Add(benecount);
                lbeneficiarios.Add(benecount);
            }

            int lUsuariosNetos = 0;
            DataTable lResultsAnioAnterior = ObtenerUsuariosNetosPorAnioMesModeloAsesor(pAnio - 1, pMes, pIdModelo, pAsesor);
            foreach (DataRow lrow in lResultsAnioAnterior.Rows)
            {
                int benecount = int.Parse(lrow["BENEFICIARIOS"].ToString());
                int lFactor = (lrow["FACTOR"] == DBNull.Value ? 0 : int.Parse(lrow["FACTOR"].ToString()));
                vResultNetoAnioAnterior.Combinaciones.Add(string.Format("({0})", lrow["TIPO"].ToString()));
                vResultNetoAnioAnterior.resultados.Add(benecount);
                vResultTalentosNetos.resultados.Add((benecount * lFactor) * -1);
                lbeneficiarios2.Add(benecount);

            }
            lUsuariosNetos = lbeneficiarios.Sum() - lbeneficiarios2.Sum();
            vResultUsuarioNetos.resultados.Add(lUsuariosNetos);


            int lPenalizacion = 0;
            string claveAsesor = Asesores.Where(x => x.id == pAsesor).FirstOrDefault().clave;
            StringBuilder qFactoresPena = new StringBuilder("Select count(1) beneficiarios, FCVN.factor ");
            qFactoresPena.Append("from TB_TRANSFERENCIAS tr ");
            qFactoresPena.Append("inner join (select id, compañia_id,ramo_id,producto_id,plan_id,tipocontrato_id from TB_REP_Beneficiario) b on b.id = tr.BENEFICIARIO_ID ");
            qFactoresPena.Append("inner join FACTORCOMISIONVARIABLENETO FCVN ");
            qFactoresPena.Append("ON FCVN.COMPANIA_ID = B.COMPAÑIA_ID ");
            qFactoresPena.Append("AND FCVN.RAMODETALLE_ID = B.RAMO_ID ");
            qFactoresPena.AppendFormat("AND FCVN.MODELO_ID = {0} ", pIdModelo);
            qFactoresPena.Append("AND (FCVN.TIPOCONTRATO_ID = B.tipocontrato_id) ");
            qFactoresPena.Append("where (ISNULL(FCVN.PRODUCTODETALLE_ID,b.producto_id) = b.producto_id) ");
            qFactoresPena.Append("AND (Isnull(FCVN.PLANDETALLE_ID, B.plan_id) = B.plan_id)  ");
            qFactoresPena.AppendFormat("and tr.ASESOR_NEW = {0} ", claveAsesor);
            qFactoresPena.AppendFormat("and tr.fecha < DATEADD(MONTH, 1,CAST('{0}-{1}-01' as date)) ", pAnio, pMes);
            qFactoresPena.AppendFormat("AND tr.fecha >=  DATEADD(year,-1,DATEADD(day, -1, DATEADD(MONTH, 1, CAST('{0}-{1}-01' as date)))) ", pAnio, pMes);
            qFactoresPena.Append("group by FCVN.factor");
            DataTable dtFactoresPena = GetResults(qFactoresPena.ToString());
            int ltPenalizacionez = 0;
            foreach (DataRow lrow in dtFactoresPena.Rows)
            {
                int numBeneficiarios = int.Parse(lrow["beneficiarios"].ToString());
                int pFactor = int.Parse(lrow["factor"].ToString());
                lPenalizacion += numBeneficiarios;
                ltPenalizacionez += numBeneficiarios * pFactor;
            }
            int lUsuariosPenalizadosConExcepcion = 0, lTalentosUsuariosPenalizadosPenalizacion = 0;
            StringBuilder qFactoresExcepcionPena = new StringBuilder("Select count(1) beneficiarios, FCVN.factor ");
            qFactoresExcepcionPena.Append("from TB_TRANSFERENCIAS tr ");
            qFactoresExcepcionPena.Append("inner join (select id, compañia_id,ramo_id,producto_id,plan_id,tipocontrato_id, numeronegociopadre from TB_REP_Beneficiario) b on b.id = tr.BENEFICIARIO_ID ");
            qFactoresExcepcionPena.Append("inner join Participante p ");
            qFactoresExcepcionPena.AppendFormat("on p.clave = {0} ",claveAsesor);
            qFactoresExcepcionPena.Append("inner join ExcepcionPenalizacion ep ");
            qFactoresExcepcionPena.Append("on b.numeronegociopadre = ep.numerocontrato and (p.id = ep.participantedestino_id) ");
            qFactoresExcepcionPena.Append("inner join FACTORCOMISIONVARIABLENETO FCVN ");
            qFactoresExcepcionPena.Append("ON FCVN.COMPANIA_ID = B.COMPAÑIA_ID ");
            qFactoresExcepcionPena.Append("AND FCVN.RAMODETALLE_ID = B.RAMO_ID ");
            qFactoresExcepcionPena.AppendFormat("AND FCVN.MODELO_ID = {0} ", pIdModelo);
            qFactoresExcepcionPena.Append("AND (FCVN.TIPOCONTRATO_ID = B.tipocontrato_id) ");
            qFactoresExcepcionPena.Append("where (ISNULL(FCVN.PRODUCTODETALLE_ID,b.producto_id) = b.producto_id) ");
            qFactoresExcepcionPena.Append("AND (Isnull(FCVN.PLANDETALLE_ID, B.plan_id) = B.plan_id)  ");
            qFactoresExcepcionPena.AppendFormat("and tr.ASESOR_NEW = {0} ", claveAsesor);
            qFactoresExcepcionPena.AppendFormat("AND tr.fecha >=  DATEADD(year,-1,DATEADD(day, -1, DATEADD(MONTH, 1, CAST('{0}-{1}-01' as date)))) ", pAnio, pMes);
            qFactoresExcepcionPena.Append("and ep.activo = 1 and ep.aplica in ('Ambas','Destino') ");
            qFactoresExcepcionPena.Append("group by FCVN.factor union ");

            qFactoresExcepcionPena.Append("Select count(1) beneficiarios, FCVN.factor ");
            qFactoresExcepcionPena.Append("from TB_TRANSFERENCIAS tr ");
            qFactoresExcepcionPena.Append("inner join (select id, compañia_id,ramo_id,producto_id,plan_id,tipocontrato_id, numeronegociopadre from TB_REP_Beneficiario) b on b.id = tr.BENEFICIARIO_ID ");
            qFactoresExcepcionPena.Append("inner join Participante p ");
            qFactoresExcepcionPena.AppendFormat("on p.clave = {0} ", claveAsesor);
            qFactoresExcepcionPena.Append("inner join ExcepcionPenalizacion ep ");
            qFactoresExcepcionPena.Append("on b.numeronegociopadre = ep.numerocontrato and (p.id = ep.participanteorigen_id) ");
            qFactoresExcepcionPena.Append("inner join FACTORCOMISIONVARIABLENETO FCVN ");
            qFactoresExcepcionPena.Append("ON FCVN.COMPANIA_ID = B.COMPAÑIA_ID ");
            qFactoresExcepcionPena.Append("AND FCVN.RAMODETALLE_ID = B.RAMO_ID ");
            qFactoresExcepcionPena.AppendFormat("AND FCVN.MODELO_ID = {0} ", pIdModelo);
                qFactoresExcepcionPena.Append("AND (FCVN.TIPOCONTRATO_ID = B.tipocontrato_id) ");
                qFactoresExcepcionPena.Append("where (ISNULL(FCVN.PRODUCTODETALLE_ID,b.producto_id) = b.producto_id) ");
            qFactoresExcepcionPena.Append("AND (Isnull(FCVN.PLANDETALLE_ID, B.plan_id) = B.plan_id)  ");
            qFactoresExcepcionPena.AppendFormat("and tr.ASESOR_OLD = {0} ", claveAsesor);
            qFactoresExcepcionPena.AppendFormat("AND tr.fecha >=  DATEADD(year,-1,DATEADD(day, -1, DATEADD(MONTH, 1, CAST('{0}-{1}-01' as date)))) ", pAnio, pMes);
            qFactoresExcepcionPena.Append("and ep.activo = 1 and ep.aplica in ('Ambas','Origen') ");
            qFactoresExcepcionPena.Append("group by FCVN.factor");

            DataTable dtFactoresExcepcionPenalizacion = GetResults(qFactoresExcepcionPena.ToString());
            foreach (DataRow lrow in dtFactoresExcepcionPenalizacion.Rows)
            {
                int numBeneficiarios = int.Parse(lrow["beneficiarios"].ToString());
                int pFactor = int.Parse(lrow["factor"].ToString());
                lUsuariosPenalizadosConExcepcion+= numBeneficiarios;
                lTalentosUsuariosPenalizadosPenalizacion += numBeneficiarios * pFactor;
            }
            //vResultUsuarioNetos.resultados.Add(lUsuariosPenalizadosConExcepcion);
            vResultUsuariosConExcepcion.resultados.Add(lUsuariosPenalizadosConExcepcion);
            vResultUsuariosPenalizados.resultados.Add(lPenalizacion);
            lPenalizacion = lPenalizacion - lUsuariosPenalizadosConExcepcion;
            lUsuariosNetos = (vResultUsuarioNetos.resultados.Sum()) - lPenalizacion;
            vResultUsuariosTotales.resultados.Add(lUsuariosNetos);
            vResultTalentosNetosTotales.resultados.Add((vResultTalentosNetos.resultados.Sum() + lTalentosUsuariosPenalizadosPenalizacion) - (ltPenalizacionez));
            result.Add(vResultUsuarioNetos);
            result.Add(vResultUsuariosPenalizados);
            result.Add(vResultUsuariosConExcepcion);
            result.Add(vResultUsuariosTotales);
            result.Add(vResultTalentosNetos);
            result.Add(vResultTalentosNetosTotales);

            return result;

        }

        private DataTable ObtenerUsuariosNetosPorAnioMesModeloAsesor(int pAnio, int pMes, int pIdModelo, int pAsesor)
        {
            StringBuilder lSentencia = new StringBuilder("SELECT COUNT(1) AS BENEFICIARIOS,FCVN.FACTOR,CONCAT(FCVN.COMPANIA_ID,',', FCVN.RAMODETALLE_ID ,',', FCVN.PRODUCTODETALLE_ID ,',', FCVN.PLANDETALLE_ID) TIPO,FCVN.COMPANIA_ID, ");
            lSentencia.Append("FCVN.PRODUCTODETALLE_ID,FCVN.RAMODETALLE_ID,FCVN.PLANDETALLE_ID FROM TB_REP_BENEFICIARIO B INNER JOIN FACTORCOMISIONVARIABLENETO FCVN ON FCVN.COMPANIA_ID = B.COMPAÑIA_ID AND FCVN.RAMODETALLE_ID = B.RAMO_ID ");
            lSentencia.Append("INNER JOIN Participante p ON p.id = B.participante_id  ");
            lSentencia.AppendFormat("INNER JOIN ModeloComisionCanalDetalle mccd ON p.canaldetalle_id = mccd.canaldetalle_id AND mccd.modelo_id = {0} ", pIdModelo);
            lSentencia.Append("WHERE (ISNULL(FCVN.PRODUCTODETALLE_ID,b.producto_id) = b.producto_id) ");
            lSentencia.Append("AND (Isnull(FCVN.PLANDETALLE_ID, B.plan_id) = B.plan_id)  AND (FCVN.TIPOCONTRATO_ID = B.tipocontrato_id) ");
            lSentencia.AppendFormat("AND (B.año = {0} AND B.mes = {1} ) ", pAnio, pMes);
            lSentencia.AppendFormat("AND B.PARTICIPANTE_ID = {0} AND FCVN.MODELO_ID = {1} ", pAsesor, pIdModelo);
            lSentencia.Append("AND B.ESTADOBENEFICIARIO_ID IN (4) GROUP BY FCVN.COMPANIA_ID, FCVN.RAMODETALLE_ID, FCVN.PRODUCTODETALLE_ID, ");
            lSentencia.Append("FCVN.PLANDETALLE_ID,FCVN.FACTOR,FCVN.COMPANIA_ID,FCVN.PRODUCTODETALLE_ID,FCVN.RAMODETALLE_ID,FCVN.PLANDETALLE_ID ");

            DataTable lResults = new DataTable();
            lResults = GetResults(lSentencia.ToString());
            return lResults;
        }
        #endregion

        #region InsertarValoresNetos

        private bool InsertValores(int pUsuariosNetos, int pAnio, int pMes, int pIdModelo, int pAsesor, int pIdPeriodo, int pCuentaBeneficiarios, out int pId, string Username)
        {
            bool lResult = false;
            pId = 0;
            StringBuilder lSentencia = new StringBuilder("INSERT INTO TALENTOSNETOSASESORPERIODO ");
            lSentencia.Append("(ID,PARTICIPANTE_ID,COMPANIA_ID,RAMODETALLE_ID,PRODUCTODETALLE_ID,PLANDETALLE_ID,TIPOCONTRATO_ID,NUMERONETOS) ");
            int ID = ConsultaIdentity("TALENTOSNETOSASESORPERIODO");
            pId = ID;
            lSentencia.AppendFormat("SELECT {0},{1},FCVN.COMPANIA_ID, FCVN.RAMODETALLE_ID,FCVN.PRODUCTODETALLE_ID, FCVN.PLANDETALLE_ID, FCVN.TIPOCONTRATO_ID,{2} ", ID, pAsesor, pUsuariosNetos);
            lSentencia.Append("FROM TB_REP_BENEFICIARIO B ");
            lSentencia.Append("INNER JOIN FACTORCOMISIONVARIABLENETO FCVN ");
            lSentencia.Append("ON FCVN.COMPANIA_ID = B.COMPAÑIA_ID AND FCVN.RAMODETALLE_ID = B.RAMO_ID ");
            lSentencia.Append("WHERE (Isnull(FCVN.PRODUCTODETALLE_ID,B.producto_id) = B.producto_id) ");
            lSentencia.Append("AND (Isnull(FCVN.PLANDETALLE_ID, B.plan_id ) = B.plan_id) ");
            lSentencia.Append("AND (FCVN.TIPOCONTRATO_ID = B.tipocontrato_id) ");
            lSentencia.AppendFormat("AND (B.año = {0} AND B.mes = {1}) ", pAnio, pMes);
            lSentencia.AppendFormat("AND B.PARTICIPANTE_ID = {0} AND FCVN.MODELO_ID = {1} ", pAsesor, pIdModelo);
            lSentencia.Append("AND B.estadobeneficiario_id IN (4) ");
            lSentencia.Append("GROUP BY FCVN.COMPANIA_ID, FCVN.RAMODETALLE_ID, FCVN.PRODUCTODETALLE_ID, FCVN.PLANDETALLE_ID,FCVN.TIPOCONTRATO_ID,FCVN.FACTOR ");
            lSentencia.AppendFormat("HAVING COUNT(1) = {0}", pCuentaBeneficiarios);

            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNeto,
                    SegmentosInsercion.Personas_Y_Pymes, null, pCuentaBeneficiarios);
            int r = _dbcontext.ExecuteStoreCommand(lSentencia.ToString(), null);
            lResult = r > 0;
            return lResult;
        }

        #endregion

        #region ConsultaNetosAñosAnteriores

        private int ConsultarNetosAñoAnterior(int AnioAnterior, int Mes, int Modelo, int Factor, int Asesor = default(int), int Compania = default(int), int Ramo = default(int), int Producto = default(int), int Plan = default(int))
        {
            int lResult = 0;
            StringBuilder lSentencia = new StringBuilder("SELECT COUNT(1) AS BENEFICIARIOS FROM TB_REP_BENEFICIARIO B INNER JOIN FACTORCOMISIONVARIABLENETO FCVN ON FCVN.COMPANIA_ID = B.COMPAÑIA_ID AND FCVN.RAMODETALLE_ID = B.RAMO_ID ");
            lSentencia.Append("INNER JOIN Participante p ON p.id = B.participante_id  ");
            lSentencia.AppendFormat("INNER JOIN ModeloComisionCanalDetalle mccd ON p.canaldetalle_id = mccd.canaldetalle_id AND mccd.modelo_id = {0} ", Modelo);
            lSentencia.Append(" WHERE (Isnull(FCVN.PRODUCTODETALLE_ID, B.producto_id) = B.producto_id ) ");
            lSentencia.Append("AND (Isnull(FCVN.PLANDETALLE_ID,B.plan_id ) = B.plan_id)  AND (FCVN.TIPOCONTRATO_ID = B.TIPOCONTRATO_ID) ");
            lSentencia.AppendFormat("AND (B.año = {0} AND B.mes = {1}) ", AnioAnterior, Mes);
            lSentencia.AppendFormat("AND B.PARTICIPANTE_ID = {0} AND FCVN.MODELO_ID = {1} ", Asesor, Modelo);
            lSentencia.AppendFormat("AND FCVN.FACTOR = {0}", Factor);
            lSentencia.AppendFormat("AND B.COMPAÑIA_ID = {0} AND B.RAMO_ID = {1} ", Compania, Ramo);
            lSentencia.Append("AND B.estadobeneficiario_id IN (4) GROUP BY FCVN.COMPANIA_ID, FCVN.RAMODETALLE_ID, FCVN.PRODUCTODETALLE_ID, ");
            lSentencia.Append("FCVN.PLANDETALLE_ID,FCVN.FACTOR,FCVN.COMPANIA_ID,FCVN.PRODUCTODETALLE_ID,FCVN.RAMODETALLE_ID,FCVN.PLANDETALLE_ID ");


            try
            {
                DataTable lResults = new DataTable();
                lResults = GetResults(lSentencia.ToString());
                foreach (DataRow lrow in lResults.Rows)
                {
                    lResult = (lrow["BENEFICIARIOS"] == DBNull.Value ? 0 : int.Parse(lrow["BENEFICIARIOS"].ToString()));
                }
            }
            catch (Exception ex)
            {
                lResult = 0;
            }
            return lResult;
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
