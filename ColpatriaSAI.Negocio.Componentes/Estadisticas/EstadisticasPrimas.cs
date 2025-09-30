using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Estadisticas
{
    public class EstadisticasPrimas
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<ReporteValores> TraerValoresPrimasMesPromedio(int anio, int mes)
        {
            string script = "SELECT Compania.id AS IdCompania, Compania.nombre AS Compania, ISNULL(TABLA2.valorPrimaTotal,0) AS TotalValorMes, TABLA3.promedioPrima AS Promedio" +
                            " FROM( SELECT compania_id, SUM(valorPrimaTotal) as valorPrimaTotal FROM Negocio" +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre = " + mes.ToString() +
                            " GROUP BY compania_id) AS TABLA2" +
                            " RIGHT JOIN ( SELECT compania_id, AVG(valorPrimaTotal) as promedioPrima " +
                            " FROM (SELECT compania_id,mesCierre, SUM(valorPrimaTotal) as valorPrimaTotal " +
                            " FROM Negocio " +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre " + (mes == 1 ? "= " : "< ") + mes.ToString() +
                            " GROUP BY compania_id, mesCierre) AS TABLA1 " +
                            " GROUP BY compania_id) AS TABLA3 ON TABLA2.compania_id = TABLA3.compania_id " +
                            " INNER JOIN Compania ON TABLA3.compania_id = Compania.id " +
                            " ORDER BY TABLA3.compania_id";
            List<ReporteValores> reporteRecaudo = contexto.ExecuteStoreQuery<ReporteValores>(script).ToList();

            return reporteRecaudo;
        }

        public List<ReporteRegistros> TraerRegistrosPrimasMesPromedio(int anio, int mes)
        {
            string script = "SELECT Compania.id AS IdCompania, Compania.nombre AS Compania, ISNULL(TABLA2.Registros,0) AS TotalRegistrosMes, TABLA3.promedioRegistros AS PromedioRegistros FROM(" +
                            " SELECT compania_id, COUNT(1) as Registros " +
                            " FROM Negocio" +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre = " + mes.ToString() +
                            " GROUP BY compania_id) AS TABLA2" +
                            " RIGHT JOIN (" +
                            " SELECT compania_id, AVG(registrosNegocio) as promedioRegistros FROM (" +
                            " SELECT compania_id,mesCierre, COUNT(1) as registrosNegocio" +
                            " FROM Negocio" +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre < " + mes.ToString() +
                            " GROUP BY compania_id, mesCierre) AS TABLA1" +
                            " GROUP BY compania_id) AS TABLA3 ON TABLA2.compania_id = TABLA3.compania_id" +
                            " INNER JOIN Compania ON TABLA3.compania_id = Compania.id" +
                            " ORDER BY TABLA3.compania_id";
            List<ReporteRegistros> reporteRecaudo = contexto.ExecuteStoreQuery<ReporteRegistros>(script).ToList();

            return reporteRecaudo;
        }
    }
}
