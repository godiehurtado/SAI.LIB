using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;
using ColpatriaSAI.Negocio.Componentes.Concursos;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using ColpatriaSAI.Negocio.Componentes.Utilidades;
using System.Diagnostics;
using ColpatriaSAI.Negocio.Entidades.Informacion;

namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    public class Reglas
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region REGLAS

        public List<Regla> ListarRegla()
        {
            return tabla.Reglas.Include("Concurso").Include("EstrategiaRegla").Include("PeriodoRegla").Include("TipoRegla").Include("SubReglas").Where(Regla => Regla.id > 0).ToList();
        }

        public List<Regla> ListarReglaPorId(int id)
        {
            return tabla.Reglas.Include("Concurso").Include("EstrategiaRegla").Include("PeriodoRegla").Include("TipoRegla").Include("SubReglas").Where(Regla => Regla.id == id && Regla.id > 0).ToList();
        }

        /// <summary>
        /// Permite obtener las reglas de un concurso
        /// </summary>
        /// <param name="concurso_id">Código del concurso a consultar</param>
        /// <returns>Lista de reglas del concurso</returns>
        public List<Regla> ListarReglaPorConcursoId(int concurso_id)
        {
            return tabla.Reglas.Where(Regla => Regla.concurso_id == concurso_id).ToList();
        }

        public List<Regla> ListarPremiosParaAsociar(int concurso_id, int regla_id)
        {
            return tabla.Reglas.Where(r => r.concurso_id == concurso_id && r.id != regla_id).ToList();
        }

        public int InsertarRegla(Regla regla, string Username)
        {
            int resultado = 0;

            if (tabla.Reglas.Where(Regla => Regla.nombre == regla.nombre
                && Regla.periodoRegla_id == regla.periodoRegla_id
                && Regla.fecha_inicio == regla.fecha_inicio
                && Regla.fecha_fin == regla.fecha_fin
                && Regla.tipoRegla_id == regla.tipoRegla_id
                && Regla.concurso_id == regla.concurso_id).ToList().Count() == 0)
            {
                tabla.Reglas.AddObject(regla);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Regla,
    SegmentosInsercion.Personas_Y_Pymes, null, regla);
                tabla.SaveChanges();
                resultado = regla.id;
            }
            return resultado;
        }

        public int ActualizarRegla(int id, Regla regla, string Username)
        {
            var reglaActual = this.tabla.Reglas.Where(Regla => Regla.id == id).First();
            var pValorAntiguo = reglaActual;
            reglaActual.descripcion = regla.descripcion;
            reglaActual.nombre = regla.nombre;
            reglaActual.tipoRegla_id = regla.tipoRegla_id;
            reglaActual.periodoRegla_id = regla.periodoRegla_id;
            reglaActual.fecha_inicio = regla.fecha_inicio;
            reglaActual.fecha_fin = regla.fecha_fin;
            reglaActual.regla_id = regla.regla_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Regla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, reglaActual);
            return tabla.SaveChanges();
        }

        public string EliminarRegla(int id, Regla regla, string Username)
        {
            // Se valida la cantidad de liquidaciones de la regla. Sin la regla tiene liquidaciones no se elimina.
            int cantidadLiquidaciones = VerificarLiquidacionRegla(id);

            if (cantidadLiquidaciones > 0)
            {
                return "1";
            }

            var conceptoxregla = this.tabla.ReglaxConceptoDescuentoes.Where(rcd => rcd.regla_id == id);
            foreach (var item6 in conceptoxregla)
            {
                tabla.DeleteObject(item6);
            }

            var reglaActual = this.tabla.Reglas.Where(Regla => Regla.id == id && Regla.id > 0).First();
            ActualizarReferencia(id);
            tabla.DeleteObject(reglaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Regla,
    SegmentosInsercion.Personas_Y_Pymes, reglaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            var eliminarSubN = this.tabla.SubReglas.Where(SubRegla => SubRegla.regla_id == null && SubRegla.condicionAgrupada_id == null && SubRegla.id > 0);

            foreach (var item5 in eliminarSubN)
            {
                tabla.DeleteObject(item5);
            }
            tabla.SaveChanges();
            return "";
        }
        /// <summary>
        /// Actualiza a NULL los valores de condicionAgrupada_id, subregla_id1 y subregla_id2
        /// para poder eliminar la subregla y la condición Agrupada. Ésto se da por que existe una referenciación
        /// bilateral entre las tablas dbo.Subregla y dbo.CondicionAgrupada.
        /// </summary>
        /// <param name="regla">Id de la regla para recorrer todas las subreglas pertenecientes a la regla.</param>
        /// <returns>""</returns>
        public string ActualizarReferencia(int regla)
        {
            var subreglan = this.tabla.SubReglas.Where(sr => sr.regla_id == regla);
            foreach (var item in subreglan)
            {
                var condicionAgrn = this.tabla.CondicionAgrupadas.Where(ca => ca.id == item.condicionAgrupada_id);

                foreach (var item1 in condicionAgrn)
                {
                    item.condicionAgrupada_id = null;
                    item1.subregla_id1 = null;
                    item1.subregla_id2 = null;
                }
            }
            tabla.SaveChanges();
            EliminarNSubRegla(regla);
            return "";
        }
        /// <summary>
        /// Método que duplica la Regla y todos los atributos de la regla (subregla, condiciones, premiosxsubregla y agrupaciones).
        /// </summary>
        /// <param name="id">id de la regla a duplicar.</param>
        /// <param name="regla">colección con los atributos e la regla.</param>
        /// <returns>El nuevo id de la regla insertada.</returns>
        public int DuplicarRegla(int id, Regla regla, string Username)
        {
            int resultado = 0;
            var reglaActual = this.tabla.Reglas.Where(r => r.id == id).First();

            Regla reglaNueva = new Regla();

            reglaNueva.nombre = reglaActual.nombre + " copia " + DateTime.Now.ToShortDateString();
            reglaNueva.fecha_inicio = regla.fecha_inicio;
            reglaNueva.fecha_fin = regla.fecha_fin;
            reglaNueva.tipoRegla_id = reglaActual.tipoRegla_id;
            reglaNueva.periodoRegla_id = reglaActual.periodoRegla_id;
            reglaNueva.descripcion = reglaActual.descripcion;
            reglaNueva.concurso_id = regla.concurso_id;

            tabla.Reglas.AddObject(reglaNueva);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Regla,
    SegmentosInsercion.Personas_Y_Pymes, reglaActual, reglaNueva);
            tabla.SaveChanges();
            resultado = reglaNueva.id;

            Dictionary<int, int> idSubRegla = DuplicarSubReglasxRegla(id, resultado);

            foreach (var itemc in idSubRegla)
            {
                Dictionary<int, int> idCondicion = DuplicarCondicionesxRegla(itemc);
            }
            foreach (var itemp in idSubRegla)
            {
                Dictionary<int, int> idPremio = DuplicarPremiosxRegla(itemp);
            }
            return resultado;
        }

        /// <summary>
        /// Elimina las condiciones, premios y agrupaciones asociados a un id de subregla.
        /// </summary>
        /// <param name="idNuevo">id del la subregla</param>        
        public string EliminarNSubRegla(int idNuevo)
        {
            var subreglaCC = this.tabla.SubReglas.Where(sr => sr.regla_id == idNuevo);

            foreach (var item in subreglaCC)
            {
                var condicionDelete = this.tabla.Condicions.Where(c => c.subregla_id == item.id);

                foreach (var item2 in condicionDelete)
                {
                    tabla.DeleteObject(item2);
                }

                var premioxsubreglaDelete = this.tabla.PremioxSubreglas.Where(pxs => pxs.subregla_id == item.id);

                foreach (var item4 in premioxsubreglaDelete)
                {
                    tabla.DeleteObject(item4);
                }

                var condicionAgr = this.tabla.CondicionAgrupadas.Where(ca => (ca.subregla_id1 == null || ca.subregla_id2 == null));

                foreach (var item1 in condicionAgr)
                {
                    tabla.DeleteObject(item1);
                }
                tabla.DeleteObject(item);
            }
            return "";
        }

        public Dictionary<int, int> DuplicarSubReglasxRegla(int subreglaActual, int subreglaNueva)
        {
            Dictionary<int, int> resultadoSubRegla = new Dictionary<int, int>();
            var subreglaconcursoActual = this.tabla.SubReglas.Where(SubRegla => SubRegla.regla_id == subreglaActual
                && SubRegla.condicionAgrupada_id == null && SubRegla.tipoSubregla != 2); // Subreglas Simples.

            if (subreglaconcursoActual != null)
            {
                foreach (var item in subreglaconcursoActual)
                {
                    SubRegla subreglaconcursoNuevo = new SubRegla();
                    subreglaconcursoNuevo.descripcion = item.descripcion;
                    subreglaconcursoNuevo.principal = item.principal;
                    subreglaconcursoNuevo.regla_id = subreglaNueva; // Nuevo id de Regla (Value)
                    subreglaconcursoNuevo.tipoSubregla = item.tipoSubregla;

                    SAI_Entities tabla1 = new SAI_Entities();
                    tabla1.SubReglas.AddObject(subreglaconcursoNuevo);
                    tabla1.SaveChanges();
                    resultadoSubRegla.Add(item.id, subreglaconcursoNuevo.id);

                    RecorrerSubreglasAgrupadasxRegla(item.id, subreglaconcursoNuevo.id, subreglaActual, subreglaNueva);
                }
                DuplicarSubreglasAgrupadasxRegla();

                return resultadoSubRegla;
            }
            else
                return null;
        }

        public int RecorrerSubreglasAgrupadasxRegla(int subreglaActual, int subreglaNueva, int reglaActual, int reglaNueva)
        {
            int result = 0;
            {
                EntityConnection entityConnection = (EntityConnection)tabla.Connection;
                DbConnection storeConnection = entityConnection.StoreConnection;
                DbCommand command = storeConnection.CreateCommand();
                command.CommandText = "RecorrerSubreglasAgrupadas";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("subreglaActual", subreglaActual));
                command.Parameters.Add(new SqlParameter("subreglaNueva", subreglaNueva));
                command.Parameters.Add(new SqlParameter("reglaActual", reglaActual));
                command.Parameters.Add(new SqlParameter("reglaNueva", reglaNueva));

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
            }
            return result;
        }

        public int DuplicarSubreglasAgrupadasxRegla()
        {
            int result = 0;
            {
                EntityConnection entityConnection = (EntityConnection)tabla.Connection;
                DbConnection storeConnection = entityConnection.StoreConnection;
                DbCommand command = storeConnection.CreateCommand();
                command.CommandText = "DuplicarSubreglasAgrupadas";

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
            }
            return result;
        }

        public Dictionary<int, int> DuplicarCondicionesxRegla(KeyValuePair<int, int> SubReglaTotal)
        {
            Dictionary<int, int> resultadoCondicion = new Dictionary<int, int>();
            var condicionconcursoActual = this.tabla.Condicions.Where(Condicion => Condicion.subregla_id == SubReglaTotal.Key);

            if (condicionconcursoActual != null)
            {
                foreach (var item in condicionconcursoActual)
                {
                    Condicion condicionconcursoNuevo = new Condicion();

                    string selectedValue = (string)item.seleccion;
                    DateTime fechaValue = (DateTime)item.fecha;
                    string valorValue = (string)item.valor;

                    if (selectedValue == "0" && fechaValue == Convert.ToDateTime("1900-01-01 00:00:00.000"))
                    {
                        string variableCondicion1 = tabla.Variables.Single(V => V.id == item.variable_id).nombre;
                        string operadorCondicion1 = tabla.Operadors.Single(O => O.id == item.operador_id).nombre;
                        string descripcionVariable1 = variableCondicion1 + " " + operadorCondicion1 + " " + item.valor;

                        condicionconcursoNuevo.variable_id = item.variable_id;
                        condicionconcursoNuevo.operador_id = item.operador_id;
                        condicionconcursoNuevo.valor = item.valor;
                        condicionconcursoNuevo.seleccion = item.seleccion;
                        condicionconcursoNuevo.textoSeleccion = item.textoSeleccion;
                        condicionconcursoNuevo.fecha = item.fecha;
                        condicionconcursoNuevo.subregla_id = SubReglaTotal.Value;

                        SAI_Entities tabla2 = new SAI_Entities();
                        tabla2.Condicions.AddObject(condicionconcursoNuevo);
                        tabla2.SaveChanges();
                        resultadoCondicion.Add(item.id, condicionconcursoNuevo.id);

                        var condicionActual = tabla2.Condicions.Where(Condicion => Condicion.id == condicionconcursoNuevo.id).First();
                        condicionActual.descripcion = descripcionVariable1;
                        tabla2.SaveChanges();

                    }
                    else if (valorValue == "0" && fechaValue == Convert.ToDateTime("1900-01-01 00:00:00.000"))
                    {
                        string variableCondicion2 = tabla.Variables.Single(V => V.id == item.variable_id).nombre;
                        string operadorCondicion2 = tabla.Operadors.Single(O => O.id == item.operador_id).nombre;
                        string descripcionVariable2 = variableCondicion2 + " " + operadorCondicion2 + " " + item.textoSeleccion;

                        condicionconcursoNuevo.variable_id = item.variable_id;
                        condicionconcursoNuevo.operador_id = item.operador_id;
                        condicionconcursoNuevo.valor = item.valor;
                        condicionconcursoNuevo.seleccion = item.seleccion;
                        condicionconcursoNuevo.textoSeleccion = item.textoSeleccion;
                        condicionconcursoNuevo.fecha = item.fecha;
                        condicionconcursoNuevo.subregla_id = SubReglaTotal.Value;

                        SAI_Entities tabla2 = new SAI_Entities();
                        tabla2.Condicions.AddObject(condicionconcursoNuevo);
                        tabla2.SaveChanges();
                        resultadoCondicion.Add(item.id, condicionconcursoNuevo.id);

                        var condicionActual = tabla2.Condicions.Where(Condicion => Condicion.id == condicionconcursoNuevo.id).First();
                        condicionActual.descripcion = descripcionVariable2;
                        tabla2.SaveChanges();
                    }

                    else if (valorValue == "0" && selectedValue == "0")
                    {
                        string variableCondicion3 = tabla.Variables.Single(V => V.id == item.variable_id).nombre;
                        string operadorCondicion3 = tabla.Operadors.Single(O => O.id == item.operador_id).nombre;
                        string descripcionVariable3 = variableCondicion3 + " " + operadorCondicion3 + " " + Convert.ToDateTime(item.fecha).ToString("yyyy/MM/dd");

                        condicionconcursoNuevo.variable_id = item.variable_id;
                        condicionconcursoNuevo.operador_id = item.operador_id;
                        condicionconcursoNuevo.valor = item.valor;
                        condicionconcursoNuevo.seleccion = item.seleccion;
                        condicionconcursoNuevo.textoSeleccion = item.textoSeleccion;
                        condicionconcursoNuevo.fecha = item.fecha;
                        condicionconcursoNuevo.subregla_id = SubReglaTotal.Value;

                        SAI_Entities tabla2 = new SAI_Entities();
                        tabla2.Condicions.AddObject(condicionconcursoNuevo);
                        tabla2.SaveChanges();
                        resultadoCondicion.Add(item.id, condicionconcursoNuevo.id);

                        var condicionActual = tabla2.Condicions.Where(Condicion => Condicion.id == condicionconcursoNuevo.id).First();
                        condicionActual.descripcion = descripcionVariable3;
                        tabla2.SaveChanges();
                    }
                }
                return resultadoCondicion;
            }
            else
                return null;
        }

        public Dictionary<int, int> DuplicarPremiosxRegla(KeyValuePair<int, int> SubReglaTotal)
        {
            Dictionary<int, int> resultadoPremio = new Dictionary<int, int>();
            var premioconcursoActual = this.tabla.PremioxSubreglas.Where(PremioxSubregla => PremioxSubregla.subregla_id == SubReglaTotal.Key);

            if (premioconcursoActual != null)
            {
                foreach (var item in premioconcursoActual)
                {
                    PremioxSubregla premioconcursoNuevo = new PremioxSubregla();
                    premioconcursoNuevo.premio_id = item.premio_id;
                    premioconcursoNuevo.subregla_id = SubReglaTotal.Value;

                    SAI_Entities tabla3 = new SAI_Entities();
                    tabla3.PremioxSubreglas.AddObject(premioconcursoNuevo);
                    tabla3.SaveChanges();
                    resultadoPremio.Add(item.id, premioconcursoNuevo.id);
                }
                return resultadoPremio;
            }
            else
                return null;
        }

        #endregion

        #region SUBREGLA

        public List<SubRegla> ListarSubRegla()
        {
            return tabla.SubReglas.Include("Regla").Where(SubRegla => SubRegla.id > 0).ToList();
        }

        public List<SubRegla> ListarSubReglaPorId(int id)
        {
            return tabla.SubReglas.Include("Regla").Where(SubRegla => SubRegla.id == id && SubRegla.id > 0).ToList();
        }

        public int InsertarSubRegla(SubRegla subregla, string Username)
        {
            int resultado = 0;
            int tipoSubReglaValue = (int)subregla.tipoSubregla;

            if (tipoSubReglaValue == 1)
            {
                if (tabla.SubReglas.Where(SubRegla => SubRegla.regla_id == subregla.regla_id && SubRegla.descripcion == subregla.descripcion && SubRegla.principal == subregla.principal).ToList().Count() == 0)
                {
                    tabla.SubReglas.AddObject(subregla);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.SubRegla,
    SegmentosInsercion.Personas_Y_Pymes, null, subregla);
                    tabla.SaveChanges();
                    resultado = subregla.id;
                }
            }
            else if (tipoSubReglaValue == 2)
            {
                if (tabla.SubReglas.Where(SubRegla => SubRegla.regla_id == subregla.regla_id && SubRegla.condicionAgrupada_id == subregla.condicionAgrupada_id).ToList().Count() == 0)
                {
                    tabla.SubReglas.AddObject(subregla);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.SubRegla,
    SegmentosInsercion.Personas_Y_Pymes, null, subregla);
                    tabla.SaveChanges();
                    resultado = subregla.id;
                }
            }

            else if (tipoSubReglaValue == 3)
            {
                if (tabla.SubReglas.Where(SubRegla => SubRegla.regla_id == subregla.regla_id && SubRegla.descripcion == subregla.descripcion).ToList().Count() == 0)
                {
                    tabla.SubReglas.AddObject(subregla);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.SubRegla,
    SegmentosInsercion.Personas_Y_Pymes, null, subregla);
                    tabla.SaveChanges();
                    resultado = subregla.id;
                }
            }
            return resultado;
        }

        public List<Variable> ListarVariablesxTabla(int subregla_id)
        {
            int tipoTabla = RetornarTipoTablaxVariable(subregla_id);

            List<Variable> datos = new List<Variable>();

            var q1 = (from v in tabla.Variables
                      join ttv in tabla.TipoTablaVariables on v.id equals ttv.variable_id
                      where (ttv.tipotabla_id == tipoTabla)
                      group v by new
                      {
                          v.id,
                          v.nombre,
                          v.tipoVariable_id,
                          v.tabla_id,
                          v.columnaTablaMaestra,
                          v.tipoDato,
                          v.operacionAgregacion_id,
                          v.tipoMedida_id,
                          v.tipoTabla,
                          v.tipoConcurso
                      } into variableresult
                      select new
                      {
                          id = variableresult.Max(v => v.id),
                          nombre = variableresult.Max(v => v.nombre),
                          tipoVariable_id = variableresult.Max(v => v.tipoVariable_id),
                          tabla_id = variableresult.Max(v => v.tabla_id),
                          columnaTablaMaestra = variableresult.Max(v => v.columnaTablaMaestra),
                          tipoDato = variableresult.Max(v => v.tipoDato),
                          operacionAgregacion_id = variableresult.Max(v => v.operacionAgregacion_id),
                          tipoMedida_id = variableresult.Max(v => v.tipoMedida_id),
                          tipoTabla = variableresult.Max(v => v.tipoTabla),
                          tipoConcurso = variableresult.Max(v => v.tipoConcurso)
                      });

            datos = (from q in q1.AsEnumerable()
                     select new Variable
                     {
                         id = q.id,
                         nombre = q.nombre,
                         descripcion = "",
                         tipoVariable_id = q.tipoVariable_id,
                         tabla_id = q.tabla_id,
                         columnaTablaMaestra = q.columnaTablaMaestra,
                         tipoDato = q.tipoDato,
                         operacionAgregacion_id = q.operacionAgregacion_id,
                         esFiltro = false,
                         tipoMedida_id = q.tipoMedida_id,
                         tipoTabla = q.tipoTabla,
                         tipoConcurso = q.tipoConcurso
                     }).ToList();

            return datos;
        }

        public int RetornarTipoTablaxVariable(int subregla_id)
        {
            int resultado = 0;

            var condicionActual = (from c in tabla.Condicions
                                   join v in tabla.Variables on c.variable_id equals v.id
                                   where (c.subregla_id == subregla_id && v.esFiltro == false)
                                   select c);

            foreach (var item in condicionActual)
            {
                var q1 = (from c in tabla.Condicions
                          join v in tabla.Variables on c.variable_id equals v.id
                          join ttv in tabla.TipoTablaVariables on v.id equals ttv.variable_id
                          where (c.id == item.id)
                          select ttv.tipotabla_id).First();

                resultado = (int)q1;
            }
            return resultado;
        }

        public int ActualizarSubRegla(int id, SubRegla subregla, string Username)
        {
            if (subregla.principal == false)
            {
                var subreglaActual = this.tabla.SubReglas.Where(SubRegla => SubRegla.id == id && SubRegla.id > 0).First();
                var pValorAntiguo = subreglaActual;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SubRegla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, subreglaActual);
                subreglaActual.descripcion = subregla.descripcion;
            }

            else if (subregla.principal == true)
            {
                var subreglaActual = this.tabla.SubReglas.Where(SubRegla => SubRegla.id == id && SubRegla.id > 0).First();
                subreglaActual.descripcion = subregla.descripcion;
                subreglaActual.principal = subregla.principal;

                int valorSubRegla = (int)subreglaActual.id;
                int valorRegla = (int)subreglaActual.regla_id;

                var subreglasUpdate = this.tabla.SubReglas.Where(SubRegla => SubRegla.principal == true && SubRegla.id != valorSubRegla && SubRegla.regla_id == valorRegla);
                foreach (var item in subreglasUpdate)
                {
                    var pvalorAntiguo = item;
                    item.principal = false;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SubRegla,
    SegmentosInsercion.Personas_Y_Pymes, pvalorAntiguo, item);
                }
            }
            return tabla.SaveChanges();
        }

        public string EliminarSubRegla(int id, SubRegla subregla, string Username)
        {
            // Devuelve el valor de la regla asociado a la sibregla a eliminar.
            var regla_id = (from sr in tabla.SubReglas
                            where (sr.id == id)
                            select sr.regla_id).First();

            // Devuelve el tipo de Subregla: 1 --> Simple, 2 --> Agrupada, 3 --> Excepción.
            // Si la subregla es Agrupada (2), debe ingresar al llamado del método ActualizarValoresxSubregla.
            var tipoSubregla = (from sr in tabla.SubReglas
                                where (sr.id == id)
                                select sr.tipoSubregla).First();

            int cantidadLiquidaciones = VerificarLiquidacionRegla((int)regla_id);

            if (cantidadLiquidaciones > 0)
            {
                return "1";
            }

            if ((int)tipoSubregla == 2)
            {
                ActualizarValoresxSubregla(id);
            }

            var subreglaEliminar = this.tabla.SubReglas.Where(sr => sr.id == id);
            foreach (var item in subreglaEliminar)
            {
                var condicionEliminar = this.tabla.Condicions.Where(c => c.subregla_id == item.id);

                foreach (var item1 in condicionEliminar)
                {
                    tabla.DeleteObject(item1);
                }

                var premioxsubregla = this.tabla.PremioxSubreglas.Where(ps => ps.subregla_id == item.id);

                foreach (var item2 in premioxsubregla)
                {
                    tabla.DeleteObject(item2);
                }
            }

            if ((int)tipoSubregla == 2)
            {
                var condicionAgr = this.tabla.CondicionAgrupadas.Where(ca => (ca.subregla_id1 == null || ca.subregla_id2 == null));
                foreach (var item1 in condicionAgr)
                {
                    tabla.DeleteObject(item1);
                }
            }

            var subreglaActual = this.tabla.SubReglas.Where(SubRegla => SubRegla.id == id).First();
            tabla.DeleteObject(subreglaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.SubRegla,
    SegmentosInsercion.Personas_Y_Pymes, subreglaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActualizarValoresxSubregla(int subregla_id)
        {
            var condicionAgrupada = (from sr in tabla.SubReglas
                                     where (sr.id == subregla_id)
                                     select sr.condicionAgrupada_id).First();

            var condicion = this.tabla.CondicionAgrupadas.Where(ca => ca.id == (int)condicionAgrupada).First();

            condicion.subregla_id1 = null;
            condicion.subregla_id2 = null;

            var subregla = this.tabla.SubReglas.Where(sr => sr.id == subregla_id).First();

            subregla.condicionAgrupada_id = null;

            tabla.SaveChanges();
            return "";
        }

        /// <summary>
        /// Verifica si la regla ya tiene liquidaciones. Si tiene liquidaciones devuelve 1.
        /// </summary>
        /// <param name="regla_id">id de la regla.</param>
        /// <returns>0 si la regla no tiene liquidaciones, 1 si la regla tiene liquidaciones.</returns>
        public int VerificarLiquidacionRegla(int regla_id)
        {
            int resultado = 0;

            var q1 = this.tabla.LiquidacionReglas.Where(lr => lr.regla_id == regla_id).Count();
            if (q1 > 0)
            {
                return 1;
            }
            return resultado;
        }

        #endregion

        #region CONDICIONES

        public string nombretabla(string id)
        {
            return "";
        }

        public List<Condicion> ListarCondicion()
        {
            return tabla.Condicions.Include("Variable").Include("Operador").Include("tabla").Include("PeriodoAnteriors").Include("SubRegla").Where(Condicion => Condicion.id > 0).ToList();
        }

        public List<Condicion> ListarCondicionPorId(int id)
        {
            return tabla.Condicions.Include("Variable").Include("Operador").Include("tabla").Include("PeriodoAnteriors").Include("SubRegla").Where(Condicion => Condicion.id == id).ToList();
        }

        public int MostrarTipodeVariable(Condicion condicion)
        {
            int resultadoTipoVariable = 0;
            resultadoTipoVariable = (int)condicion.Variable.tipoVariable_id;
            return resultadoTipoVariable;
        }

        public string RetornarTipoDato(int variable_id)
        {
            var q1 = (from v in tabla.Variables
                      where (v.id == variable_id)
                      select v.tipoDato).First();

            return q1;
        }

        // Cuenta la cantidad de condiciones de la subreglaActual que tienen variables con el campo esFiltro = false
        // ya que si esFiltro = false, significa que es la variable no es de liquidacion si no que unicamente filtra la informacion
        // de tabla Maestra. Si las condiciones de una SubRegla no tiene variables esFiltro = false (es decir la cuenta da '0')
        // se presenta mensaje de advertencia ya que no se puede liquidar.
        public int ContarVariablexLiquidacion(int concurso_id, int regla_id, int subregla_id)
        {
            var q1 = (from c in tabla.Condicions
                      join sr in tabla.SubReglas on c.subregla_id equals sr.id
                      join r in tabla.Reglas on sr.regla_id equals r.id
                      join v in tabla.Variables on c.variable_id equals v.id
                      where (c.subregla_id == subregla_id && sr.regla_id == regla_id && r.concurso_id == concurso_id && v.esFiltro == false)
                      select c).Count();

            return q1;
        }

        public int RetornarTipoSubRegla(int subregla_id)
        {
            int tipoSubRegla = 0;
            var q1 = (from sr in tabla.SubReglas
                      where (sr.id == subregla_id)
                      select sr.tipoSubregla).First();

            tipoSubRegla = (int)q1;
            return tipoSubRegla;
        }

        public int InsertarCondicion(Condicion condicion, string Username)
        {
            int resultado = 0;
            string selectedValue = (string)condicion.seleccion;
            DateTime fechaValue = (DateTime)condicion.fecha;
            string valorValue = (string)condicion.valor;
            string valorTipoDato = RetornarTipoDato((int)condicion.variable_id);

            if (valorTipoDato == "Numero")
            {
                string variableCondicion1 = tabla.Variables.Single(V => V.id == condicion.variable_id).nombre;
                string operadorCondicion1 = tabla.Operadors.Single(O => O.id == condicion.operador_id).nombre;
                string descripcionVariable1 = variableCondicion1 + " " + operadorCondicion1 + " " + condicion.valor;

                if (tabla.Condicions.Where(Condicion => Condicion.variable_id == condicion.variable_id && Condicion.operador_id == condicion.operador_id
                    && Condicion.valor == condicion.valor && Condicion.subregla_id == condicion.subregla_id && valorTipoDato == "Numero").ToList().Count() == 0)
                {
                    tabla.Condicions.AddObject(condicion);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Condicion,
    SegmentosInsercion.Personas_Y_Pymes, null, condicion);
                    tabla.SaveChanges();
                    resultado = condicion.id;

                    var condicionActual = this.tabla.Condicions.Where(Condicion => Condicion.id == condicion.id).First();
                    condicionActual.descripcion = descripcionVariable1;
                    tabla.SaveChanges();
                }
            }
            else if (valorTipoDato == "Seleccion")
            {
                string variableCondicion2 = tabla.Variables.Single(V => V.id == condicion.variable_id).nombre;
                string operadorCondicion2 = tabla.Operadors.Single(O => O.id == condicion.operador_id).nombre;
                string descripcionVariable2 = variableCondicion2 + " " + operadorCondicion2 + " " + condicion.textoSeleccion;

                if (tabla.Condicions.Where(Condicion => Condicion.variable_id == condicion.variable_id && Condicion.operador_id == condicion.operador_id &&
                    Condicion.seleccion == condicion.seleccion && Condicion.subregla_id == condicion.subregla_id && valorTipoDato == "Seleccion").ToList().Count() == 0)
                {
                    tabla.Condicions.AddObject(condicion);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Condicion,
    SegmentosInsercion.Personas_Y_Pymes, null, condicion);
                    tabla.SaveChanges();
                    resultado = condicion.id;

                    var condicionActual = this.tabla.Condicions.Where(Condicion => Condicion.id == condicion.id).First();
                    condicionActual.descripcion = descripcionVariable2;
                    tabla.SaveChanges();
                }
            }

            else if (valorTipoDato == "Fecha")
            {
                string variableCondicion3 = tabla.Variables.Single(V => V.id == condicion.variable_id).nombre;
                string operadorCondicion3 = tabla.Operadors.Single(O => O.id == condicion.operador_id).nombre;
                string descripcionVariable3 = variableCondicion3 + " " + operadorCondicion3 + " " + Convert.ToDateTime(condicion.fecha).ToString("yyyy/MM/dd");

                if (tabla.Condicions.Where(Condicion => Condicion.variable_id == condicion.variable_id && Condicion.operador_id == condicion.operador_id &&
                    Condicion.subregla_id == condicion.subregla_id && valorTipoDato == "Fecha").ToList().Count() == 0)
                {
                    tabla.Condicions.AddObject(condicion);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Condicion,
    SegmentosInsercion.Personas_Y_Pymes, null, condicion);
                    tabla.SaveChanges();
                    resultado = condicion.id;

                    var condicionActual = this.tabla.Condicions.Where(Condicion => Condicion.id == condicion.id).First();
                    condicionActual.descripcion = descripcionVariable3;
                    tabla.SaveChanges();
                }
            }
            return resultado;
        }

        public int ActualizarCondicion(int id, Condicion condicion, string Username)
        {
            int resultado = 0;
            string selectedValue = (string)condicion.seleccion;
            DateTime fechaValue = (DateTime)condicion.fecha;
            string valorValue = (string)condicion.valor;
            string valorTipoDato = RetornarTipoDato((int)condicion.variable_id);

            if (valorTipoDato == "Numero")
            {
                string variableCondicion1 = tabla.Variables.Single(V => V.id == condicion.variable_id).nombre;
                string operadorCondicion1 = tabla.Operadors.Single(O => O.id == condicion.operador_id).nombre;
                string descripcionVariable1 = variableCondicion1 + " " + operadorCondicion1 + " " + condicion.valor;

                var condicionActual = this.tabla.Condicions.Where(Condicion => Condicion.id == id && Condicion.id > 0).First();
                var pValorAntiguo = condicionActual;
                condicionActual.variable_id = condicion.variable_id;
                condicionActual.operador_id = condicion.operador_id;
                condicionActual.tabla_id = condicion.tabla_id;
                condicionActual.valor = condicion.valor;
                condicionActual.seleccion = condicion.seleccion;
                condicionActual.textoSeleccion = condicion.textoSeleccion;
                condicionActual.fecha = condicion.fecha;
                condicionActual.descripcion = descripcionVariable1;
                condicionActual.mesinicio = condicion.mesinicio;
                condicionActual.mesfin = condicion.mesfin;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Condicion,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, condicionActual);
                tabla.SaveChanges();

            }

            else if (valorTipoDato == "Seleccion")
            {
                string variableCondicion2 = tabla.Variables.Single(V => V.id == condicion.variable_id).nombre;
                string operadorCondicion2 = tabla.Operadors.Single(O => O.id == condicion.operador_id).nombre;
                string descripcionVariable2 = variableCondicion2 + " " + operadorCondicion2 + " " + condicion.textoSeleccion;

                var condicionActual = this.tabla.Condicions.Where(Condicion => Condicion.id == id && Condicion.id > 0).First();
                var pValorAntiguo = condicionActual;
                condicionActual.variable_id = condicion.variable_id;
                condicionActual.operador_id = condicion.operador_id;
                condicionActual.tabla_id = condicion.tabla_id;
                condicionActual.valor = condicion.valor;
                condicionActual.seleccion = condicion.seleccion;
                condicionActual.textoSeleccion = condicion.textoSeleccion;
                condicionActual.fecha = condicion.fecha;
                condicionActual.descripcion = descripcionVariable2;
                condicionActual.mesinicio = condicion.mesinicio;
                condicionActual.mesfin = condicion.mesfin;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Condicion,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, condicionActual);
                tabla.SaveChanges();
            }

            else if (valorTipoDato == "Fecha")
            {
                string variableCondicion3 = tabla.Variables.Single(V => V.id == condicion.variable_id).nombre;
                string operadorCondicion3 = tabla.Operadors.Single(O => O.id == condicion.operador_id).nombre;
                string descripcionVariable3 = variableCondicion3 + " " + operadorCondicion3 + " " + Convert.ToDateTime(condicion.fecha).ToString("yyyy/MM/dd");

                var condicionActual = this.tabla.Condicions.Where(Condicion => Condicion.id == id && Condicion.id > 0).First();
                var pValorAntiguo = condicionActual;
                condicionActual.variable_id = condicion.variable_id;
                condicionActual.operador_id = condicion.operador_id;
                condicionActual.tabla_id = condicion.tabla_id;
                condicionActual.valor = condicion.valor;
                condicionActual.seleccion = condicion.seleccion;
                condicionActual.textoSeleccion = condicion.textoSeleccion;
                condicionActual.fecha = condicion.fecha;
                condicionActual.descripcion = descripcionVariable3;
                condicionActual.mesinicio = condicion.mesinicio;
                condicionActual.mesfin = condicion.mesfin;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Condicion,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, condicionActual);
                tabla.SaveChanges();
            }

            return resultado;
        }

        public string EliminarCondicion(int id, Condicion condicion, string Username)
        {
            var condicionActual = this.tabla.Condicions.Where(Condicion => Condicion.id == id && Condicion.id > 0).First();
            tabla.DeleteObject(condicionActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Condicion,
    SegmentosInsercion.Personas_Y_Pymes, condicionActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region CONDICIONXPREMIOSUBREGLA

        public List<CondicionxPremioSubregla> ListarCondicionxPremioSubRegla()
        {
            return tabla.CondicionxPremioSubreglas.Include("Variable").Include("Operador").Where(c => c.id > 0).ToList();
        }

        public CondicionxPremioSubregla ListarCondicionxPremioSubReglaPorId(int id)
        {
            return tabla.CondicionxPremioSubreglas.Include("Variable").Include("Operador").Where(c => c.id == id).FirstOrDefault();
        }

        public int InsertarCondicionxPremioSubRegla(CondicionxPremioSubregla condicionPS, string Username)
        {
            int resultado = 0;
            string selectedValue = (string)condicionPS.seleccion;
            DateTime fechaValue = (DateTime)condicionPS.fecha;
            string valorValue = (string)condicionPS.valor;
            string valorTipoDato = RetornarTipoDato((int)condicionPS.variable_id);

            if (valorTipoDato == "Numero")
            {
                string variableCondicion1 = tabla.Variables.Single(V => V.id == condicionPS.variable_id).nombre;
                string operadorCondicion1 = tabla.Operadors.Single(O => O.id == condicionPS.operador_id).nombre;
                string descripcionVariable1 = variableCondicion1 + " " + operadorCondicion1 + " " + condicionPS.valor;

                if (tabla.CondicionxPremioSubreglas.Where(c => c.variable_id == condicionPS.variable_id && c.operador_id == condicionPS.operador_id
                    && c.valor == condicionPS.valor && c.premioxsubregla_id == condicionPS.premioxsubregla_id && valorTipoDato == "Numero").ToList().Count() == 0)
                {
                    tabla.CondicionxPremioSubreglas.AddObject(condicionPS);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, null, condicionPS);
                    tabla.SaveChanges();
                    resultado = condicionPS.id;

                    var condicionActual = this.tabla.CondicionxPremioSubreglas.Where(c => c.id == condicionPS.id).First();
                    condicionActual.descripcion = descripcionVariable1;
                    tabla.SaveChanges();
                }
            }
            else if (valorTipoDato == "Seleccion")
            {
                string variableCondicion2 = tabla.Variables.Single(V => V.id == condicionPS.variable_id).nombre;
                string operadorCondicion2 = tabla.Operadors.Single(O => O.id == condicionPS.operador_id).nombre;
                string descripcionVariable2 = variableCondicion2 + " " + operadorCondicion2 + " " + condicionPS.textoSeleccion;

                if (tabla.CondicionxPremioSubreglas.Where(c => c.variable_id == condicionPS.variable_id && c.operador_id == condicionPS.operador_id &&
                    c.seleccion == condicionPS.seleccion && c.premioxsubregla_id == condicionPS.premioxsubregla_id && valorTipoDato == "Seleccion").ToList().Count() == 0)
                {
                    tabla.CondicionxPremioSubreglas.AddObject(condicionPS);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, null, condicionPS);
                    tabla.SaveChanges();
                    resultado = condicionPS.id;

                    var condicionActual = this.tabla.CondicionxPremioSubreglas.Where(c => c.id == condicionPS.id).First();
                    condicionActual.descripcion = descripcionVariable2;
                    tabla.SaveChanges();
                }
            }

            else if (valorTipoDato == "Fecha")
            {
                string variableCondicion3 = tabla.Variables.Single(V => V.id == condicionPS.variable_id).nombre;
                string operadorCondicion3 = tabla.Operadors.Single(O => O.id == condicionPS.operador_id).nombre;
                string descripcionVariable3 = variableCondicion3 + " " + operadorCondicion3 + " " + Convert.ToDateTime(condicionPS.fecha).ToString("yyyy/MM/dd");

                if (tabla.CondicionxPremioSubreglas.Where(c => c.variable_id == condicionPS.variable_id && c.operador_id == condicionPS.operador_id &&
                    c.premioxsubregla_id == condicionPS.premioxsubregla_id && valorTipoDato == "Fecha").ToList().Count() == 0)
                {
                    tabla.CondicionxPremioSubreglas.AddObject(condicionPS);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, null, condicionPS);
                    tabla.SaveChanges();
                    resultado = condicionPS.id;

                    var condicionActual = this.tabla.CondicionxPremioSubreglas.Where(c => c.id == condicionPS.id).First();
                    condicionActual.descripcion = descripcionVariable3;
                    tabla.SaveChanges();
                }
            }
            return resultado;
        }

        public int ActualizarCondicionxPremioSubRegla(int id, CondicionxPremioSubregla condicionPS, string Username)
        {
            int resultado = 0;
            string selectedValue = (string)condicionPS.seleccion;
            DateTime fechaValue = (DateTime)condicionPS.fecha;
            string valorValue = (string)condicionPS.valor;
            string valorTipoDato = RetornarTipoDato((int)condicionPS.variable_id);

            if (valorTipoDato == "Numero")
            {
                string variableCondicion1 = tabla.Variables.Single(V => V.id == condicionPS.variable_id).nombre;
                string operadorCondicion1 = tabla.Operadors.Single(O => O.id == condicionPS.operador_id).nombre;
                string descripcionVariable1 = variableCondicion1 + " " + operadorCondicion1 + " " + condicionPS.valor;

                var condicionActual = this.tabla.CondicionxPremioSubreglas.Where(c => c.id == id && c.id > 0).First();
                var pValorAntiguo = condicionActual;
                condicionActual.variable_id = condicionPS.variable_id;
                condicionActual.operador_id = condicionPS.operador_id;
                condicionActual.valor = condicionPS.valor;
                condicionActual.seleccion = condicionPS.seleccion;
                condicionActual.textoSeleccion = condicionPS.textoSeleccion;
                condicionActual.fecha = condicionPS.fecha;
                condicionActual.descripcion = descripcionVariable1;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, condicionActual);
                tabla.SaveChanges();
            }
            else if (valorTipoDato == "Seleccion")
            {
                string variableCondicion2 = tabla.Variables.Single(V => V.id == condicionPS.variable_id).nombre;
                string operadorCondicion2 = tabla.Operadors.Single(O => O.id == condicionPS.operador_id).nombre;
                string descripcionVariable2 = variableCondicion2 + " " + operadorCondicion2 + " " + condicionPS.textoSeleccion;

                var condicionActual = this.tabla.CondicionxPremioSubreglas.Where(c => c.id == id && c.id > 0).First();
                var pValorAntiguo = condicionActual;
                condicionActual.variable_id = condicionPS.variable_id;
                condicionActual.operador_id = condicionPS.operador_id;
                condicionActual.valor = condicionPS.valor;
                condicionActual.seleccion = condicionPS.seleccion;
                condicionActual.textoSeleccion = condicionPS.textoSeleccion;
                condicionActual.fecha = condicionPS.fecha;
                condicionActual.descripcion = descripcionVariable2;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, condicionActual);
                tabla.SaveChanges();
            }
            else if (valorTipoDato == "Fecha")
            {
                string variableCondicion3 = tabla.Variables.Single(V => V.id == condicionPS.variable_id).nombre;
                string operadorCondicion3 = tabla.Operadors.Single(O => O.id == condicionPS.operador_id).nombre;
                string descripcionVariable3 = variableCondicion3 + " " + operadorCondicion3 + " " + Convert.ToDateTime(condicionPS.fecha).ToString("yyyy/MM/dd");

                var condicionActual = this.tabla.CondicionxPremioSubreglas.Where(Condicion => Condicion.id == id && Condicion.id > 0).First();
                var pValorAntiguo = condicionActual;
                condicionActual.variable_id = condicionPS.variable_id;
                condicionActual.operador_id = condicionPS.operador_id;
                condicionActual.valor = condicionPS.valor;
                condicionActual.seleccion = condicionPS.seleccion;
                condicionActual.textoSeleccion = condicionPS.textoSeleccion;
                condicionActual.fecha = condicionPS.fecha;
                condicionActual.descripcion = descripcionVariable3;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, condicionActual);
                tabla.SaveChanges();
            }

            return resultado;
        }

        public string EliminarCondicionxPremioSubRegla(int id, CondicionxPremioSubregla condicionPS, string Username)
        {
            var condicionActual = this.tabla.CondicionxPremioSubreglas.Where(c => c.id == id && c.id > 0).First();
            tabla.DeleteObject(condicionActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, condicionActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region CONDICIONAGRUPADA

        public List<CondicionAgrupada> ListarCondicionAgrupada()
        {
            return tabla.CondicionAgrupadas.Include("SubRegla").Include("SubRegla1").Include("Operador").Include("Regla.Concurso").Where(CondicionAgrupada => CondicionAgrupada.id > 0).ToList();
        }

        public List<CondicionAgrupada> ListarCondicionAgrupadaPorId(int id)
        {
            return tabla.CondicionAgrupadas.Include("SubRegla").Include("SubRegla1").Include("Operador").Include("Regla.Concurso").Where(CondicionAgrupada => CondicionAgrupada.id == id && CondicionAgrupada.id > 0).ToList();
        }

        public int InsertarCondicionAgrupada(CondicionAgrupada condicionagrupada, string Username)
        {
            int resultado = 0;

            if (tabla.CondicionAgrupadas.Where(CondicionAgrupada => condicionagrupada.subregla_id1 == condicionagrupada.subregla_id2).ToList().Count() == 0)
            {
                if (tabla.CondicionAgrupadas.Where(CondicionAgrupada => ((CondicionAgrupada.subregla_id1 == condicionagrupada.subregla_id1 && CondicionAgrupada.subregla_id2 == condicionagrupada.subregla_id2)
                    || (CondicionAgrupada.subregla_id2 == condicionagrupada.subregla_id1 && CondicionAgrupada.subregla_id1 == condicionagrupada.subregla_id2)) &&
                    CondicionAgrupada.operador_id == condicionagrupada.operador_id && CondicionAgrupada.regla_id == condicionagrupada.regla_id).ToList().Count() == 0)
                {
                    tabla.CondicionAgrupadas.AddObject(condicionagrupada);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.CondicionAgrupada,
    SegmentosInsercion.Personas_Y_Pymes, null, condicionagrupada);
                    tabla.SaveChanges();
                    resultado = condicionagrupada.id;
                }
            }

            return resultado;
        }

        public int ActualizarCondicionAgrupada(int id, CondicionAgrupada condicionagrupada, string Username)
        {
            int resultado = 0;

            if (tabla.CondicionAgrupadas.Where(CondicionAgrupada => (CondicionAgrupada.subregla_id1 == condicionagrupada.subregla_id1 && CondicionAgrupada.subregla_id2 == condicionagrupada.subregla_id2
                || CondicionAgrupada.subregla_id2 == condicionagrupada.subregla_id1 && CondicionAgrupada.subregla_id1 == condicionagrupada.subregla_id2) &&
                CondicionAgrupada.operador_id == condicionagrupada.operador_id && CondicionAgrupada.regla_id == condicionagrupada.regla_id
                && CondicionAgrupada.nombre == condicionagrupada.nombre).ToList().Count() == 0)
            {
                var condicionagrupadaActual = this.tabla.CondicionAgrupadas.Where(CondicionAgrupada => CondicionAgrupada.id == id && CondicionAgrupada.id > 0).First();
                var pValorAntiguo = condicionagrupadaActual;
                condicionagrupadaActual.subregla_id1 = condicionagrupada.subregla_id1;
                condicionagrupadaActual.subregla_id2 = condicionagrupada.subregla_id2;
                condicionagrupadaActual.operador_id = condicionagrupada.operador_id;
                condicionagrupadaActual.nombre = condicionagrupada.nombre;

                try
                {
                    SubRegla subreglaActual = this.tabla.SubReglas.Where(sr => sr.condicionAgrupada_id == condicionagrupadaActual.id).First();
                    subreglaActual.descripcion = condicionagrupada.nombre;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CondicionAgrupada,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, condicionagrupadaActual);
                }

                catch { }

                tabla.SaveChanges();
                resultado = condicionagrupadaActual.id;
            }
            return resultado;
        }

        public List<Condicion> ListarCondicionPorSubRegla(int idSubRegla)
        {
            return tabla.Condicions.Where(Condicion => Condicion.subregla_id == idSubRegla && Condicion.id > 0).OrderBy(e => e.id).ToList();
        }

        public string EliminarCondicionAgrupada(int id, CondicionAgrupada condicionagrupada, string Username)
        {
            try
            {
                var subreglaActual = this.tabla.SubReglas.Where(sr => sr.condicionAgrupada_id == id && sr.id > 0).First();
                tabla.DeleteObject(subreglaActual);
            }

            catch { }

            var condicionagrupadaActual = this.tabla.CondicionAgrupadas.Where(CondicionAgrupada => CondicionAgrupada.id == id && CondicionAgrupada.id > 0).First();
            tabla.DeleteObject(condicionagrupadaActual);

            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.CondicionAgrupada,
    SegmentosInsercion.Personas_Y_Pymes, condicionagrupadaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        public List<Operador> ListarOperadorAgrupado()
        {
            return tabla.Operadors.Where(Operador => Operador.id > 0 && Operador.id == 11 || Operador.id == 12).ToList();
        }

        #endregion

        #region PREMIOS

        public List<Premio> ListarPremio()
        {
            return tabla.Premios.Include("Operador").Include("TipoPremio").Include("Variable").Include("UnidadMedida").Include("PremioxSubreglas").Include("VariablexPremios").Where(p => p.id > 0).ToList();
        }

        public List<Premio> ListarPremioPorId(int id)
        {
            return tabla.Premios.Include("Operador").Include("Variable").Include("TipoPremio").Include("UnidadMedida").Include("PremioxSubreglas").Include("VariablexPremios").Where(p => p.id == id && p.id > 0).ToList();
        }

        public int InsertarPremio(Premio premio, string Username)
        {
            tabla.Premios.AddObject(premio);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Premio,
    SegmentosInsercion.Personas_Y_Pymes, null, premio);
            tabla.SaveChanges();
            return 0;
        }

        public int ActualizarPremio(int id, Premio premio, string Username)
        {
            var premioActual = this.tabla.Premios.Where(p => p.id == id && p.id > 0).First();
            var pValorAntiguo = premioActual;
            premioActual.descripcion = premio.descripcion;
            premioActual.operador_id = premio.operador_id;
            premioActual.tipoPremio_id = premio.tipoPremio_id;
            premioActual.valor = premio.valor;
            premioActual.unidadmedida_id = premio.unidadmedida_id;
            premioActual.regularidad = premio.regularidad;
            premioActual.variable_id = premio.variable_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Premio,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, premioActual);
            return tabla.SaveChanges();
        }

        public string EliminarPremio(int id, Premio premio, string Username)
        {
            var premioActual = this.tabla.Premios.Where(p => p.id == id && p.id > 0).First();
            tabla.DeleteObject(premioActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Premio,
    SegmentosInsercion.Personas_Y_Pymes, premioActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region CONCEPTODESCUENTO

        public List<ConceptoDescuento> ListarConceptoDescuento()
        {
            return tabla.ConceptoDescuentoes.Include("TipoMedida").Where(cd => cd.id > 0).ToList();
        }

        public int InsertarReglaxConceptoDescuento(string conceptoDescuento, int idRegla, InfoAplicacion info, string Username)
        {
            try
            {
                var reglaxconceptodescuento = this.tabla.ReglaxConceptoDescuentoes.Where(rcd => rcd.regla_id == idRegla);
                foreach (var item in reglaxconceptodescuento)
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ReglaxConceptoDescuento,
    SegmentosInsercion.Personas_Y_Pymes, item, null);
                    tabla.DeleteObject(item);
                }
            }

            catch (Exception ex)
            {
                LoggingUtil log = new LoggingUtil();
                log.Error(ex.Message, LoggingUtil.Prioridad.Alta, Modulo.Concursos, TraceEventType.Error, info);
            }

            try
            {
                foreach (var item1 in conceptoDescuento.Split(','))
                {
                    ReglaxConceptoDescuento reglaxconceptodescuento = new ReglaxConceptoDescuento();
                    reglaxconceptodescuento.conceptoDescuento_id = int.Parse(item1);
                    reglaxconceptodescuento.regla_id = idRegla;

                    tabla.ReglaxConceptoDescuentoes.AddObject(reglaxconceptodescuento);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ReglaxConceptoDescuento,
    SegmentosInsercion.Personas_Y_Pymes, null, reglaxconceptodescuento);
                }
            }

            catch (Exception ex)
            {
                LoggingUtil log = new LoggingUtil();
                log.Error(ex.Message, LoggingUtil.Prioridad.Alta, Modulo.Concursos, TraceEventType.Error, info);
            }
            return tabla.SaveChanges();
        }

        #endregion

        #region TIPOPREMIOS

        public List<TipoPremio> ListarTipoPremio()
        {
            return tabla.TipoPremios.Include("Premios").Include("UnidadMedida").Where(tp => tp.id > 0).ToList();
        }

        public List<TipoPremio> ListarTipoPremioPorId(int id)
        {
            return tabla.TipoPremios.Include("Premios").Include("UnidadMedida").Where(tp => tp.id == id && tp.id > 0).ToList();
        }

        public int InsertarTipoPremio(TipoPremio tipopremio, string Username)
        {
            tabla.TipoPremios.AddObject(tipopremio);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.TipoPremio,
    SegmentosInsercion.Personas_Y_Pymes, null, tipopremio);
            return tabla.SaveChanges();
        }

        public int ActualizarTipoPremio(int id, TipoPremio tipopremio, string Username)
        {
            var tipopremioActual = this.tabla.TipoPremios.Where(tp => tp.id == id && tp.id > 0).First();
            var pValorAntiguo = tipopremioActual;
            tipopremioActual.generapago = tipopremio.generapago;
            tipopremioActual.nombre = tipopremio.nombre;
            tipopremioActual.unidadMedida_id = tipopremio.unidadMedida_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.TipoPremio,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tipopremioActual);
            return tabla.SaveChanges();
        }

        public string EliminarTipoPremio(int id, TipoPremio tipopremio, string Username)
        {
            var tipopremioActual = this.tabla.TipoPremios.Where(tp => tp.id == id && tp.id > 0).First();
            tabla.DeleteObject(tipopremioActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.TipoPremio,
    SegmentosInsercion.Personas_Y_Pymes, tipopremioActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region UNIDADMEDIDA

        public List<UnidadMedida> ListarUnidadMedida()
        {
            return tabla.UnidadMedidas.Include("TipoUnidadMedida").Where(u => u.id > 0).ToList();
        }

        public List<UnidadMedida> ListaUnidadMedidaPorId(int id)
        {
            return tabla.UnidadMedidas.Include("TipoUnidadMedida").Where(u => u.id == id && u.id > 0).ToList();
        }

        public int InsertarUnidadMedida(UnidadMedida unidadmedida, string Username)
        {
            tabla.UnidadMedidas.AddObject(unidadmedida);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.UnidadMedida,
    SegmentosInsercion.Personas_Y_Pymes, null, unidadmedida);
            return tabla.SaveChanges();
        }

        public int ActualizarUnidadMedida(int id, UnidadMedida unidadmedida, string Username)
        {
            var unidadmedidaActual = this.tabla.UnidadMedidas.Where(u => u.id == id && u.id > 0).First();
            var pValorAntiguo = unidadmedidaActual;
            unidadmedidaActual.nombre = unidadmedida.nombre;
            unidadmedidaActual.tipounidadmedida_id = unidadmedida.tipounidadmedida_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, unidadmedidaActual);
            return tabla.SaveChanges();
        }

        public string EliminarUnidadMedida(int id, UnidadMedida unidadmedida, string Username)
        {
            var unidadmedidaActual = this.tabla.UnidadMedidas.Where(u => u.id == id && u.id > 0).First();

            tabla.DeleteObject(unidadmedidaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.UnidadMedida,
    SegmentosInsercion.Personas_Y_Pymes, unidadmedidaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region TIPOUNIDADMEDIDA

        public List<TipoUnidadMedida> ListarTipoUnidadMedida()
        {
            return tabla.TipoUnidadMedidas.Where(tu => tu.id > 0).ToList();
        }

        public List<TipoUnidadMedida> ListaTipoUnidadMedidaPorId(int id)
        {
            return tabla.TipoUnidadMedidas.Where(tu => tu.id == id && tu.id > 0).ToList();
        }

        public int InsertarTipoUnidadMedida(TipoUnidadMedida tipounidadmedida, string Username)
        {
            tabla.TipoUnidadMedidas.AddObject(tipounidadmedida);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.TipoUnidadMedida,
    SegmentosInsercion.Personas_Y_Pymes, null, tipounidadmedida);
            return tabla.SaveChanges();
        }

        public int ActualizarTipoUnidadMedida(int id, TipoUnidadMedida tipounidadmedida, string Username)
        {
            var tipounidadmedidaActual = this.tabla.TipoUnidadMedidas.Where(tu => tu.id == id && tu.id > 0).First();
            var pValorAntiguo = tipounidadmedidaActual;
            tipounidadmedidaActual.nombre = tipounidadmedida.nombre;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.TipoUnidadMedida,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tipounidadmedidaActual);
            return tabla.SaveChanges();
        }

        public string EliminarTipoUnidadMedida(int id, TipoUnidadMedida tipounidadmedida, string Username)
        {
            var tipounidadmedidaActual = this.tabla.TipoUnidadMedidas.Where(tu => tu.id == id && tu.id > 0).First();
            tabla.DeleteObject(tipounidadmedidaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.TipoUnidadMedida,
    SegmentosInsercion.Personas_Y_Pymes, tipounidadmedidaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region PREMIOXSUBREGLA

        public List<PremioxSubregla> ListarPremioxSubregla()
        {
            return tabla.PremioxSubreglas.Include("Premio").Include("SubRegla").Where(pxs => pxs.id > 0).ToList();
        }

        public List<PremioxSubregla> ListaPremioxSubreglaPorId(int id)
        {
            return tabla.PremioxSubreglas.Include("Premio").Include("SubRegla").Where(pxs => pxs.id == id && pxs.id > 0).ToList();
        }

        public int InsertarPremioxSubregla(PremioxSubregla premioxsubregla, string Username)
        {
            int resultado = 0;

            if (tabla.PremioxSubreglas.Include("SubRegla").Where(PremioxSubregla => PremioxSubregla.premio_id == premioxsubregla.premio_id && PremioxSubregla.subregla_id == premioxsubregla.subregla_id).ToList().Count() == 0)
            {
                tabla.PremioxSubreglas.AddObject(premioxsubregla);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.PremioxSubregla,
    SegmentosInsercion.Personas_Y_Pymes, null, premioxsubregla);
                tabla.SaveChanges();
                resultado = premioxsubregla.id;
            }
            return resultado;
        }

        public int ActualizarPremioxSubregla(int id, PremioxSubregla premioxsubregla, string Username)
        {
            var premioxsubreglaActual = this.tabla.PremioxSubreglas.Where(pxs => pxs.id == id && pxs.id > 0).First();
            var pValorAntiguo = premioxsubreglaActual;
            premioxsubreglaActual.subregla_id = premioxsubregla.subregla_id;
            premioxsubreglaActual.premio_id = premioxsubregla.premio_id;
            premioxsubreglaActual.mesinicio = premioxsubregla.mesinicio;
            premioxsubreglaActual.mesfin = premioxsubregla.mesfin;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PremioxSubregla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, premioxsubreglaActual);
            return tabla.SaveChanges();
        }

        public string EliminarPremioxSubregla(int id, PremioxSubregla premioxsubregla, string Username)
        {
            var premioxsubreglaActual = this.tabla.PremioxSubreglas.Where(pxs => pxs.id == id && pxs.id > 0).First();
            tabla.DeleteObject(premioxsubreglaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.PremioxSubregla,
    SegmentosInsercion.Personas_Y_Pymes, premioxsubreglaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region TIPOREGLA

        public List<TipoRegla> ListarTipoRegla()
        {
            return tabla.TipoReglas.Where(TipoRegla => TipoRegla.id > 0).ToList();
        }

        #endregion

        #region PERIODOREGLA

        public List<PeriodoRegla> ListarPeriodoRegla()
        {
            return tabla.PeriodoReglas.Where(PeriodoRegla => PeriodoRegla.id > 0).ToList();
        }

        #endregion

        #region ESTRATEGIAREGLA

        public List<EstrategiaRegla> ListarEstrategiaRegla()
        {
            return tabla.EstrategiaReglas.Where(EstrategiaRegla => EstrategiaRegla.id > 0).ToList();
        }

        #endregion

        #region OPERADOR

        public List<Operador> ListarOperadorLogico()
        {
            bool logico = true;
            return tabla.Operadors.Where(Operador => Operador.id > 0 && Operador.logico == logico && Operador.id != 11 && Operador.id != 12).ToList();
        }

        public List<Operador> ListarOperadorMatematico()
        {
            bool logico = false;
            return tabla.Operadors.Where(Operador => Operador.id > 0 && Operador.logico == logico).ToList();
        }

        #endregion

        #region REGLAXCONCEPTODESCUENTO

        public List<ReglaxConceptoDescuento> ListarReglaxConceptoDescuento()
        {
            return tabla.ReglaxConceptoDescuentoes.Include("ConceptoDescuento").Include("Regla").ToList();
        }

        #endregion

        #region tabla

        public List<tabla> Listartabla()
        {
            return tabla.tablas.ToList();
        }

        #endregion

        #region OCULTAR/MOSTRAR CAMPOS

        public string variablextipovariable(string valorVariable)
        {
            string resultado = tabla.Variables.Where(v => v.tipoDato == valorVariable).ToString();
            return resultado;

        }

        #endregion

        #region VARIABLESCONTENIDAS

        public List<TempList> ListarTablas(int idtabla)
        {
            List<TempList> datos = new List<TempList>();
            Variables var = new Variables();

            int resultado = (int)tabla.Variables.Where(v => v.id == idtabla).First().tabla_id;

            try
            {
                if (resultado != 0)
                {

                    if (idtabla == 14 || idtabla == 53)
                    {
                        List<Categoria> cat = tabla.Categorias.Where(Categoria => Categoria.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Categoria item in cat)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }

                    }
                    //if (idtabla == 20)
                    //{
                    //    List<Zona> cat = tabla.Zonas.Where(z => z.id > 0).ToList();
                    //    foreach (Zona item in cat)
                    //    {
                    //        TempList temp = new TempList();
                    //        temp.id = item.id;
                    //        temp.nombre = item.nombre;
                    //        datos.Add(temp);
                    //    }
                    //}
                    //if (idtabla == 17)
                    //{
                    //    List<ModalidadPago> cat = tabla.ModalidadPagoes.Where(mp => mp.id > 0).ToList();
                    //    foreach (ModalidadPago item in cat)
                    //    {
                    //        TempList temp = new TempList();
                    //        temp.id = item.id;
                    //        temp.nombre = item.nombre;
                    //        datos.Add(temp);
                    //    }
                    //}
                    //if (idtabla == 16)
                    //{
                    //    List<Red> cat = tabla.Reds.Where(r => r.id > 0).ToList();
                    //    foreach (Red item in cat)
                    //    {
                    //        TempList temp = new TempList();
                    //        temp.id = item.id;
                    //        temp.nombre = item.nombre;
                    //        datos.Add(temp);
                    //    }
                    //}

                    if (idtabla == 39)
                    {
                        List<Localidad> Loc = tabla.Localidads.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Localidad item in Loc)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 60)
                    {
                        List<Ramo> Ram = tabla.Ramoes.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Ramo item in Ram)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 58 || idtabla == 153)
                    {
                        List<Nivel> Niv = tabla.Nivels.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Nivel item in Niv)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 55)
                    {
                        List<Producto> Pro = tabla.Productoes.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Producto item in Pro)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 8)
                    {
                        List<EstadoNegocio> EstN = tabla.EstadoNegocios.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (EstadoNegocio item in EstN)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    //if (idtabla == 64)
                    //{
                    //    List<EtapaProducto> EtP = tabla.EtapaProductoes.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                    //    foreach (EtapaProducto item in EtP)
                    //    {
                    //        TempList temp = new TempList();
                    //        temp.id = item.id;
                    //        temp.nombre = item.nombre;
                    //        datos.Add(temp);
                    //    }
                    //}

                    if (idtabla == 67)
                    {
                        List<Compania> C = tabla.Companias.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Compania item in C)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 139)
                    {
                        List<Meta> M = tabla.Metas.Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Meta item in M)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 161)
                    {
                        List<Canal> Canal = tabla.Canals.Where(canal => canal.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Canal item in Canal)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 169)
                    {
                        List<LineaNegocio> LineaNegocio = tabla.LineaNegocios.Where(ln => ln.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (LineaNegocio item in LineaNegocio)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }

                    if (idtabla == 190)
                    {
                        List<Amparo> Amparo = tabla.Amparoes.Where(a => a.id > 0).OrderBy(e => e.nombre).ToList();
                        foreach (Amparo item in Amparo)
                        {
                            TempList temp = new TempList();
                            temp.id = item.id;
                            temp.nombre = item.nombre;
                            datos.Add(temp);
                        }
                    }
                }
            }
            catch
            {
                datos[0].resultado = resultado;
                return null;
            }

            return datos;
        }
        #endregion
    }
}
