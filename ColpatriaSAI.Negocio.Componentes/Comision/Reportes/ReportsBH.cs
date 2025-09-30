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
    public class ReportsBH
    {

        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<ReportBHDeferredClass> GetReportBHDeferred(DateTime DateIni = default(DateTime), DateTime DateFin = default(DateTime), int Format = default(int), int ProcessType = default(int), string UserName = default(string))
        {

            List<ReportBHDeferredClass> lResult = new List<ReportBHDeferredClass>();


            DataSet DatasetR = new DataSet();

            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmd = new SqlCommand("ReportsBH", sqlConn);
            cmd.CommandTimeout = 6000;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DateIni", Convert.ToDateTime(DateIni));
            cmd.Parameters.Add("@DateFin", Convert.ToDateTime(DateFin));
            cmd.Parameters.Add("@Format", Convert.ToInt32(Format));
            cmd.Parameters.Add("@ProcessType", Convert.ToInt32(ProcessType));
            cmd.Parameters.Add("@UserName", Convert.ToString(UserName));


            SqlDataAdapter daReport = new SqlDataAdapter(cmd);
            daReport.Fill(DatasetR);
            sqlConn.Close();
            return lResult;
        }  // fin metodo getasesorescomerciales

        public List<ReportBHAccruedClass> GetReportBHAccrued(DateTime DateIni = default(DateTime), DateTime DateFin = default(DateTime), int Format = default(int), int ProcessType = default(int), string UserName = default(string))
        {

            List<ReportBHAccruedClass> lResult = new List<ReportBHAccruedClass>();

            DataSet DatasetR = new DataSet();

            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmd = new SqlCommand("ReportsBH", sqlConn);
            cmd.CommandTimeout = 6000;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DateIni", Convert.ToDateTime(DateIni));
            cmd.Parameters.Add("@DateFin", Convert.ToDateTime(DateFin));
            cmd.Parameters.Add("@Format", Convert.ToInt32(Format));
            cmd.Parameters.Add("@ProcessType", Convert.ToInt32(ProcessType));
            cmd.Parameters.Add("@UserName", Convert.ToString(UserName));


            SqlDataAdapter daReport = new SqlDataAdapter(cmd);
            daReport.Fill(DatasetR);
            sqlConn.Close();
            return lResult;
        }

        public Dictionary<int, string> GetAvaliablePeriodsBH()
        {
            try
            {
                Dictionary<int, string> AvaliablePeriods= new Dictionary<int, string>();
                DataSet DatasetR = new DataSet();

                EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                SqlCommand cmd = new SqlCommand("GETAVALIABLEPERIODSBH", sqlConn);
                cmd.CommandTimeout = 6000;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter daReport = new SqlDataAdapter(cmd);
                daReport.Fill(DatasetR);
                sqlConn.Close();
                int i = 1;
                DateTime date1 = new DateTime();
                if (DatasetR.Tables[0].Rows.Count <= 0)
                {
                    return AvaliablePeriods;
                }

                else
                {
                    foreach (DataRow lRow in DatasetR.Tables[0].Rows)
                    {
                        date1 = lRow["PERIODBH"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(lRow["PERIODBH"]);
                        AvaliablePeriods.Add(i, date1.ToString("yyyy/MM/dd")); 
                        i++;
                    }
                    return AvaliablePeriods;
                }

                return AvaliablePeriods;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
