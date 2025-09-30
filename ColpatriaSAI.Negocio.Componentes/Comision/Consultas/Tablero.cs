using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ColpatriaSAI.Datos;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Consultas
{
    public class Tablero
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        #region consultas tablero
        public List<ListadoUsuarios> traer()
        {

            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select codigoextraccion, (select count(documento) from TB_REP_Beneficiario b ");
            lSentencia.Append(" where b.codigoextraccion = r.codigoextraccion) as 'usersEntrantes',");
            lSentencia.Append(" (select SUM(conteo) from [LogProcesoExtraccionBH_SAI] where Descripcion_log not in ('Total Registros Facturas BH Origen','Total Registros Recaudo BH Origen') ");
            lSentencia.Append(" and  CodExtraccion=codigoextraccion) as 'UsuariosSalientes'");
            lSentencia.Append(" from SAI.[dbo].TB_REP_Recaudo r ");
            lSentencia.Append(" group by codigoextraccion ");
            lSentencia.Append("  order by codigoextraccion desc ");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                                select new ListadoUsuarios()
                                {
                                    numExtraccion = item["codigoextraccion"].ToString(),
                                    usuariosEntrantes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["usersEntrantes"].ToString()) ? "0" : item["usersEntrantes"].ToString())),
                                    UsuariosSalientes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["UsuariosSalientes"].ToString()) ? "0" : item["UsuariosSalientes"].ToString()))
                                };



            //retVal.Tables[0].Select().ToList()
            return UserFiltrados.ToList();
        }

        public List<ListadoUsuarios> traerFiltroLogUsuarios(string fechIni, string fechFinal)
        {

            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select codigoextraccion, (select count(documento) from TB_REP_Beneficiario b");
            lSentencia.Append(" where b.codigoextraccion = codigoextraccion) as 'usersEntrantes',");
            lSentencia.Append(" (select SUM(conteo) from [LogProcesoExtraccionBH_SAI] where Descripcion_log not in ('Total Registros Facturas BH Origen','Total Registros Recaudo BH Origen') ");
            lSentencia.Append(" and  CodExtraccion=codigoextraccion) as 'UsuariosSalientes'");
            lSentencia.Append(" from SAI.[dbo].TB_REP_Recaudo");
            lSentencia.Append(" where fechaextraccion between '" + fechIni + " 00:00:00:000' and '" + fechFinal + " 23:59:59:999' ");
            lSentencia.Append(" group by codigoextraccion");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                                select new ListadoUsuarios()
                                {
                                    numExtraccion = item["codigoextraccion"].ToString(),
                                    usuariosEntrantes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["usersEntrantes"].ToString()) ? "0" : item["usersEntrantes"].ToString())),
                                    UsuariosSalientes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["UsuariosSalientes"].ToString()) ? "0" : item["UsuariosSalientes"].ToString()))
                                };



            //retVal.Tables[0].Select().ToList()
            return UserFiltrados.ToList();
        }


        /// <summary>
        /// Lista las facturas entrantes vs las salientes por codigo de extracción
        /// </summary>
        /// <returns></returns>
        public List<ListadoUsuarios> ListarFacturasEntrantes()
        {

            StringBuilder lSentencia = new StringBuilder();

            lSentencia.Append("select codigoextraccion, (select SUM(conteo) from [LogProcesoExtraccionBH_SAI]");
            lSentencia.Append(" where  Descripcion_log ='Total Registros Facturas BH Origen' and CodExtraccion=codigoextraccion) as 'FacturasEntrantes',");
            lSentencia.Append("( select  count(distinct numerofactura) as Facturas");
            lSentencia.Append(" from SAI.[dbo].TB_REP_Recaudo r");
            lSentencia.Append(" where r.codigoextraccion=f.codigoextraccion");
            lSentencia.Append(" ) as 'FacturasSalientes'");
            lSentencia.Append(" from SAI.[dbo].TB_REP_Facturacion f");
            lSentencia.Append(" group by codigoextraccion");
            lSentencia.Append(" order by codigoextraccion desc");



            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                                select new ListadoUsuarios()
                                {
                                    numExtraccion = item["codigoextraccion"].ToString(),
                                    usuariosEntrantes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["FacturasEntrantes"].ToString()) ? "0" : item["FacturasEntrantes"].ToString())),
                                    UsuariosSalientes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["FacturasSalientes"].ToString()) ? "0" : item["FacturasSalientes"].ToString()))
                                };



            //retVal.Tables[0].Select().ToList()
            return UserFiltrados.ToList();
        }

        /// <summary>
        /// Listado de recaudos entrantes vs recaudos salientes organziados por codigo de extracción
        /// </summary>
        /// <returns></returns>
        public List<ListadoUsuarios> ListarRecaudosEntrantes()
        {

            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append(" select codigoextraccion, (select SUM(conteo) from [LogProcesoExtraccionBH_SAI]");
            lSentencia.Append(" where  Descripcion_log ='Total Registros Recaudo BH Origen' and CodExtraccion=codigoextraccion) as 'RecaudosEntrantes',");
            lSentencia.Append(" (select  count(distinct numerofactura) as Facturas");
            lSentencia.Append(" from SAI.[dbo].TB_REP_Recaudo r");
            lSentencia.Append(" where r.codigoextraccion=f.codigoextraccion");
            lSentencia.Append("  ) as 'RecaudosSalientes'");
            lSentencia.Append(" from SAI.[dbo].TB_REP_Recaudo f");
            lSentencia.Append(" group by codigoextraccion");
            lSentencia.Append("  order by codigoextraccion desc");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                                select new ListadoUsuarios()
                                {
                                    numExtraccion = item["codigoextraccion"].ToString(),
                                    usuariosEntrantes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["RecaudosEntrantes"].ToString()) ? "0" : item["RecaudosEntrantes"].ToString())),
                                    UsuariosSalientes = Convert.ToInt64((String.IsNullOrWhiteSpace(item["RecaudosSalientes"].ToString()) ? "0" : item["RecaudosSalientes"].ToString()))
                                };



            //retVal.Tables[0].Select().ToList()
            return UserFiltrados.ToList();
        }

        /// <summary>
        /// Este metodo lista el detalle de usuarios que no pasaron por codigo de extracción
        /// </summary>
        /// <param name="codExtraccion"></param>
        /// <returns></returns>
        public List<DetalleUsuarios> ListarDetalleUsuarios(string codExtraccion)
        {
            List<object> ListResult = new List<object>();
            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select Descripcion_log, sum(conteo) as 'conteo' from [LogProcesoExtraccionBH_SAI]");
            lSentencia.Append(" where descripcion_log in ('Usuario sin Ramo', 'Usuario sin Producto', 'Usuario sin Plan', ");
            lSentencia.Append(" 'Usuario sin Modalidad Financiacion', 'Usuario sin Tipo Beneficiario', 'Usuario sin Estado Beneficiario', 'Usuarios sin asesor')");
            lSentencia.Append(" and CodExtraccion = '" + codExtraccion + "'");
            lSentencia.Append(" group by Descripcion_log;");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                                select new DetalleUsuarios()
                                {
                                    Descripcion_log = item["Descripcion_log"].ToString(),
                                    conteo = Convert.ToInt64(item["conteo"].ToString())
                                };

            return UserFiltrados.ToList();
        }


        public List<LogExtraccionBH> ExcelDetalleUsuario(string codExtraccion, string tipoLog, string idUsuario)
        {
            List<object> ListResult = new List<object>();
            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("SELECT [MEM_NCODE],[TID_NCODE],[ITY_CTYPEIDENTIFICATION],[PER_CIDENTIFICATIONNUMBER],[PER_CLASTNAME] ");
            lSentencia.Append(" ,[PER_CMOTHERNAME],[PER_CFIRSTNAME],[PER_CMIDDLENAME],[AGE]");
            lSentencia.Append(" ,[ACO_CONTRACTCODE],[SUB_CONTRACT],[UTY_NCODE],[UTY_CNAME]");
            lSentencia.Append(" ,[INT_NCODE],[INT_CINTERMEDIARY_NAME],[INT_CKEY],[BRE_NCOMMISSIONPERCENT],[DIR_NCODE],[DIR_CNAME]");
            lSentencia.Append(" ,[CTY_NCODE],[CTY_CNAME],[PROPLA_NCODE],[MEM_DSTARTINGDATE],[MEM_DENDINGDATE],[PME_CDESCRIPTION]");
            lSentencia.Append(" ,[FST_NCODE],[FST_CNAME],[PME_NCODE],[ESTADO],[TIPOLOG],[CodExtraccion]");
            lSentencia.Append(" FROM [SAI].[dbo].[LogBeneficiariosSinClave]");
            lSentencia.Append(" WHERE codExtraccion='" + codExtraccion + "'");// campo obligatorio
            if (!string.IsNullOrEmpty(tipoLog))
                lSentencia.Append(" AND TIPOLOG ='" + tipoLog + "'");

            if (!string.IsNullOrEmpty(idUsuario))
                lSentencia.Append(" AND PER_CIDENTIFICATIONNUMBER IN (" + idUsuario + ")");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                                select new LogExtraccionBH()
                                {
                                    MEM_NCODE = item["MEM_NCODE"].ToString(),
                                    TID_NCODE = item["TID_NCODE"].ToString(),
                                    ITY_CTYPEIDENTIFICATION = item["ITY_CTYPEIDENTIFICATION"].ToString(),
                                    PER_CIDENTIFICATIONNUMBER = item["PER_CIDENTIFICATIONNUMBER"].ToString(),
                                    PER_CLASTNAME = item["PER_CLASTNAME"].ToString(),
                                    PER_CMOTHERNAME = item["PER_CFIRSTNAME"].ToString(),
                                    PER_CMIDDLENAME = item["PER_CMIDDLENAME"].ToString(),
                                    AGE = item["AGE"].ToString(),
                                    ACO_CONTRACTCODE = item["SUB_CONTRACT"].ToString(),
                                    UTY_NCODE = item["UTY_NCODE"].ToString(),
                                    UTY_CNAME = item["UTY_CNAME"].ToString(),
                                    INT_CINTERMEDIARY_NAME = item["INT_CINTERMEDIARY_NAME"].ToString(),
                                    INT_CKEY = item["INT_CKEY"].ToString(),
                                    BRE_NCOMMISSIONPERCENT = item["BRE_NCOMMISSIONPERCENT"].ToString(),
                                    DIR_NCODE = item["DIR_NCODE"].ToString(),
                                    DIR_CNAME = item["DIR_CNAME"].ToString(),
                                    CTY_NCODE = item["CTY_NCODE"].ToString(),
                                    CTY_CNAME = item["CTY_CNAME"].ToString(),
                                    PROPLA_NCODE = item["PROPLA_NCODE"].ToString(),
                                    MEM_DSTARTINGDATE = item["MEM_DSTARTINGDATE"].ToString(),
                                    MEM_DENDINGDATE = item["MEM_DENDINGDATE"].ToString(),
                                    PME_CDESCRIPTION = item["PME_CDESCRIPTION"].ToString(),
                                    FST_NCODE = item["FST_NCODE"].ToString(),
                                    FST_CNAME = item["FST_CNAME"].ToString(),
                                    PME_NCODE = item["PME_NCODE"].ToString(),
                                    ESTADO = item["ESTADO"].ToString(),
                                    TIPOLOG = item["TIPOLOG"].ToString(),
                                    CodExtraccion = item["CodExtraccion"].ToString(),
                                };

            return UserFiltrados.ToList();
        }

        /// <summary>
        /// Este metodo lista el detalle de facturacions que no pasaron por codigo de extracción
        /// </summary>
        /// <param name="codExtraccion"></param>
        /// <returns></returns>
       public List<DetalleFacturas> ListarDetalleFacturacion(string codExtraccion)
        {
            List<object> ListResult = new List<object>();
            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select Descripcion_log , sum(conteo) as 'conteo' from  LogProcesoExtraccionBH_SAI");
            lSentencia.Append(" where Descripcion_log  in ('Usuario sin Ramo', 'Usuario sin Producto',  'Usuario sin Plan','Usuario sin Modalidad Financiacion', 'Usuario sin Tipo Beneficiario',  'Usuario sin Estado Beneficiario', 'Usuario sin asesor')");
            lSentencia.Append(" and CodExtraccion = '" + codExtraccion + "'");
            lSentencia.Append(" group by Descripcion_log;");

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                                select new DetalleFacturas()
                                {
                                    Descripcion_log = item["Descripcion_log"].ToString(),
                                    conteo = Convert.ToInt64(item["conteo"].ToString())
                                };

            return UserFiltrados.ToList();
        }

       public List<DetalleFacturas> ListarDetalleRecaudo(string codExtraccion)
       {
           List<object> ListResult = new List<object>();
           StringBuilder lSentencia = new StringBuilder();
           lSentencia.Append("select Descripcion_log , sum(conteo) as 'conteo' from  LogProcesoExtraccionBH_SAI");
           lSentencia.Append(" where Descripcion_log  in ('RECAUDO SIN FACTURA')");
           lSentencia.Append(" and CodExtraccion = '" + codExtraccion + "'");
           lSentencia.Append(" group by Descripcion_log;");

           DataSet retVal = new DataSet();
           EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
           SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
           SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
           SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
           using (cmdReport)
           {
               daReport.Fill(retVal);
           }

           var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                               select new DetalleFacturas()
                               {
                                   Descripcion_log = item["Descripcion_log"].ToString(),
                                   conteo = Convert.ToInt64(item["conteo"].ToString())
                               };

           return UserFiltrados.ToList();
       }

       public List<LogExtFacturacion> ExcelDetalleFactura(string codExtraccion, string tipoLog)
       {
           List<LogExtFacturacion> Result = new List<LogExtFacturacion>();

           List<object> ListResult = new List<object>();
           StringBuilder lSentencia = new StringBuilder();
           lSentencia.Append("SELECT * ");
           lSentencia.Append(" FROM [SAI].[dbo].[LOG_EXTRACT_FACTURAS_BH]");
           lSentencia.Append(" WHERE codExtraccion='" + codExtraccion + "'");// campo obligatorio
           if (!string.IsNullOrEmpty(tipoLog))
               lSentencia.Append(" AND TIPOLOG ='" + tipoLog + "'");

           DataSet retVal = new DataSet();
           EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
           SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
           SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
           SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
           using (cmdReport)
           {
               daReport.Fill(retVal);
           }
           var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                               select new LogExtFacturacion()
                               {
                                   AIN_CLEGALCODE = item["AIN_CLEGALCODE"].ToString(),
                                   AIN_DINVOICEDATE = Convert.ToDateTime(item["AIN_DINVOICEDATE"]),
                                   AIN_NTAXES = Convert.ToDouble(item["AIN_NTAXES"]),
                                   AIN_NTOTALVALUE = item["AIN_NTOTALVALUE"].ToString(),
                                   ANO_DCREATEDDATE = item["ANO_DCREATEDDATE"].ToString(),
                                   ANO_DFINALDATE = item["ANO_DFINALDATE"].ToString(),
                                   ANO_NCODE = Convert.ToInt32(item["ANO_NCODE"]),
                                   ANO_NNOVELTYVALUE = item["ANO_NNOVELTYVALUE"].ToString(),
                                   BRE_NCOMMISSIONPERCENT = item["BRE_NCOMMISSIONPERCENT"].ToString(),
                                   CodExtraccion = item["CodExtraccion"].ToString(),
                                   CONTRACT_CODE = item["CONTRACT_CODE"].ToString(),
                                   CTY_CNAME = item["CTY_CNAME"].ToString(),
                                   ESTADO_ID = Convert.ToInt32(item["ESTADO_ID"]),
                                   EXTRACT_DATE = Convert.ToDateTime(item["EXTRACT_DATE"]),
                                   INT_CINTERMEDIARY_NAME = item["INT_CINTERMEDIARY_NAME"].ToString(),
                                   INT_CKEY = item["INT_CKEY"].ToString(),
                                   INT_NCODE = Convert.ToInt32(item["INT_NCODE"]),
                                   ITY_CSHORTNAME = item["ITY_CSHORTNAME"].ToString(),
                                   ITY_NCODE = item["ITY_NCODE"].ToString(),
                                   MEM_NCODE = Convert.ToInt32(item["MEM_NCODE"]),
                                   MVA_NVALUE = item["MVA_NVALUE"].ToString(),
                                   NOV_CDESCRIPTION = item["NOV_CDESCRIPTION"].ToString(),
                                   PCO_CFARE_CODE = item["PCO_CFARE_CODE"].ToString(),
                                   PER_CIDENTIFICATIONNUMBER = item["PER_CIDENTIFICATIONNUMBER"].ToString(),
                                   PLA_CNAME = item["PLA_CNAME"].ToString(),
                                   PLA_NCODE = item["PLA_NCODE"].ToString(),
                                   PRO_CNAME = item["PRO_CNAME"].ToString(),
                                   PRO_NCODE = item["PRO_NCODE"].ToString(),
                                   TOTAL_FACT = item["TOTAL_FACT"].ToString(),
                                   VALOR_ASESOR = item["VALOR_ASESOR"].ToString(),
                                   VALOR_USUARIO = item["VALOR_USUARIO"].ToString()
                               };

           return UserFiltrados.ToList();
       }


       public List<LogExtracionRecaudo > ExcelDetalleRecaudo(string codExtraccion, string tipoLog)
       {
           
           List<object> ListResult = new List<object>();
           StringBuilder lSentencia = new StringBuilder();
           lSentencia.Append("SELECT * ");
           lSentencia.Append(" FROM [SAI].[dbo].[LOG_EXTRACT_RECAUDO_BH]");
           lSentencia.Append(" WHERE codExtraccion='" + codExtraccion + "'");// campo obligatorio
           if (!string.IsNullOrEmpty(tipoLog))
               lSentencia.Append(" AND TIPOLOG ='" + tipoLog + "'");

           DataSet retVal = new DataSet();
           EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
           SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
           SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
           SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
           using (cmdReport)
           {
               daReport.Fill(retVal);
           }
           var UserFiltrados = from item in retVal.Tables[0].AsEnumerable()
                               select new LogExtracionRecaudo()
                               {
                                   EXTRACT_STATE = item["EXTRACT_STATE"].ToString(),
                                   PARTICIPATION = item["PARTICIPATION"].ToString(),
                                   CTY_NCODE = item["CTY_NCODE"].ToString(),
                                   AIN_CLEGALCODE = item["AIN_CLEGALCODE"].ToString(),
                                   AIN_DINVOICEDATE = Convert.ToDateTime(item["AIN_DINVOICEDATE"]),
                                   AIN_NTAXES = item["AIN_NTAXES"].ToString(),
                                   AIN_NTOTALVALUE = item["AIN_NTOTALVALUE"].ToString(),
                                   BRE_NCOMMISSIONPERCENT = item["BRE_NCOMMISSIONPERCENT"].ToString(),
                                   CodExtraccion = item["CodExtraccion"].ToString(),
                                   CONTRACT_CODE = item["CONTRACT_CODE"].ToString(),
                                   CTY_CNAME = item["CTY_CNAME"].ToString(),
                                   EXTRACT_DATE = Convert.ToDateTime(item["EXTRACT_DATE"]),
                                   INT_CINTERMEDIARY_NAME = item["INT_CINTERMEDIARY_NAME"].ToString(),
                                   INT_CKEY = item["INT_CKEY"].ToString(),
                                   INT_NCODE = item["INT_NCODE"].ToString(),
                                   ITY_CSHORTNAME = item["ITY_CSHORTNAME"].ToString(),
                                   ITY_NCODE = item["ITY_NCODE"].ToString(),
                                   MVA_NVALUE = item["MVA_NVALUE"].ToString(),
                                   PCO_CFARE_CODE = item["PCO_CFARE_CODE"].ToString(),
                                   PER_CIDENTIFICATIONNUMBER = item["PER_CIDENTIFICATIONNUMBER"].ToString(),
                                   PLA_CNAME = item["PLA_CNAME"].ToString(),
                                   PLA_NCODE = item["PLA_NCODE"].ToString(),
                                   PRO_CNAME = item["PRO_CNAME"].ToString(),
                                   PRO_NCODE = item["PRO_NCODE"].ToString(),
                                   TOTAL_FACT = item["TOTAL_FACT"].ToString(),
                                   VALOR_ASESOR = item["VALOR_ASESOR"].ToString(),
                                   VALOR_USUARIO = item["VALOR_USUARIO"].ToString()
                               };

           return UserFiltrados.ToList();
       }

        #endregion
    }
}
