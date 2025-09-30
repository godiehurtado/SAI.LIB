using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data;

namespace ColpatriaSAI.Negocio.Componentes.Estadisticas
{
    public class EstadisticasRecaudos
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<ReporteValores> TraerValoresRecaudosMesPromedio(int anio, int mes)
        {
            string script = "SELECT Compania.id AS IdCompania, Compania.nombre AS Compania, ISNULL(TABLA2.valorRecaudo,0) AS TotalValorMes, TABLA3.promedioRecaudo AS Promedio FROM(" +
                            " SELECT compania_id, SUM(valorRecaudo) as valorRecaudo " +
                            " FROM Recaudo" +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre = " + mes.ToString() +
                            " GROUP BY compania_id) AS TABLA2" +
                            " RIGHT JOIN (" +
                            " SELECT compania_id, AVG(valorRecaudo) as promedioRecaudo FROM (" +
                            " SELECT compania_id,mesCierre, SUM(valorRecaudo) as valorRecaudo" +
                            " FROM Recaudo" +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre " + (mes == 1 ? "= " : "< ") + mes.ToString() +
                            " GROUP BY compania_id, mesCierre) AS TABLA1" +
                            " GROUP BY compania_id) AS TABLA3 ON TABLA2.compania_id = TABLA3.compania_id" +
                            " INNER JOIN Compania ON TABLA3.compania_id = Compania.id" +
                            " ORDER BY TABLA3.compania_id";
            List<ReporteValores> reporteRecaudo = contexto.ExecuteStoreQuery<ReporteValores>(script).ToList();            

            return reporteRecaudo;
        }

        public List<ReporteRegistros> TraerRegistrosRecaudosMesPromedio(int anio, int mes)
        {
            string script = "SELECT Compania.id AS IdCompania, Compania.nombre AS Compania, ISNULL(TABLA2.Registros,0) AS TotalRegistrosMes, TABLA3.promedioRegistros AS PromedioRegistros FROM(" +
                            " SELECT compania_id, COUNT(1) as Registros " +
                            " FROM Recaudo" +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre = " + mes.ToString() +
                            " GROUP BY compania_id) AS TABLA2" +
                            " RIGHT JOIN (" +
                            " SELECT compania_id, AVG(registrosRecaudo) as promedioRegistros FROM (" +
                            " SELECT compania_id,mesCierre, COUNT(1) as registrosRecaudo" +
                            " FROM Recaudo" +
                            " WHERE anioCierre = " + anio.ToString() +
                            " AND mesCierre " + (mes == 1 ? "= " : "< ") + mes.ToString() +
                            " GROUP BY compania_id, mesCierre) AS TABLA1" +
                            " GROUP BY compania_id) AS TABLA3 ON TABLA2.compania_id = TABLA3.compania_id" +
                            " INNER JOIN Compania ON TABLA3.compania_id = Compania.id" +
                            " ORDER BY TABLA3.compania_id";
            List<ReporteRegistros> reporteRecaudo = contexto.ExecuteStoreQuery<ReporteRegistros>(script).ToList();

            return reporteRecaudo;
        }
    }
}
