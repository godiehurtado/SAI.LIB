using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using System.Web.Configuration;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;

namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    public class LiquidacionConcurso
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region LIQUIDACION

        #region NUEVOSMETODOS
        /// <summary>
        /// Realiza la liquidación y generación del reporte de la liquidación de Reglas (premios)
        /// </summary>
        /// <param name="fechaInicio">Determina la fecha inicio del periodo a liquidar (Solo si la regla es periódica)</param>
        /// <param name="fechaFin">Determina la fecha final del periodo a liquidar (Solo si la regla es periódica)</param>
        /// <param name="idRegla">Es el id de la regla a liquidar</param>
        /// <param name="idConcurso">Es el id del concurso relacionado</param>
        /// <returns>Retorna 1 cuando la liquidación fue correcta o 0 en caso contrario</returns>
        public void GenerarLiquidacionRegla_Iniciar(DateTime fechaInicio, DateTime fechaFin, int idRegla, int idConcurso)
        {
            ///Se obtiene la regla de la liquidación actual
            Regla regla = tabla.Reglas.Where(x => x.id == idRegla).FirstOrDefault();
            /// Se busca una liquidación para la regla a liquidar que esté en estado pagado
            LiquidacionRegla liquidacionregla = tabla.LiquidacionReglas.Where(x => x.regla_id == idRegla && x.estado == 2).FirstOrDefault();

            /// En caso de que exista una liquidación con estado pagado para la regla a liquidar, no se realiza una nueva liquidación
            if (liquidacionregla == null)
            {
                ///Se crea la nueva liquidación
                liquidacionregla = new LiquidacionRegla();

                liquidacionregla.estado = 1;
                liquidacionregla.fecha_inicial = (regla.tipoRegla_id == 1 ? fechaInicio : regla.fecha_inicio);
                liquidacionregla.fecha_final = (regla.tipoRegla_id == 1 ? fechaFin : regla.fecha_fin);
                liquidacionregla.fecha_liquidacion = DateTime.Now;
                liquidacionregla.regla_id = idRegla;

                tabla.LiquidacionReglas.AddObject(liquidacionregla);
                tabla.SaveChanges();

                ///Se registra que el proceso de liquidación está en proceso
                using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                {
                    string script_1 = "INSERT INTO ProcesoLiquidacion VALUES (1, " + liquidacionregla.id.ToString() + ", GETDATE(), 3)";

                    conn.Open();

                    SqlCommand command = new SqlCommand(script_1, conn);
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 1000;

                    command.ExecuteNonQuery();

                    command.Connection.Close();
                }

                /// Se obtienen la subreglas de último nivel, que tiene las condiciones a procesar
                List<int> subReglas = ObtenerSubReglasALiquidar(idRegla);

                int result = 1;

                /// Si es tipo de regla periodica, se liquida con las fechas seleccionadas por el usuario, 
                /// sino se liquida con las fechas parametrizadas en las variables
                if (regla.tipoRegla_id == 1)
                {
                    /// Se liquida cada subregla
                    foreach (int idsubregla in subReglas)
                    {
                        result = LiquidarSubregla(idsubregla, idConcurso, liquidacionregla.id, fechaInicio, fechaFin);
                    }
                }
                else
                {
                    /// Se liquida cada subregla
                    foreach (int idsubregla in subReglas)
                    {
                        result = LiquidarSubregla(idsubregla, idConcurso, liquidacionregla.id);
                    }
                }

                if (result == 0)
                {
                    tabla.LiquidacionReglas.DeleteObject(liquidacionregla);
                    ///Se elimina que el proceso de liquidación está en curso
                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                    {
                        string script_1 = "DELETE FROM ProcesoLiquidacion WHERE liquidacion_id = " + liquidacionregla.id.ToString();

                        conn.Open();

                        SqlCommand command = new SqlCommand(script_1, conn);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1000;

                        command.ExecuteNonQuery();

                        command.Connection.Close();
                    }
                }


                /// Se obtine la subregla principal de donde parte el árbol de subreglas para liquidar los premios.
                SubRegla subReglaPrincipal = ObtenerSubreglaPrincipal(idRegla);

                /// Se listan los participantes que están dentro de la liquidación
                List<int> Participantes = ListarParticipantesConcurso(liquidacionregla.id);

                /// Se obtiene el año de la liquidación en curso
                int anio = ((DateTime)tabla.Concursoes.Where(x => x.id == idConcurso).Select(x => x.fecha_inicio).FirstOrDefault()).Year;

                /// Si es tipo de regla periodica, se liquida con las fechas seleccionadas por el usuario, 
                /// sino se liquida con las fechas parametrizadas en las variables
                if (regla.tipoRegla_id == 1)
                {
                    /// Se liquidan los premios partiendo de la subregla principal
                    Participantes = LiquidarPremios(subReglaPrincipal, Participantes, liquidacionregla.id, anio, fechaInicio, fechaFin);
                }
                else
                {
                    /// Se liquidan los premios partiendo de la subregla principal
                    Participantes = LiquidarPremios(subReglaPrincipal, Participantes, liquidacionregla.id, anio);
                }

                //GenerarReporteLiquidacionAsesor(liquidacionregla.id);

                ///Se elimina que el proceso de liquidación está en curso
                using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                {
                    string script_1 = "DELETE FROM ProcesoLiquidacion WHERE liquidacion_id = " + liquidacionregla.id.ToString();

                    conn.Open();

                    SqlCommand command = new SqlCommand(script_1, conn);
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 1000;

                    command.ExecuteNonQuery();

                    command.Connection.Close();
                }
            }
            else
            {
            }
        }

        private List<int> ObtenerSubReglasALiquidar(int idRegla)
        {
            List<int> idsSubreglas = tabla.Condicions.Where(x => x.SubRegla.regla_id == idRegla).Select(x => (int)x.subregla_id).Distinct().ToList();
            return idsSubreglas;
        }

        /// <summary>
        /// Realiza la liquidación de la subregla, de acuerdo a sus condiciones (Para reglas únicas)
        /// </summary>
        /// <param name="idSubRegla">Id de la subregla a liquidar</param>
        /// <param name="idConcurso">Id del concurso relacionado</param>
        /// <param name="idLiquidacionRegla">Id de la liquidación del proceso actual</param>
        /// <returns></returns>
        private int LiquidarSubregla(int idSubRegla, int idConcurso, int idLiquidacionRegla)
        {
            try
            {
                int anio = ((DateTime)tabla.Concursoes.Where(x => x.id == idConcurso).Select(x => x.fecha_inicio).FirstOrDefault()).Year;
                List<Condicion> condiciones = tabla.Condicions.Where(x => x.subregla_id == idSubRegla).ToList();
                List<Condicion> condicionesfiltro = new List<Condicion>();
                bool participante = false;

                foreach (Condicion con1 in condiciones)
                {
                    Variable var1 = tabla.Variables.Where(x => x.id == con1.variable_id).FirstOrDefault();
                    if ((bool)var1.esFiltro)
                    {
                        condicionesfiltro.Add(con1);
                        if (var1.nombre == "Fecha de ingreso del asesor")
                        {
                            participante = true;
                        }
                    }
                }

                foreach (Condicion confil in condicionesfiltro)
                {
                    condiciones.Remove(confil);
                }

                foreach (Condicion condicion in condiciones)
                {
                    string script_ = String.Empty;
                    Variable variable = tabla.Variables.Where(x => x.id == condicion.variable_id).FirstOrDefault();
                    if (variable.tipoDato == "Numero")
                    {
                        script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + condicion.mesinicio.ToString() + " DECLARE @mesfin INT = " + condicion.mesfin.ToString() + " ";
                    }

                    script_ += "SELECT TablaMaestra.clave, ";
                    script_ += variable.columnaTablaMaestra;
                    script_ += " FROM TablaMaestra";

                    int indiceW = script_.IndexOf("WHERE");
                    if (indiceW != -1)
                    {
                        indiceW += 5;
                        string parcial = string.Empty;
                        foreach (Condicion condfil in condicionesfiltro)
                        {
                            Variable variablefil = tabla.Variables.Where(x => x.id == condfil.variable_id).FirstOrDefault();
                            Operador operador = tabla.Operadors.Where(x => x.id == condfil.operador_id).FirstOrDefault();

                            string culumna = variablefil.columnaTablaMaestra.Replace("TablaMaestra.", "");

                            string evaluacion = culumna + " " + operador.expresion.ToString() + " "
                                + (variablefil.tipoDato == "Numero" ? condfil.valor.ToString() :
                                (variablefil.tipoDato == "Seleccion" ? condfil.seleccion.ToString() :
                                (variablefil.tipoDato == "Fecha" ? "'" + condfil.fecha.ToString() + "'" : string.Empty)));

                            parcial += evaluacion + " AND ";
                        }
                        script_ = script_.Insert(indiceW, " " + parcial);
                    }

                    if (participante)
                    {
                        script_ += " INNER JOIN Participante ON TablaMaestra.participante_id = Participante.id";
                    }

                    script_ += " WHERE ";

                    foreach (Condicion condfil in condicionesfiltro)
                    {
                        Variable variablefil = tabla.Variables.Where(x => x.id == condfil.variable_id).FirstOrDefault();
                        Operador operador = tabla.Operadors.Where(x => x.id == condfil.operador_id).FirstOrDefault();

                        string evaluacion = variablefil.columnaTablaMaestra + " " + operador.expresion.ToString() + " "
                            + (variablefil.tipoDato == "Numero" ? condfil.valor.ToString() :
                            (variablefil.tipoDato == "Seleccion" ? condfil.seleccion.ToString() :
                            (variablefil.tipoDato == "Fecha" ? "'" + condfil.fecha.ToString() + "'" : string.Empty)));

                        script_ += evaluacion + " AND ";
                    }

                    if (variable.tipoDato == "Numero")
                    {
                        script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                    }

                    DataTable tablaCondicion = new DataTable();

                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                    {
                        conn.Open();

                        SqlCommand command = new SqlCommand(script_, conn);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1000;

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(tablaCondicion);
                    }

                    if (tablaCondicion.Rows.Count == 0)
                    {
                        return 0;
                    }

                    Operador operador2 = tabla.Operadors.Where(x => x.id == condicion.operador_id).FirstOrDefault();
                    string evaluacion2 = "valor " + operador2.expresion.ToString() + " "
                            + (variable.tipoDato == "Numero" ? condicion.valor.ToString() :
                            (variable.tipoDato == "Seleccion" ? condicion.seleccion.ToString() :
                            (variable.tipoDato == "Fecha" ? "'" + condicion.fecha.ToString() + "'" : string.Empty)));

                    List<DataRow> temporalcondiciones = tablaCondicion.Select(evaluacion2).ToList();

                    List<string> clavespro = new List<string>();

                    foreach (DataRow row in temporalcondiciones)
                    {
                        LiquidacionReglaDetalleCondicion detalle = new LiquidacionReglaDetalleCondicion();
                        detalle.liquidacionRegla_id = idLiquidacionRegla;
                        detalle.condicion_id = condicion.id;
                        string clave_ = row[0].ToString();
                        clavespro.Add(clave_);
                        detalle.participante_id = tabla.Participantes.Where(x => x.clave == clave_).First().id;
                        detalle.valor = row[1].ToString();
                        detalle.cumple = true;
                        tabla.LiquidacionReglaDetalleCondicions.AddObject(detalle);
                        tablaCondicion.Rows.Remove(row);
                    }
                    tabla.SaveChanges();

                    foreach (DataRow row2 in tablaCondicion.Rows)
                    {
                        LiquidacionReglaDetalleCondicion detalle = new LiquidacionReglaDetalleCondicion();
                        detalle.liquidacionRegla_id = idLiquidacionRegla;
                        detalle.condicion_id = condicion.id;
                        string clave_ = row2[0].ToString();
                        detalle.participante_id = tabla.Participantes.Where(x => x.clave == clave_).First().id;
                        detalle.valor = row2[1].ToString();
                        detalle.cumple = false;
                        tabla.LiquidacionReglaDetalleCondicions.AddObject(detalle);
                    }

                    tabla.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Realiza la liquidación de la subregla, de acuerdo a sus condiciones (Para reglas periódicas)
        /// </summary>
        /// <param name="idSubRegla">Id de la subregla a liquidar</param>
        /// <param name="idConcurso">Id del concurso relacionado</param>
        /// <param name="idLiquidacionRegla">Id de la liquidación del proceso actual</param>
        /// <param name="fechaInicio">Fecha inicio del periodo a liquidar</param>
        /// <param name="fechaFin">Fecha fin del periodo a liquidar</param>
        /// <returns></returns>
        private int LiquidarSubregla(int idSubRegla, int idConcurso, int idLiquidacionRegla, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                int anio = ((DateTime)tabla.Concursoes.Where(x => x.id == idConcurso).Select(x => x.fecha_inicio).FirstOrDefault()).Year;
                List<Condicion> condiciones = tabla.Condicions.Where(x => x.subregla_id == idSubRegla).ToList();
                List<Condicion> condicionesfiltro = new List<Condicion>();
                bool participante = false;

                foreach (Condicion con1 in condiciones)
                {
                    Variable var1 = tabla.Variables.Where(x => x.id == con1.variable_id).FirstOrDefault();
                    if ((bool)var1.esFiltro)
                    {
                        condicionesfiltro.Add(con1);
                        if (var1.nombre == "Fecha de ingreso del asesor")
                        {
                            participante = true;
                        }
                    }
                }

                foreach (Condicion confil in condicionesfiltro)
                {
                    condiciones.Remove(confil);
                }

                foreach (Condicion condicion in condiciones)
                {
                    string script_ = String.Empty;
                    Variable variable = tabla.Variables.Where(x => x.id == condicion.variable_id).FirstOrDefault();
                    if (variable.tipoDato == "Numero")
                    {
                        script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + fechaInicio.Month.ToString() + " DECLARE @mesfin INT = " + fechaFin.Month.ToString() + " ";
                    }

                    script_ += "SELECT TablaMaestra.clave, ";
                    script_ += variable.columnaTablaMaestra;
                    script_ += " FROM TablaMaestra";

                    int indiceW = script_.IndexOf("WHERE");
                    if (indiceW != -1)
                    {
                        indiceW += 5;
                        string parcial = string.Empty;
                        foreach (Condicion condfil in condicionesfiltro)
                        {
                            Variable variablefil = tabla.Variables.Where(x => x.id == condfil.variable_id).FirstOrDefault();
                            Operador operador = tabla.Operadors.Where(x => x.id == condfil.operador_id).FirstOrDefault();

                            string culumna = variablefil.columnaTablaMaestra.Replace("TablaMaestra.", "");

                            string evaluacion = culumna + " " + operador.expresion.ToString() + " "
                                + (variablefil.tipoDato == "Numero" ? condfil.valor.ToString() :
                                (variablefil.tipoDato == "Seleccion" ? condfil.seleccion.ToString() :
                                (variablefil.tipoDato == "Fecha" ? "'" + condfil.fecha.ToString() + "'" : string.Empty)));

                            parcial += evaluacion + " AND ";
                        }
                        script_ = script_.Insert(indiceW, " " + parcial);
                    }

                    if (participante)
                    {
                        script_ += " INNER JOIN Participante ON TablaMaestra.participante_id = Participante.id";
                    }

                    script_ += " WHERE ";

                    foreach (Condicion condfil in condicionesfiltro)
                    {
                        Variable variablefil = tabla.Variables.Where(x => x.id == condfil.variable_id).FirstOrDefault();
                        Operador operador = tabla.Operadors.Where(x => x.id == condfil.operador_id).FirstOrDefault();

                        string evaluacion = variablefil.columnaTablaMaestra + " " + operador.expresion.ToString() + " "
                            + (variablefil.tipoDato == "Numero" ? condfil.valor.ToString() :
                            (variablefil.tipoDato == "Seleccion" ? condfil.seleccion.ToString() :
                            (variablefil.tipoDato == "Fecha" ? "'" + condfil.fecha.ToString() + "'" : string.Empty)));

                        script_ += evaluacion + " AND ";
                    }

                    if (variable.tipoDato == "Numero")
                    {
                        script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                    }

                    DataTable tablaCondicion = new DataTable();

                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                    {
                        conn.Open();

                        SqlCommand command = new SqlCommand(script_, conn);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1000;

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(tablaCondicion);
                    }

                    if (tablaCondicion.Rows.Count == 0)
                    {
                        return 0;
                    }

                    Operador operador2 = tabla.Operadors.Where(x => x.id == condicion.operador_id).FirstOrDefault();
                    string evaluacion2 = "valor " + operador2.expresion.ToString() + " "
                            + (variable.tipoDato == "Numero" ? condicion.valor.ToString() :
                            (variable.tipoDato == "Seleccion" ? condicion.seleccion.ToString() :
                            (variable.tipoDato == "Fecha" ? "'" + condicion.fecha.ToString() + "'" : string.Empty)));

                    List<DataRow> temporalcondiciones = tablaCondicion.Select(evaluacion2).ToList();

                    List<string> clavespro = new List<string>();

                    foreach (DataRow row in temporalcondiciones)
                    {
                        LiquidacionReglaDetalleCondicion detalle = new LiquidacionReglaDetalleCondicion();
                        detalle.liquidacionRegla_id = idLiquidacionRegla;
                        detalle.condicion_id = condicion.id;
                        string clave_ = row[0].ToString();
                        clavespro.Add(clave_);
                        detalle.participante_id = tabla.Participantes.Where(x => x.clave == clave_).First().id;
                        detalle.valor = row[1].ToString();
                        detalle.cumple = true;
                        tabla.LiquidacionReglaDetalleCondicions.AddObject(detalle);
                        tablaCondicion.Rows.Remove(row);
                    }
                    tabla.SaveChanges();

                    foreach (DataRow row2 in tablaCondicion.Rows)
                    {
                        LiquidacionReglaDetalleCondicion detalle = new LiquidacionReglaDetalleCondicion();
                        detalle.liquidacionRegla_id = idLiquidacionRegla;
                        detalle.condicion_id = condicion.id;
                        string clave_ = row2[0].ToString();
                        detalle.participante_id = tabla.Participantes.Where(x => x.clave == clave_).First().id;
                        detalle.valor = row2[1].ToString();
                        detalle.cumple = false;
                        tabla.LiquidacionReglaDetalleCondicions.AddObject(detalle);
                    }
                    tabla.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private SubRegla ObtenerSubreglaPrincipal(int idRegla)
        {
            SubRegla subregla = tabla.SubReglas.Where(x => x.regla_id == idRegla && x.principal == true).FirstOrDefault();
            return subregla;
        }

        /// <summary>
        /// Realiza la liquidación de premios, este es un método recursivo de acuerdo a los nieveles de subreglas que tenga la regla
        /// (Para reglas únicas)
        /// </summary>
        /// <param name="subregla">SubRegla actual a liquidar</param>
        /// <param name="idsParticipantes">Lista de los ids de los participantes que van cumpliendo las condiciones</param>
        /// <param name="idLiquidacionRegla">Id de la liquidación actual</param>
        /// <param name="anio">año de la liquidación actual</param>
        /// <returns>Retorna la lista de los ids de los participantes que cumplieron las condiciones</returns>
        private List<int> LiquidarPremios(SubRegla subregla, List<int> idsParticipantes, int idLiquidacionRegla, int anio)
        {
            SAI_Entities tabla2 = new SAI_Entities();
            if (subregla.condicionAgrupada_id != null)
            {
                CondicionAgrupada condicionAgr = tabla2.CondicionAgrupadas.Where(x => x.id == subregla.condicionAgrupada_id).FirstOrDefault();
                SubRegla sub1 = tabla2.SubReglas.Where(x => x.id == condicionAgr.subregla_id1).FirstOrDefault();
                SubRegla sub2 = tabla2.SubReglas.Where(x => x.id == condicionAgr.subregla_id2).FirstOrDefault();
                List<int> cum1 = LiquidarPremios(sub1, idsParticipantes, idLiquidacionRegla, anio);
                List<int> cum2 = LiquidarPremios(sub2, idsParticipantes, idLiquidacionRegla, anio);

                switch (condicionAgr.operador_id)
                {
                    case 11:
                        idsParticipantes = cum1.Intersect(cum2).ToList();

                        List<PremioxSubregla> premiossub = tabla2.PremioxSubreglas.Where(x => x.subregla_id == subregla.id).ToList();
                        foreach (PremioxSubregla premiosub in premiossub)
                        {
                            string script_ = string.Empty;
                            Premio premioV = tabla2.Premios.Where(x => x.id == premiosub.premio_id).FirstOrDefault();
                            Variable variable = tabla2.Variables.Where(x => x.id == premioV.variable_id).FirstOrDefault();
                            if (variable.tipoDato == "Numero")
                            {
                                script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + premiosub.mesinicio.ToString() + " DECLARE @mesfin INT = " + premiosub.mesfin.ToString() + " ";
                            }

                            script_ += "SELECT TablaMaestra.clave, ";
                            script_ += variable.columnaTablaMaestra;
                            script_ += " FROM TablaMaestra WHERE ";

                            List<CondicionxPremioSubregla> condfils = tabla2.CondicionxPremioSubreglas.Where(x => x.premioxsubregla_id == premiosub.id).ToList();
                            foreach (CondicionxPremioSubregla condfil in condfils)
                            {
                                Variable variablefil = tabla2.Variables.Where(x => x.id == condfil.variable_id).FirstOrDefault();
                                Operador operador = tabla2.Operadors.Where(x => x.id == condfil.operador_id).FirstOrDefault();

                                string evaluacion = variablefil.columnaTablaMaestra + " " + operador.expresion.ToString() + " "
                                    + (variablefil.tipoDato == "Numero" ? condfil.valor.ToString() :
                                    (variablefil.tipoDato == "Seleccion" ? condfil.seleccion.ToString() :
                                    (variablefil.tipoDato == "Fecha" ? "'" + condfil.fecha.ToString() + "'" : string.Empty)));

                                script_ += evaluacion + " AND ";
                            }

                            if (variable.tipoDato == "Numero")
                            {
                                script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                            }

                            DataTable tablaCondicion = new DataTable();

                            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                            {
                                conn.Open();

                                SqlCommand command = new SqlCommand(script_, conn);
                                command.CommandType = CommandType.Text;
                                command.CommandTimeout = 1000;

                                SqlDataAdapter adapter = new SqlDataAdapter();
                                adapter.SelectCommand = command;
                                adapter.Fill(tablaCondicion);
                            }

                            foreach (int idParticipante in idsParticipantes)
                            {
                                Participante parti = tabla2.Participantes.Where(x => x.id == idParticipante).FirstOrDefault();
                                string evaluacion2 = "clave = '" + parti.clave + "'";
                                DataRow temporalcondiciones = tablaCondicion.Select(evaluacion2).FirstOrDefault();
                                LiquidacionReglaPremio premio = new LiquidacionReglaPremio();
                                premio.liquidacionrelga_id = idLiquidacionRegla;
                                premio.participante_id = idParticipante;
                                premio.premio_id = (int)premiosub.premio_id;
                                premio.valorPremio = double.Parse(temporalcondiciones["valor"].ToString());
                                tabla2.LiquidacionReglaPremios.AddObject(premio);
                            }
                        }
                        tabla2.SaveChanges();
                        return idsParticipantes;
                    case 12:
                        idsParticipantes = cum1.Union(cum2).ToList();

                        List<PremioxSubregla> premiossub2 = tabla2.PremioxSubreglas.Where(x => x.subregla_id == subregla.id).ToList();
                        foreach (PremioxSubregla premiosub in premiossub2)
                        {
                            string script_ = string.Empty;
                            Premio premioV = tabla2.Premios.Where(x => x.id == premiosub.premio_id).FirstOrDefault();
                            Variable variable = tabla2.Variables.Where(x => x.id == premioV.variable_id).FirstOrDefault();
                            if (variable.tipoDato == "Numero")
                            {
                                script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + premiosub.mesinicio.ToString() + " DECLARE @mesfin INT = " + premiosub.mesfin.ToString() + " ";
                            }

                            script_ += "SELECT TablaMaestra.clave, ";
                            script_ += variable.columnaTablaMaestra;
                            script_ += " FROM TablaMaestra WHERE ";

                            if (variable.tipoDato == "Numero")
                            {
                                script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                            }

                            DataTable tablaCondicion = new DataTable();

                            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                            {
                                conn.Open();

                                SqlCommand command = new SqlCommand(script_, conn);
                                command.CommandType = CommandType.Text;
                                command.CommandTimeout = 1000;

                                SqlDataAdapter adapter = new SqlDataAdapter();
                                adapter.SelectCommand = command;
                                adapter.Fill(tablaCondicion);
                            }

                            foreach (int idParticipante in idsParticipantes)
                            {
                                Participante parti = tabla2.Participantes.Where(x => x.id == idParticipante).FirstOrDefault();
                                string evaluacion2 = "clave = '" + parti.clave + "'";
                                DataRow temporalcondiciones = tablaCondicion.Select(evaluacion2).FirstOrDefault();
                                LiquidacionReglaPremio premio = new LiquidacionReglaPremio();
                                premio.liquidacionrelga_id = idLiquidacionRegla;
                                premio.participante_id = idParticipante;
                                premio.premio_id = (int)premiosub.premio_id;
                                premio.valorPremio = double.Parse(temporalcondiciones["valor"].ToString());
                                tabla2.LiquidacionReglaPremios.AddObject(premio);
                            }
                        }
                        tabla2.SaveChanges();
                        return idsParticipantes;
                    default:
                        return idsParticipantes;
                }
            }
            else
            {
                List<Condicion> condiciones = tabla2.Condicions.Where(x => x.subregla_id == subregla.id).ToList();
                foreach (Condicion cond in condiciones)
                {
                    Variable vari = tabla2.Variables.Where(x => x.id == cond.variable_id).FirstOrDefault();
                    if (!(bool)vari.esFiltro)
                    {
                        List<LiquidacionReglaDetalleCondicion> detalles = tabla2.LiquidacionReglaDetalleCondicions.Where(x => x.condicion_id == cond.id && x.liquidacionRegla_id == idLiquidacionRegla && x.cumple == true).ToList();
                        List<int> participantescumplen = detalles.FindAll(x => idsParticipantes.Contains(x.participante_id)).Select(x => x.participante_id).ToList();

                        idsParticipantes = idsParticipantes.Intersect(participantescumplen).ToList();
                    }
                }

                List<PremioxSubregla> premiossub = tabla2.PremioxSubreglas.Where(x => x.subregla_id == subregla.id).ToList();
                foreach (PremioxSubregla premiosub in premiossub)
                {
                    string script_ = string.Empty;
                    Premio premioV = tabla2.Premios.Where(x => x.id == premiosub.premio_id).FirstOrDefault();
                    Variable variable = tabla2.Variables.Where(x => x.id == premioV.variable_id).FirstOrDefault();
                    if (variable.tipoDato == "Numero")
                    {
                        script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + premiosub.mesinicio.ToString() + " DECLARE @mesfin INT = " + premiosub.mesfin.ToString() + " ";
                    }

                    script_ += "SELECT TablaMaestra.clave, ";
                    script_ += variable.columnaTablaMaestra;
                    script_ += " FROM TablaMaestra WHERE ";

                    if (variable.tipoDato == "Numero")
                    {
                        script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                    }

                    DataTable tablaCondicion = new DataTable();

                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                    {
                        conn.Open();

                        SqlCommand command = new SqlCommand(script_, conn);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1000;

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(tablaCondicion);
                    }

                    foreach (int idParticipante in idsParticipantes)
                    {
                        Participante parti = tabla2.Participantes.Where(x => x.id == idParticipante).FirstOrDefault();
                        string evaluacion2 = "clave = '" + parti.clave + "'";
                        DataRow temporalcondiciones = tablaCondicion.Select(evaluacion2).FirstOrDefault();
                        LiquidacionReglaPremio premio = new LiquidacionReglaPremio();
                        premio.liquidacionrelga_id = idLiquidacionRegla;
                        premio.participante_id = idParticipante;
                        premio.premio_id = (int)premiosub.premio_id;
                        premio.valorPremio = double.Parse(temporalcondiciones["valor"].ToString());
                        tabla2.LiquidacionReglaPremios.AddObject(premio);
                    }
                }
                tabla2.SaveChanges();

                return idsParticipantes;
            }
        }

        /// <summary>
        /// Realiza la liquidación de premios, este es un método recursivo de acuerdo a los nieveles de subreglas que tenga la regla
        /// (Para reglas periódicas)
        /// </summary>
        /// <param name="subregla">SubRegla actual a liquidar</param>
        /// <param name="idsParticipantes">Lista de los ids de los participantes que van cumpliendo las condiciones</param>
        /// <param name="idLiquidacionRegla">Id de la liquidación actual</param>
        /// <param name="anio">año de la liquidación actual</param>
        /// <param name="fechaInicio">Fecha inicio del perido a liquidar</param>
        /// <param name="fechaFin">Fecha fin del periodo a liquidar</param>
        /// <returns>Retorna la lista de los ids de los participantes que cumplieron las condiciones</returns>
        private List<int> LiquidarPremios(SubRegla subregla, List<int> idsParticipantes, int idLiquidacionRegla, int anio, DateTime fechaInicio, DateTime fechaFin)
        {
            SAI_Entities tabla2 = new SAI_Entities();
            if (subregla.condicionAgrupada_id != null)
            {
                CondicionAgrupada condicionAgr = tabla2.CondicionAgrupadas.Where(x => x.id == subregla.condicionAgrupada_id).FirstOrDefault();
                SubRegla sub1 = tabla2.SubReglas.Where(x => x.id == condicionAgr.subregla_id1).FirstOrDefault();
                SubRegla sub2 = tabla2.SubReglas.Where(x => x.id == condicionAgr.subregla_id2).FirstOrDefault();
                List<int> cum1 = LiquidarPremios(sub1, idsParticipantes, idLiquidacionRegla, anio, fechaInicio, fechaFin);
                List<int> cum2 = LiquidarPremios(sub2, idsParticipantes, idLiquidacionRegla, anio, fechaInicio, fechaFin);

                switch (condicionAgr.operador_id)
                {
                    case 11:
                        idsParticipantes = cum1.Intersect(cum2).ToList();

                        List<PremioxSubregla> premiossub = tabla2.PremioxSubreglas.Where(x => x.subregla_id == subregla.id).ToList();
                        foreach (PremioxSubregla premiosub in premiossub)
                        {
                            string script_ = string.Empty;
                            Premio premioV = tabla2.Premios.Where(x => x.id == premiosub.premio_id).FirstOrDefault();
                            Variable variable = tabla2.Variables.Where(x => x.id == premioV.variable_id).FirstOrDefault();
                            if (variable.tipoDato == "Numero")
                            {
                                script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + fechaInicio.Month.ToString() + " DECLARE @mesfin INT = " + fechaFin.Month.ToString() + " ";
                            }

                            script_ += "SELECT TablaMaestra.clave, ";
                            script_ += variable.columnaTablaMaestra;
                            script_ += " FROM TablaMaestra WHERE ";

                            List<CondicionxPremioSubregla> condfils = tabla2.CondicionxPremioSubreglas.Where(x => x.premioxsubregla_id == premiosub.id).ToList();
                            foreach (CondicionxPremioSubregla condfil in condfils)
                            {
                                Variable variablefil = tabla2.Variables.Where(x => x.id == condfil.variable_id).FirstOrDefault();
                                Operador operador = tabla2.Operadors.Where(x => x.id == condfil.operador_id).FirstOrDefault();

                                string evaluacion = variablefil.columnaTablaMaestra + " " + operador.expresion.ToString() + " "
                                    + (variablefil.tipoDato == "Numero" ? condfil.valor.ToString() :
                                    (variablefil.tipoDato == "Seleccion" ? condfil.seleccion.ToString() :
                                    (variablefil.tipoDato == "Fecha" ? "'" + condfil.fecha.ToString() + "'" : string.Empty)));

                                script_ += evaluacion + " AND ";
                            }

                            if (variable.tipoDato == "Numero")
                            {
                                script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                            }

                            DataTable tablaCondicion = new DataTable();

                            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                            {
                                conn.Open();

                                SqlCommand command = new SqlCommand(script_, conn);
                                command.CommandType = CommandType.Text;
                                command.CommandTimeout = 1000;

                                SqlDataAdapter adapter = new SqlDataAdapter();
                                adapter.SelectCommand = command;
                                adapter.Fill(tablaCondicion);
                            }

                            foreach (int idParticipante in idsParticipantes)
                            {
                                Participante parti = tabla2.Participantes.Where(x => x.id == idParticipante).FirstOrDefault();
                                string evaluacion2 = "clave = '" + parti.clave + "'";
                                DataRow temporalcondiciones = tablaCondicion.Select(evaluacion2).FirstOrDefault();
                                LiquidacionReglaPremio premio = new LiquidacionReglaPremio();
                                premio.liquidacionrelga_id = idLiquidacionRegla;
                                premio.participante_id = idParticipante;
                                premio.premio_id = (int)premiosub.premio_id;
                                premio.valorPremio = double.Parse(temporalcondiciones["valor"].ToString());
                                tabla2.LiquidacionReglaPremios.AddObject(premio);
                            }
                        }
                        tabla2.SaveChanges();
                        return idsParticipantes;
                    case 12:
                        idsParticipantes = cum1.Union(cum2).ToList();

                        List<PremioxSubregla> premiossub2 = tabla2.PremioxSubreglas.Where(x => x.subregla_id == subregla.id).ToList();
                        foreach (PremioxSubregla premiosub in premiossub2)
                        {
                            string script_ = string.Empty;
                            Premio premioV = tabla2.Premios.Where(x => x.id == premiosub.premio_id).FirstOrDefault();
                            Variable variable = tabla2.Variables.Where(x => x.id == premioV.variable_id).FirstOrDefault();
                            if (variable.tipoDato == "Numero")
                            {
                                script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + fechaInicio.Month.ToString() + " DECLARE @mesfin INT = " + fechaFin.Month.ToString() + " ";
                            }

                            script_ += "SELECT TablaMaestra.clave, ";
                            script_ += variable.columnaTablaMaestra;
                            script_ += " FROM TablaMaestra WHERE ";

                            if (variable.tipoDato == "Numero")
                            {
                                script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                            }

                            DataTable tablaCondicion = new DataTable();

                            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                            {
                                conn.Open();

                                SqlCommand command = new SqlCommand(script_, conn);
                                command.CommandType = CommandType.Text;
                                command.CommandTimeout = 1000;

                                SqlDataAdapter adapter = new SqlDataAdapter();
                                adapter.SelectCommand = command;
                                adapter.Fill(tablaCondicion);
                            }

                            foreach (int idParticipante in idsParticipantes)
                            {
                                Participante parti = tabla2.Participantes.Where(x => x.id == idParticipante).FirstOrDefault();
                                string evaluacion2 = "clave = '" + parti.clave + "'";
                                DataRow temporalcondiciones = tablaCondicion.Select(evaluacion2).FirstOrDefault();
                                LiquidacionReglaPremio premio = new LiquidacionReglaPremio();
                                premio.liquidacionrelga_id = idLiquidacionRegla;
                                premio.participante_id = idParticipante;
                                premio.premio_id = (int)premiosub.premio_id;
                                premio.valorPremio = double.Parse(temporalcondiciones["valor"].ToString());
                                tabla2.LiquidacionReglaPremios.AddObject(premio);
                            }
                        }
                        tabla2.SaveChanges();
                        return idsParticipantes;
                    default:
                        return idsParticipantes;
                }
            }
            else
            {
                List<Condicion> condiciones = tabla2.Condicions.Where(x => x.subregla_id == subregla.id).ToList();
                foreach (Condicion cond in condiciones)
                {
                    Variable vari = tabla2.Variables.Where(x => x.id == cond.variable_id).FirstOrDefault();
                    if (!(bool)vari.esFiltro)
                    {
                        List<LiquidacionReglaDetalleCondicion> detalles = tabla2.LiquidacionReglaDetalleCondicions.Where(x => x.condicion_id == cond.id && x.liquidacionRegla_id == idLiquidacionRegla && x.cumple == true).ToList();
                        List<int> participantescumplen = detalles.FindAll(x => idsParticipantes.Contains(x.participante_id)).Select(x => x.participante_id).ToList();

                        idsParticipantes = idsParticipantes.Intersect(participantescumplen).ToList();
                    }
                }

                List<PremioxSubregla> premiossub = tabla2.PremioxSubreglas.Where(x => x.subregla_id == subregla.id).ToList();
                foreach (PremioxSubregla premiosub in premiossub)
                {
                    string script_ = string.Empty;
                    Premio premioV = tabla2.Premios.Where(x => x.id == premiosub.premio_id).FirstOrDefault();
                    Variable variable = tabla2.Variables.Where(x => x.id == premioV.variable_id).FirstOrDefault();
                    if (variable.tipoDato == "Numero")
                    {
                        script_ = script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + fechaInicio.Month.ToString() + " DECLARE @mesfin INT = " + fechaFin.Month.ToString() + " ";
                    }

                    script_ += "SELECT TablaMaestra.clave, ";
                    script_ += variable.columnaTablaMaestra;
                    script_ += " FROM TablaMaestra WHERE ";

                    if (variable.tipoDato == "Numero")
                    {
                        script_ += "anioCierre = @anio AND mesCierre between @mesinicio AND @mesfin GROUP BY TablaMaestra.clave ORDER BY TablaMaestra.clave";
                    }

                    DataTable tablaCondicion = new DataTable();

                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                    {
                        conn.Open();

                        SqlCommand command = new SqlCommand(script_, conn);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1000;

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(tablaCondicion);
                    }

                    foreach (int idParticipante in idsParticipantes)
                    {
                        Participante parti = tabla2.Participantes.Where(x => x.id == idParticipante).FirstOrDefault();
                        string evaluacion2 = "clave = '" + parti.clave + "'";
                        DataRow temporalcondiciones = tablaCondicion.Select(evaluacion2).FirstOrDefault();
                        LiquidacionReglaPremio premio = new LiquidacionReglaPremio();
                        premio.liquidacionrelga_id = idLiquidacionRegla;
                        premio.participante_id = idParticipante;
                        premio.premio_id = (int)premiosub.premio_id;
                        premio.valorPremio = double.Parse(temporalcondiciones["valor"].ToString());
                        tabla2.LiquidacionReglaPremios.AddObject(premio);
                    }

                    //if(idsParticipantes.Count > 0)
                    //    tabla.SaveChanges();
                }

                tabla2.SaveChanges();

                return idsParticipantes;
            }
        }

        private List<int> ListarParticipantesConcurso(int idLiquidacionRegla)
        {
            return tabla.LiquidacionReglaDetalleCondicions.Where(x => x.liquidacionRegla_id == idLiquidacionRegla).Select(x => x.participante_id).Distinct().ToList();
        }

        public DataTable GenerarReporteLiquidacionAsesor(int idLiquidacionRegla)
        {
            LiquidacionRegla liquidacionRegla = tabla.LiquidacionReglas.Where(x => x.id == idLiquidacionRegla).FirstOrDefault();

            ///Se registra que el proceso de liquidación está en proceso
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
            {
                string script_1 = "INSERT INTO ProcesoLiquidacion VALUES (8, " + liquidacionRegla.id.ToString() + ", GETDATE(), 20)";

                conn.Open();

                SqlCommand command = new SqlCommand(script_1, conn);
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 1000;

                command.ExecuteNonQuery();

                command.Connection.Close();
            }

            Regla regla = tabla.Reglas.Where(x => x.id == liquidacionRegla.regla_id).FirstOrDefault();
            int anio = ((DateTime)tabla.Concursoes.Where(x => x.id == regla.concurso_id).FirstOrDefault().fecha_inicio).Year;
            int mesinicio = ((DateTime)regla.fecha_inicio).Month;
            int mesfin = ((DateTime)regla.fecha_fin).Month;

            List<int> idcondiciones = (from ld in tabla.LiquidacionReglaDetalleCondicions
                                       where ld.liquidacionRegla_id == liquidacionRegla.id
                                       select ld.condicion_id).Distinct().ToList();
            List<int> idsParticipantes = ListarParticipantesConcurso(idLiquidacionRegla);

            string script_ = String.Empty;

            script_ = "DECLARE @anio INT = " + anio.ToString() + " DECLARE @mesinicio INT = " + mesinicio.ToString() + " DECLARE @mesfin INT = " + mesfin.ToString() + " ";

            script_ += "SELECT z.nombre as ZONA, l.nombre as LOCALIDAD, pd.nombre + ' ' + pd.apellidos as DIRECTOR, p.clave AS CLAVE"
            + ", p.fechaIngreso AS FECHA_INGRESO, p.nombre + ' ' + p.apellidos AS NOMBRE_ASESOR, cc.nombre AS CATEGORIA"
            + ", c.nombre AS COMPANIA, r.nombre AS RAMO, ";

            Dictionary<int, int> idsCondiciones = new Dictionary<int, int>();
            List<int> idsVariables = new List<int>();

            foreach (int idcon in idcondiciones)
            {
                Condicion condicion = tabla.Condicions.Where(x => x.id == idcon).FirstOrDefault();

                Variable variable = tabla.Variables.Where(x => x.id == condicion.variable_id).FirstOrDefault();
                if (!idsVariables.Contains((int)variable.variabledetalle_id))
                {
                    Variable variable2 = tabla.Variables.Where(x => x.id == variable.variabledetalle_id).FirstOrDefault();

                    string nombretabla = "t" + variable2.id.ToString();
                    script_ += nombretabla + ".[1] AS '" + variable2.nombre + "_ENE', "
                    + nombretabla + ".[2] AS '" + variable2.nombre + "_FEB', "
                    + nombretabla + ".[3] AS '" + variable2.nombre + "_MAR', "
                    + nombretabla + ".[4] AS '" + variable2.nombre + "_ABR', "
                    + nombretabla + ".[5] AS '" + variable2.nombre + "_MAY', "
                    + nombretabla + ".[6] AS '" + variable2.nombre + "_JUN', "
                    + nombretabla + ".[7] AS '" + variable2.nombre + "_JUL', "
                    + nombretabla + ".[8] AS '" + variable2.nombre + "_AGO', "
                    + nombretabla + ".[9] AS '" + variable2.nombre + "_SEP', "
                    + nombretabla + ".[10] AS '" + variable2.nombre + "_OCT', "
                    + nombretabla + ".[11] AS '" + variable2.nombre + "_NOV', "
                    + nombretabla + ".[12] AS '" + variable2.nombre + "_DIC', "
                    + "'' AS '" + variable2.nombre + "_TOTAL', ";
                    idsVariables.Add(variable2.id);
                    idsCondiciones.Add(variable2.id, idcon);
                }
            }

            script_ = script_.Substring(0, script_.Length - 2);
            script_ += " FROM ";
            int cuenta = 1;
            string variableant = string.Empty;

            foreach (int idvar in idsVariables)
            {
                int idCondicion = idsCondiciones[idvar];
                Condicion condicionval = tabla.Condicions.Where(x => x.id == idCondicion).FirstOrDefault();
                List<Condicion> condiciones = tabla.Condicions.Where(x => x.subregla_id == condicionval.subregla_id).ToList();
                List<Condicion> condicionesfiltro = new List<Condicion>();

                foreach (Condicion con1 in condiciones)
                {
                    Variable var1 = tabla.Variables.Where(x => x.id == con1.variable_id).FirstOrDefault();
                    if ((bool)var1.esFiltro)
                    {
                        condicionesfiltro.Add(con1);
                    }
                }

                Variable variable = tabla.Variables.Where(x => x.id == idvar).FirstOrDefault();

                string columnamaestra = variable.columnaTablaMaestra;

                int indiceW = columnamaestra.IndexOf("WHERE");
                string parcial = string.Empty;
                if (indiceW != -1)
                {
                    indiceW += 5;
                    foreach (Condicion condfil in condicionesfiltro)
                    {
                        Variable variablefil = tabla.Variables.Where(x => x.id == condfil.variable_id).FirstOrDefault();
                        Operador operador = tabla.Operadors.Where(x => x.id == condfil.operador_id).FirstOrDefault();

                        string culumna = variablefil.columnaTablaMaestra.Replace("TablaMaestra.", "");

                        string evaluacion = culumna + " " + operador.expresion.ToString() + " "
                            + (variablefil.tipoDato == "Numero" ? condfil.valor.ToString() :
                            (variablefil.tipoDato == "Seleccion" ? condfil.seleccion.ToString() :
                            (variablefil.tipoDato == "Fecha" ? "'" + condfil.fecha.ToString() + "'" : string.Empty)));

                        parcial += evaluacion + " AND ";
                    }
                    columnamaestra = columnamaestra.Insert(indiceW, " " + parcial);
                }

                script_ += "(SELECT clave, compania_id, ramo_id,[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12] "
                + "FROM(SELECT clave, compania_id, ramo_id, mesCierre, " + columnamaestra
                + " FROM TablaMaestra WHERE " + parcial + "anioCierre = " + anio.ToString() + "GROUP BY clave, compania_id, ramo_id, mesCierre) AS TABLA1 "
                + "PIVOT( AVG(valor) FOR mesCierre IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) AS PivotTable) AS "
                + "t" + variable.id.ToString();

                if (cuenta > 1)
                {
                    string nomt1 = "t" + variableant;
                    string nomt2 = "t" + variable.id.ToString();
                    script_ += " ON " + nomt1 + ".clave = " + nomt2 + ".clave AND " + nomt1 + ".compania_id = "
                        + nomt2 + ".compania_id AND " + nomt1 + ".ramo_id = " + nomt2 + ".ramo_id";
                }

                if (idsVariables.Count > cuenta)
                {
                    script_ += " INNER JOIN ";
                    variableant = variable.id.ToString();
                }

                cuenta++;
            }
            string nombret = "t" + idsVariables[0].ToString();

            script_ += " INNER JOIN Ramo r ON " + nombret + ".ramo_id = r.id"
                + " INNER JOIN Compania c ON r.compania_id = c.id"
                + " INNER JOIN Participante p  ON " + nombret + ".clave = p.clave"
                + " INNER JOIN Localidad l ON p.localidad_id = l.id"
                + " INNER JOIN Zona z ON l.zona_id = z.id"
                + " INNER JOIN JerarquiaDetalle jd ON p.id = jd.participante_id"
                + " INNER JOIN JerarquiaDetalle jd2 ON jd.padre_id = jd2.id"
                + " INNER JOIN Participante pd ON jd2.participante_id = pd.id"
                + " INNER JOIN Categoria cc ON p.categoria_id = cc.id"
                + " ORDER BY p.clave, c.nombre, r.nombre";

            script_ = script_.Replace("between @mesinicio AND @mesfin", "= TablaMaestra.mesCierre");

            DataTable tablaDetalle = new DataTable();

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(script_, conn);
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 1000;

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(tablaDetalle);
            }

            List<int> idVariables = new List<int>();

            foreach (int cond in idcondiciones)
            {
                Condicion condicion = tabla.Condicions.Where(x => x.id == cond).FirstOrDefault();
                if (!idVariables.Contains((int)condicion.variable_id))
                {
                    Variable variable = tabla.Variables.Where(x => x.id == condicion.variable_id).FirstOrDefault();
                    tablaDetalle.Columns.Add(variable.nombre + "_VALOR_OBTENIDO", typeof(string));
                    idVariables.Add(variable.id);
                }
                if (tablaDetalle.Columns[condicion.descripcion + "_VALOR_ESPERADO"] == null)
                {
                    tablaDetalle.Columns.Add(condicion.descripcion + "_VALOR_ESPERADO", typeof(string));
                    tablaDetalle.Columns.Add(condicion.descripcion + "_VALOR_FALTANTE", typeof(string));
                }
            }

            tablaDetalle.Columns.Add("VALOR_PREMIO", typeof(string));

            List<ReglaxConceptoDescuento> rdescuentos = tabla.ReglaxConceptoDescuentoes.Where(x => x.regla_id == regla.id).ToList();

            foreach (ReglaxConceptoDescuento rgldes in rdescuentos)
            {
                ConceptoDescuento concepto = tabla.ConceptoDescuentoes.Where(x => x.id == rgldes.conceptoDescuento_id).FirstOrDefault();
                tablaDetalle.Columns.Add("DESCUENTO_" + concepto.nombre, typeof(string));
            }

            tablaDetalle.Columns.Add("VALOR_CON_DESCUENTO", typeof(string));
            tablaDetalle.Columns.Add("PREMIO", typeof(string));
            tablaDetalle.Columns.Add("VALOR", typeof(string));
            tablaDetalle.Columns.Add("RESULTADO", typeof(string));

            //DataTable reporte = tablaDetalle.Clone();

            List<DataRow> rows = new List<DataRow>();
            foreach (int idPart in idsParticipantes)
            {
                Participante participante = tabla.Participantes.Where(x => x.id == idPart).FirstOrDefault();
                rows.Clear();
                rows.AddRange(tablaDetalle.Select("clave = '" + participante.clave + "'").ToList());
                if (rows.Count > 0)
                {
                    int indice = tablaDetalle.Rows.IndexOf(rows[rows.Count - 1]);
                    DataRow rownew = tablaDetalle.NewRow();
                    for (int i = 0; i < 7; i++)
                    {
                        rownew[i] = rows[0][i];
                    }

                    int idcol = 9;
                    foreach (int idv in idsVariables)
                    {
                        double valortotal = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            double valor_mes = 0;
                            foreach (DataRow row in rows)
                            {
                                valor_mes += (String.IsNullOrWhiteSpace(row[idcol].ToString()) ? 0 : double.Parse(row[idcol].ToString()));
                            }
                            rownew[idcol] = valor_mes.ToString();
                            idcol++;
                            valortotal += valor_mes;
                        }
                        rownew[idcol] = valortotal.ToString();
                        idcol++;
                    }
                    List<int> idVariables2 = new List<int>();
                    Dictionary<int, double> valorobtenido = new Dictionary<int, double>();
                    foreach (int cond in idcondiciones)
                    {
                        Condicion condicion = tabla.Condicions.Where(x => x.id == cond).FirstOrDefault();
                        if (!idVariables2.Contains((int)condicion.variable_id))
                        {
                            LiquidacionReglaDetalleCondicion liquidet = (from ld in tabla.LiquidacionReglaDetalleCondicions
                                                                         where ld.liquidacionRegla_id == liquidacionRegla.id
                                                                         && ld.participante_id == idPart
                                                                         && ld.condicion_id == cond
                                                                         select ld).FirstOrDefault();
                            //liquidacionDetalle.Where(x => x.participante_id == idPart && x.condicion_id == cond).FirstOrDefault();
                            if (liquidet != null)
                            {
                                Variable variable = tabla.Variables.Where(x => x.id == condicion.variable_id).FirstOrDefault();
                                rownew[variable.nombre + "_VALOR_OBTENIDO"] = (String.IsNullOrWhiteSpace(liquidet.valor) ? "0" : liquidet.valor);
                                valorobtenido.Add((int)condicion.variable_id, double.Parse(String.IsNullOrWhiteSpace(liquidet.valor) ? "0" : liquidet.valor));
                                idVariables2.Add((int)condicion.variable_id);
                            }
                        }
                        rownew[condicion.descripcion + "_VALOR_ESPERADO"] = condicion.valor;
                        rownew[condicion.descripcion + "_VALOR_FALTANTE"] = (double.Parse(condicion.valor) - (valorobtenido.Keys.Contains((int)condicion.variable_id) ? valorobtenido[(int)condicion.variable_id] : 0)).ToString();
                    }

                    LiquidacionReglaPremio premio_temp = tabla.LiquidacionReglaPremios.Where(x => x.liquidacionrelga_id == idLiquidacionRegla && x.participante_id == idPart).ToList().LastOrDefault();
                    if (premio_temp != null)
                    {
                        rownew["VALOR_PREMIO"] = premio_temp.valorPremio.ToString();
                    }
                    else
                    {
                        rownew["VALOR_PREMIO"] = "0";
                    }

                    double descuentoTotal = 0;
                    foreach (ReglaxConceptoDescuento rgldes in rdescuentos)
                    {
                        ConceptoDescuento concepto = tabla.ConceptoDescuentoes.Where(x => x.id == rgldes.conceptoDescuento_id).FirstOrDefault();
                        ConsolidadoMe consolidado = tabla.ConsolidadoMes.Where(x => x.año == anio && x.participante_id == idPart && x.tipoMedida_id == concepto.tipoMedida_id).FirstOrDefault();
                        double descuento = 0;
                        if (consolidado != null)
                        {
                            for (int mes = mesinicio; mes <= mesfin; mes++)
                            {
                                switch (mes)
                                {
                                    case 1:
                                        descuento += (double)consolidado.Enero;
                                        break;
                                    case 2:
                                        descuento += (double)consolidado.Febrero;
                                        break;
                                    case 3:
                                        descuento += (double)consolidado.Marzo;
                                        break;
                                    case 4:
                                        descuento += (double)consolidado.Abril;
                                        break;
                                    case 5:
                                        descuento += (double)consolidado.Mayo;
                                        break;
                                    case 6:
                                        descuento += (double)consolidado.Junio;
                                        break;
                                    case 7:
                                        descuento += (double)consolidado.Julio;
                                        break;
                                    case 8:
                                        descuento += (double)consolidado.Agosto;
                                        break;
                                    case 9:
                                        descuento += (double)consolidado.Septiembre;
                                        break;
                                    case 10:
                                        descuento += (double)consolidado.Octubre;
                                        break;
                                    case 11:
                                        descuento += (double)consolidado.Noviembre;
                                        break;
                                    case 12:
                                        descuento += (double)consolidado.Diciembre;
                                        break;
                                }
                            }
                        }
                        rownew["DESCUENTO_" + concepto.nombre] = descuento.ToString();
                        descuentoTotal += descuento;
                    }

                    rownew["VALOR_CON_DESCUENTO"] = (double.Parse(rownew["VALOR_PREMIO"].ToString()) - descuentoTotal).ToString();

                    LiquidacionReglaPremio premio = tabla.LiquidacionReglaPremios.Where(x => x.liquidacionrelga_id == idLiquidacionRegla && x.participante_id == idPart).ToList().LastOrDefault();
                    if (premio != null)
                    {
                        Premio premioO = tabla.Premios.Where(x => x.id == premio.premio_id).FirstOrDefault();
                        rownew["PREMIO"] = premioO.descripcion;
                        idcol++;
                        rownew["VALOR"] = premioO.valor.ToString();
                        idcol++;
                        if (premioO.tipoPremio_id == 3 && premioO.operador_id == 9)
                        {
                            rownew["RESULTADO"] = (double.Parse(rownew["VALOR_CON_DESCUENTO"].ToString()) * (double)premioO.valor).ToString();
                        }
                        else
                        {
                            rownew["RESULTADO"] = "GANO";
                        }
                    }
                    tablaDetalle.Rows.InsertAt(rownew, indice + 1);
                    //reporte.Rows.Add(rownew);
                }
            }

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            GridView GridViewResultados = new GridView();
            GridViewResultados.DataSource = tablaDetalle;
            GridViewResultados.AllowPaging = false;
            GridViewResultados.DataBind();
            //Change the Header Row back to white color
            int celda = 0;
            int detalles = 9 + (12 * idsVariables.Count);

            foreach (TableCell cell in GridViewResultados.HeaderRow.Cells)
            {
                if (celda < detalles)
                {
                    cell.Style.Add("background-color", "#000000");
                    cell.ForeColor = System.Drawing.Color.White;
                    cell.Font.Bold = true;
                    celda++;
                }
                else
                {
                    if (cell.Text.Contains("_VALOR_OBTENIDO") || cell.Text.Contains("VALOR_PREMIO"))
                    {
                        cell.Style.Add("background-color", "#7A9F35");
                        cell.ForeColor = System.Drawing.Color.White;
                        cell.Font.Bold = true;
                        celda++;
                    }
                    else
                    {
                        if (cell.Text.Contains("_VALOR_ESPERADO") || cell.Text.Contains("_VALOR_FALTANTE"))
                        {
                            cell.Style.Add("background-color", "#0F2E86");
                            cell.ForeColor = System.Drawing.Color.White;
                            cell.Font.Bold = true;
                            celda++;
                        }
                        else
                        {
                            if (cell.Text.Contains("DESCUENTO"))
                            {
                                cell.BackColor = System.Drawing.Color.Red;
                                cell.ForeColor = System.Drawing.Color.White;
                                cell.Font.Bold = true;
                                celda++;
                            }
                            else
                            {
                                cell.Style.Add("background-color", "#2F0993");
                                cell.ForeColor = System.Drawing.Color.White;
                                cell.Font.Bold = true;
                                celda++;
                            }
                        }

                    }
                }
            }

            string clave = string.Empty;
            System.Drawing.Color color = System.Drawing.Color.FromName("AliceBlue");
            foreach (GridViewRow row in GridViewResultados.Rows)
            {
                //Apply text style to each Row
                row.Attributes.Add("class", "textmode");

                if (clave == row.Cells[3].Text)
                {
                    row.BackColor = color;
                }
                else
                {
                    int fila = row.RowIndex;
                    if (fila > 1)
                        GridViewResultados.Rows[fila - 1].Font.Bold = true;
                    clave = row.Cells[3].Text;
                    color = (color.Name == "AliceBlue" ? System.Drawing.Color.White : System.Drawing.Color.FromName("AliceBlue"));
                    row.BackColor = color;
                }
            }

            GridViewResultados.RenderControl(hw);

            string renderedGridView = sw.ToString();
            string rutaarchivo = ConfigurationManager.AppSettings["RutaReportesPremios"];
            System.IO.File.WriteAllText(rutaarchivo + "ReporteLiquidacionAsesor.xls", renderedGridView);

            string usuarioftp = ConfigurationManager.AppSettings["usuarioftp"];
            string passwordftp = ConfigurationManager.AppSettings["contrasenaftp"];
            string rutaFTP = ConfigurationManager.AppSettings["FTPReportes"] + "ReporteLiquidacionAsesor_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + "/";
            string nombreArchivo = "ReporteLiquidacionAsesor.xls";

            try
            {
                WebRequest request = WebRequest.Create(rutaFTP);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(usuarioftp, passwordftp);
                var resp = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {

            }


            FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(rutaFTP + nombreArchivo);
            ftpWebRequest.Credentials = new NetworkCredential(usuarioftp, passwordftp);
            ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;


            Byte[] BufferArchivo = File.ReadAllBytes(rutaarchivo + "ReporteLiquidacionAsesor.xls");
            Stream stream = ftpWebRequest.GetRequestStream();
            stream.Write(BufferArchivo, 0, BufferArchivo.Length);
            stream.Close();
            stream.Dispose();

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
            {
                string script_1 = "DELETE FROM ProcesoLiquidacion WHERE liquidacion_id = " + liquidacionRegla.id.ToString();

                conn.Open();

                SqlCommand command = new SqlCommand(script_1, conn);
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 1000;

                command.ExecuteNonQuery();

                command.Connection.Close();
            }

            return tablaDetalle;
        }

        #endregion


        /// <summary>
        /// Obtiene listado de liquidaciones
        /// </summary>
        /// <returns>Lista de liquidaciones</returns>
        public List<string> ListarLiquidaciones(int regla_id)
        {
            int? tipoConcurso = (int)(from r in tabla.Reglas
                                      join c in tabla.Concursoes on r.concurso_id equals c.id
                                      where r.id == regla_id
                                      select c).Take(1).ToList()[0].tipoConcurso_id;
            // Número de liquidaciones que existen del mismo tipo de concurso de la regla que se esta liquidando
            var liquidaciones1 = (from l in tabla.LiquidacionReglas
                                  join r in tabla.Reglas on l.regla_id equals r.id
                                  join c in tabla.Concursoes on r.concurso_id equals c.id
                                  where c.tipoConcurso_id == tipoConcurso && r.id == regla_id
                                  select l).Count();
            // Número de liquidaciones que existen del mismo tipo de concurso del concurso seleccionado
            //var liquidaciones2 = (from c in tabla.Concursoes where c.tipoConcurso_id == tipoConcurso select c).Count();

            List<string> conteoLiquidaciones = new List<string>();
            conteoLiquidaciones.Add(liquidaciones1.ToString()); //conteoLiquidaciones.Add(liquidaciones2.ToString());
            return conteoLiquidaciones;
        }
        /// <summary>
        /// Obtiene el listado de liquidaciones de una regla
        /// </summary>
        /// <param name="idRegla">Código de la regla</param>
        /// <returns>Listado de liquidaciones</returns>
        public List<LiquidacionRegla> ListarLiquidacionesRegla(int idRegla)
        {
            return tabla.LiquidacionReglas.Include("EstadoLiquidacion").Where(Liquidacion => Liquidacion.regla_id == idRegla).ToList();
        }

        public int InsertarLiquidacionRegla(LiquidacionRegla liquidacion, string Username)
        {
            int resultado = 0;
            liquidacion.fecha_liquidacion = DateTime.Now;
            tabla.LiquidacionReglas.AddObject(liquidacion);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquidacionRegla,
    SegmentosInsercion.Personas_Y_Pymes, null, liquidacion);
            tabla.SaveChanges();
            resultado = liquidacion.id;

            return resultado;
        }

        public int GenerarLiquidacionRegla(int idLiquidacionRegla, DateTime fechaInicio, DateTime fechaFin, int idRegla, int idConcurso)
        {
            int result = 0;
            List<LiquidacionRegla> liquidacionFranquiciaList = tabla.LiquidacionReglas.Where(x => x.regla_id == idRegla && x.estado == 2 && ((x.fecha_inicial >= fechaInicio && x.fecha_inicial <= fechaFin) || (x.fecha_final >= fechaInicio && x.fecha_final <= fechaFin))).ToList();

            if (liquidacionFranquiciaList.Count() <= 0)
            {
                SqlConnection conexion = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString + "; Asynchronous Processing=True;");//Helper.getConexionAsincrona().ConnectionString
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = "LiquidarRegla_Iniciar";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add(new SqlParameter("IdConcurso", idConcurso));
                comando.Parameters.Add(new SqlParameter("IdRegla", idRegla));
                comando.Parameters.Add(new SqlParameter("fechaInicio", fechaInicio.Year.ToString() + "-" + fechaInicio.Month.ToString() + "-" + fechaInicio.Day.ToString()));
                comando.Parameters.Add(new SqlParameter("fechaFinal", fechaFin.Year.ToString() + "-" + fechaFin.Month.ToString() + "-" + fechaFin.Day.ToString()));

                bool openingConnection = comando.Connection.State == ConnectionState.Closed;
                if (openingConnection) { comando.Connection.Open(); comando.CommandTimeout = 12000; }

                comando.BeginExecuteNonQuery(delegate(IAsyncResult ar)
                {
                    try { comando.EndExecuteNonQuery(ar); }
                    finally
                    {
                        if (openingConnection && comando.Connection.State == ConnectionState.Open)
                        {
                            comando.Connection.Close();
                        }
                    }
                }, null);
                /*AsyncCallback callback = new AsyncCallback(manejarLlamado);
                comando.BeginExecuteNonQuery(callback, comando);*/
                result = 1;
            }
            return result;
        }

        private void manejarLlamado(IAsyncResult resultado)
        {
            SqlCommand comando = (SqlCommand)resultado.AsyncState;
            int filas = comando.EndExecuteNonQuery(resultado);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ObtenerProcesoLiquidacionRegla()
        {
            int idProceso = 0;
            SAI_Entities contextoTemp = new SAI_Entities();
            var idLiquidacionTemp = (from lfp in contextoTemp.LiquidacionFranquiciaProcesoes select lfp.liquidacion_id).Max();

            if (idLiquidacionTemp != null)
            {
                idProceso = Convert.ToInt32(contextoTemp.LiquidacionFranquiciaProcesoes.Single(x => x.liquidacion_id == idLiquidacionTemp).proceso);
                //ESTA TOMANDO UNO ANTERIOR
                if (idProceso == 6) idProceso = 0;
            }
            return idProceso;
        }
        /// <summary>
        /// Retorna 0 cuando la parametrización de la regla NO sea valida - no tiene variables de liquidación; 1 de lo contrario
        /// </summary>
        /// <param name="regla_id"></param>
        /// <param name="concurso_id">El concurso no es un parametro requerido</param>
        /// <returns></returns>
        public int validarEstadoLiquidacion(int regla_id, int concurso_id)
        {
            int resultado = 1;

            var subReglasxRegla = this.tabla.SubReglas.Where(SubRegla => SubRegla.regla_id == regla_id && SubRegla.condicionAgrupada_id == null && SubRegla.tipoSubregla != 3);

            foreach (var item in subReglasxRegla)
            {
                int subregla_id = (int)item.id;
                var q1 = (from c in tabla.Condicions
                          join sr in tabla.SubReglas on c.subregla_id equals sr.id
                          join r in tabla.Reglas on sr.regla_id equals r.id
                          join v in tabla.Variables on c.variable_id equals v.id
                          where (c.subregla_id == subregla_id && sr.regla_id == regla_id && v.esFiltro == false
                          )
                          select c).Count();
                if (q1 == 0)
                {
                    return 0;
                }
            }

            return resultado;
        }

        public int validarParticipantesxConcurso(int concurso_id)
        {
            var q1 = (from pc in tabla.ParticipanteConcursoes
                      join c in tabla.Concursoes on pc.concurso_id equals c.id
                      where (c.id == concurso_id)
                      select pc).Count();
            return q1;
        }

        public int RetornarTipoConcurso(int concurso_id)
        {
            int resultado = 0;

            try
            {
                var q1 = (from c in tabla.Concursoes
                          where (c.id == concurso_id)
                          select c.tipoConcurso_id).First();
                resultado = (int)q1;
            }

            catch { }

            return resultado;
        }

        public List<DetalleLiquidacionEncabezado> TipoConcursoxRegla(int liquidacionRegla_id, string valorConsulta)
        {
            List<DetalleLiquidacionEncabezado> datos = new List<DetalleLiquidacionEncabezado>();

            var q1 = (from c in tabla.Concursoes
                      join r in tabla.Reglas on c.id equals r.concurso_id
                      join lr in tabla.LiquidacionReglas on r.id equals lr.regla_id
                      where (lr.id == liquidacionRegla_id)
                      select new
                      {
                          concursoNombre = c.nombre,
                          reglaNombre = r.nombre,
                          lr.fecha_liquidacion,
                          lr.fecha_inicial,
                          lr.fecha_final,
                          c.tipoConcurso_id,
                          regla_id = r.id
                      });

            datos = (from q in q1.AsEnumerable()
                     select new DetalleLiquidacionEncabezado
                     {
                         concursoNombre = q.concursoNombre,
                         reglaNombre = q.reglaNombre,
                         fecha_liquidacion = q.fecha_liquidacion,
                         fecha_inicial = q.fecha_inicial,
                         fecha_final = q.fecha_final,
                         tipoConcurso_id = q.tipoConcurso_id,
                         regla_id = q.regla_id
                     }).ToList();

            return datos;
        }


        #region Participantes Concurso

        public List<Participante> ListarParticipantesConcurso(int idLiquidacionRegla, int inicio, int cantidad)
        {
            List<Participante> participantes = new List<Participante>();
            //LiquidacionReglaxParticipante liqui = new LiquidacionReglaxParticipante();
            //participantes = tabla.Participantes.Include("LiquidacionReglaxParticipantes").Where(p => p.LiquidacionReglaxParticipantes == liqui).ToList();
            //participantes = tabla.LiquidacionReglaxParticipantes.Include("Participante").Where(p => p.liquidacionRegla_id == idLiquidacionRegla)
            //    .Select(l => new { l.Participante }).ToList();
            foreach (Participante participante in (List<Participante>)(from participante in tabla.Participantes
                                                                       join liquidacion in tabla.LiquidacionReglaxParticipantes on participante.id equals liquidacion.participante_id
                                                                       where liquidacion.liquidacionRegla_id == idLiquidacionRegla
                                                                       select participante).OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList())
            {
                Participante p = new Participante()
                {
                    id = participante.id,
                    nombre = participante.nombre,
                    apellidos = participante.apellidos,
                    documento = participante.documento,
                    clave = participante.clave,
                    email = participante.email
                };
                participantes.Add(p);
            }
            return participantes;
        }

        public int ActualizarLiquidacionReglaEstado(int idLiquidacion, int idEstado, string Username)
        {
            LiquidacionRegla liquidRegla = tabla.LiquidacionReglas.Where(e => e.id == idLiquidacion).FirstOrDefault();
            var pValorAntiguo = liquidRegla;
            liquidRegla.estado = idEstado;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LiquidacionRegla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, liquidRegla);
            return tabla.SaveChanges();
        }
        #endregion
        #endregion

        #region Resultado Liquidación

        public List<VistaDetalleLiquidacionReglaParticipante> ListarDetalleLiquidacionReglaParticipante(int idLiquidacionRegla, int participante_id)
        {
            return tabla.VistaDetalleLiquidacionReglaParticipantes.Where(d => d.participante_id == participante_id && d.liquidacionRegla_id == idLiquidacionRegla).ToList();
        }
        #endregion

        #region Liquidacion Pagos Regla

        public int GenerarLiquidacionPagos(int idLiquidacionRegla)
        {
            int result = 0;
            EntityConnection entityConnection = (EntityConnection)tabla.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "LiquidarRegla_Pagos";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("idLiquidacionRegla", idLiquidacionRegla));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 1200; }
            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }
            return result;
        }
        #endregion

        #region BORRAR LIQUIDACION

        public int EliminarLiquidacionConcurso(int liqregla, string Username)
        {
            int result = 0;
            {
                EntityConnection entityConnection = (EntityConnection)tabla.Connection;
                DbConnection storeConnection = entityConnection.StoreConnection;
                DbCommand command = storeConnection.CreateCommand();
                command.CommandText = "EliminarLiquidacionConcurso";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("liqregla", liqregla));

                bool openingConnection = command.Connection.State == ConnectionState.Closed;

                if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 1200; }
                try
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Concurso,
    SegmentosInsercion.Personas_Y_Pymes, null, null);
                    result = command.ExecuteNonQuery();
                }
                finally
                {
                    if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
                }

            }
            return result;
        }

        #endregion
    }

    public class ListaParticipantes
    {
        private List<Participante> Participantes { get; set; }
    }
}
/*EntityConnection entityConnection = Helper.getConexionAsincrona();
DbConnection storeConnection = entityConnection.StoreConnection;
DbCommand command = storeConnection.CreateCommand();
command.CommandText = "LiquidarRegla_Iniciar";
command.CommandType = CommandType.StoredProcedure;
command.Parameters.Add(new SqlParameter("IdConcurso", idConcurso));
command.Parameters.Add(new SqlParameter("IdRegla", idRegla));
command.Parameters.Add(new SqlParameter("fechaInicio", fechaInicio.Year.ToString() + "-" + fechaInicio.Month.ToString() + "-" + fechaInicio.Day.ToString()));
command.Parameters.Add(new SqlParameter("fechaFinal", fechaFin.Year.ToString() + "-" + fechaFin.Month.ToString() + "-" + fechaFin.Day.ToString()));

bool openingConnection = command.Connection.State == ConnectionState.Closed;
if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 1200; }

try {
    result = command.ExecuteNonQuery();
}
finally {
    if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
}*/