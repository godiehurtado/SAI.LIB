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
using ColpatriaSAI.Negocio.Componentes.Concursos;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Concursoes
    {
        private SAI_Entities tabla = new SAI_Entities();


        public List<Concurso> ListarConcursoes()
        {
            return tabla.Concursoes.Include("TipoConcurso").Include("Segmento").Where(Concurso => Concurso.id > 0).ToList();

        }

        public List<Concurso> ListarConcursoesPorId(int id)
        {
            return tabla.Concursoes.Include("TipoConcurso").Include("Segmento").Where(Concurso => Concurso.id == id && Concurso.id > 0).ToList();
        }

        public int InsertarConcurso(Concurso concurso, string Username)
        {
            int resultado = 0;
            if (tabla.Concursoes.Where(Concurso => Concurso.nombre == concurso.nombre
                && Concurso.fecha_inicio == concurso.fecha_inicio
                && Concurso.fecha_fin == concurso.fecha_fin
                && Concurso.segmento_id == concurso.segmento_id
                && Concurso.principal == concurso.principal).ToList().Count() == 0)
            {
                tabla.Concursoes.AddObject(concurso);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Concurso,
   SegmentosInsercion.Personas_Y_Pymes, null, concurso);
                tabla.SaveChanges();
                resultado = concurso.id;
            }
            return resultado;
        }

        public int ValidarConcursoPrincipal(int tipoConcurso_id, int añofIni, int añofFin)
        {
            var q1 = (from c in tabla.Concursoes
                      where (c.principal == true && c.tipoConcurso_id == tipoConcurso_id
                      && c.fecha_inicio.Value.Year == añofIni && c.fecha_fin.Value.Year == añofFin)
                      select c).Count();
            return q1;
        }

        public string RetornarNombreSegmentoUsuario(int segmento_id)
        {
            var q1 = (from s in tabla.Segmentoes
                      where (s.id == segmento_id)
                      select s.nombre).First();
            return q1;
        }

        public int ActualizarConcurso(int id, Concurso concurso, string Username)
        {
            int resultado = 0;

            try
            {
                var concursoxvalor = tabla.Concursoes.Where(c => c.id > 0 && c.tipoConcurso_id == concurso.tipoConcurso_id && c.segmento_id == concurso.segmento_id && c.principal == true
                && c.fecha_inicio.Value.Year == concurso.fecha_inicio.Value.Year && c.fecha_fin.Value.Year == c.fecha_fin.Value.Year).ToList();
                foreach (var item in concursoxvalor)
                {
                    var siniestralidadxConcurso = tabla.SiniestralidadEsperadas.Where(s => s.concurso_id == item.id).ToList();
                    foreach (var item_s in siniestralidadxConcurso)
                    {
                        tabla.DeleteObject(item_s);
                    }

                    var persistenciaxConcurso = tabla.PersistenciaEsperadas.Where(p => p.concurso_id == item.id).ToList();
                    foreach (var item_p in persistenciaxConcurso)
                    {
                        tabla.DeleteObject(item_p);
                    }

                    item.principal = false;
                }
            }

            catch { }

            resultado = tabla.Concursoes.Where(Concurso => Concurso.nombre == concurso.nombre && Concurso.fecha_inicio == concurso.fecha_inicio && Concurso.fecha_fin == concurso.fecha_fin && Concurso.tipoConcurso_id == concurso.tipoConcurso_id && Concurso.segmento_id == concurso.segmento_id && Concurso.principal == concurso.principal).ToList().Count();
            if (resultado == 0)
            {
                var concursoActual = this.tabla.Concursoes.Where(Concurso => Concurso.id == id && Concurso.id > 0).First();
                var pValorAntiguo = concursoActual;
                concursoActual.nombre = concurso.nombre;
                concursoActual.fecha_inicio = concurso.fecha_inicio;
                concursoActual.fecha_fin = concurso.fecha_fin;
                concursoActual.principal = concurso.principal;
                concursoActual.descripcion = concurso.descripcion;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Concurso,
   SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, concursoActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        #region Metodos DUPLICAR CONCURSO

        public int DuplicarConcurso(int id, Concurso concurso, string Username)
        {
            int resultado = 0;
            var concursoActual = this.tabla.Concursoes.Where(Concurso => Concurso.id == id).First();

            Concurso concursoNuevo = new Concurso();

            concursoNuevo.nombre = concursoActual.nombre + " copia " + DateTime.Now.ToShortDateString();
            concursoNuevo.fecha_inicio = concurso.fecha_inicio;
            concursoNuevo.fecha_fin = concurso.fecha_fin;
            concursoNuevo.tipoConcurso_id = concursoActual.tipoConcurso_id;
            concursoNuevo.segmento_id = concursoActual.segmento_id;
            concursoNuevo.descripcion = concursoActual.descripcion;
            concursoNuevo.principal = concursoActual.principal;

            tabla.Concursoes.AddObject(concursoNuevo);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Concurso,
   SegmentosInsercion.Personas_Y_Pymes, concursoActual, concursoNuevo);
            tabla.SaveChanges();
            resultado = concursoNuevo.id;
            DuplicarParticipanteConcurso(id, resultado, Username);
            DuplicarProductoConcurso(id, resultado);

            Dictionary<int, int> idReglaNuevo = DuplicarReglasConcurso(id, resultado);

            foreach (var item in idReglaNuevo)
            {
                Dictionary<int, int> idSubRegla = DuplicarSubReglasConcurso(item);

                foreach (var item1 in idSubRegla)
                {
                    Dictionary<int, int> idCondicion = DuplicarCondicionesConcurso(item1);
                }

                foreach (var item2 in idSubRegla)
                {
                    Dictionary<int, int> idPremio = DuplicarPremiosConcurso(item2);
                }
            }
            return resultado;
        }

        public int DuplicarParticipanteConcurso(int id, int idNuevo, string Username)
        {
            int resultado = 0;
            var participanteconcursoActual = this.tabla.ParticipanteConcursoes.Where(ParticipanteConcurso => ParticipanteConcurso.concurso_id == id);

            if (participanteconcursoActual != null)
            {
                foreach (var item in participanteconcursoActual)
                {
                    ParticipanteConcurso participanteconcursoNuevo = new ParticipanteConcurso();
                    participanteconcursoNuevo.segmento_id = item.segmento_id;
                    participanteconcursoNuevo.canal_id = item.canal_id;
                    participanteconcursoNuevo.nivel_id = item.nivel_id;
                    participanteconcursoNuevo.localidad_id = item.localidad_id;
                    participanteconcursoNuevo.zona_id = item.zona_id;
                    participanteconcursoNuevo.participante_id = item.participante_id;
                    participanteconcursoNuevo.jerarquiaDetalle_id = item.jerarquiaDetalle_id;
                    participanteconcursoNuevo.categoria_id = item.categoria_id;
                    participanteconcursoNuevo.compania_id = item.compania_id;
                    participanteconcursoNuevo.concurso_id = idNuevo;

                    tabla.ParticipanteConcursoes.AddObject(participanteconcursoNuevo);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ParticipanteConcurso,
    SegmentosInsercion.Personas_Y_Pymes, participanteconcursoActual, participanteconcursoNuevo);
                }

                tabla.SaveChanges();
                return resultado;
            }
            else
                return 0;
        }

        public int DuplicarProductoConcurso(int id, int idNuevo)
        {
            int resultado = 0;
            var productoconcursoActual = this.tabla.ProductoConcursoes.Where(ProductoConcurso => ProductoConcurso.concurso_id == id);

            if (productoconcursoActual != null)
            {
                foreach (var item in productoconcursoActual)
                {
                    ProductoConcurso productoconcursoNuevo = new ProductoConcurso();
                    productoconcursoNuevo.fecha_inicio = item.fecha_inicio;
                    productoconcursoNuevo.fecha_fin = item.fecha_fin;
                    productoconcursoNuevo.compania_id = item.compania_id;
                    productoconcursoNuevo.ramo_id = item.ramo_id;
                    productoconcursoNuevo.producto_id = item.producto_id;
                    productoconcursoNuevo.lineaNegocio_id = item.lineaNegocio_id;
                    productoconcursoNuevo.concurso_id = idNuevo;

                    tabla.ProductoConcursoes.AddObject(productoconcursoNuevo);
                }
                tabla.SaveChanges();
                return resultado;
            }
            else
                return 0;
        }

        public Dictionary<int, int> DuplicarReglasConcurso(int id, int idNuevo)
        {
            Dictionary<int, int> resultadoRegla = new Dictionary<int, int>();
            var reglaconcursoActual = this.tabla.Reglas.Where(Regla => Regla.concurso_id == id);

            if (reglaconcursoActual != null)
            {
                foreach (var item in reglaconcursoActual)
                {
                    Regla reglaconcursoNuevo = new Regla();
                    reglaconcursoNuevo.nombre = item.nombre;
                    reglaconcursoNuevo.fecha_inicio = item.fecha_inicio;
                    reglaconcursoNuevo.fecha_fin = item.fecha_fin;
                    reglaconcursoNuevo.descripcion = item.descripcion;
                    reglaconcursoNuevo.tipoRegla_id = item.tipoRegla_id;
                    reglaconcursoNuevo.periodoRegla_id = item.periodoRegla_id;
                    reglaconcursoNuevo.concurso_id = idNuevo;

                    SAI_Entities tabla1 = new SAI_Entities();
                    tabla1.Reglas.AddObject(reglaconcursoNuevo);
                    tabla1.SaveChanges();
                    resultadoRegla.Add(item.id, reglaconcursoNuevo.id);
                }

                return resultadoRegla;
            }
            else
                return null;
        }

        public Dictionary<int, int> DuplicarSubReglasConcurso(KeyValuePair<int, int> ReglaTotal)
        {
            Dictionary<int, int> resultadoSubRegla = new Dictionary<int, int>();
            var subreglaconcursoActual = this.tabla.SubReglas.Where(SubRegla => SubRegla.regla_id == ReglaTotal.Key && SubRegla.condicionAgrupada_id == null && SubRegla.tipoSubregla != 2); // Subreglas Simples.

            if (subreglaconcursoActual != null)
            {
                foreach (var item in subreglaconcursoActual)
                {
                    SubRegla subreglaconcursoNuevo = new SubRegla();
                    subreglaconcursoNuevo.descripcion = item.descripcion;
                    subreglaconcursoNuevo.principal = item.principal;
                    subreglaconcursoNuevo.regla_id = ReglaTotal.Value; // Nuevo id de Regla
                    subreglaconcursoNuevo.tipoSubregla = item.tipoSubregla;

                    SAI_Entities tabla1 = new SAI_Entities();
                    tabla1.SubReglas.AddObject(subreglaconcursoNuevo);
                    tabla1.SaveChanges();
                    resultadoSubRegla.Add(item.id, subreglaconcursoNuevo.id);

                    RecorrerSubreglasAgrupadas(item.id, subreglaconcursoNuevo.id, ReglaTotal.Key, ReglaTotal.Value);
                }
                DuplicarSubreglasAgrupadas();

                return resultadoSubRegla;
            }
            else
                return null;
        }

        public int RecorrerSubreglasAgrupadas(int subreglaActual, int subreglaNueva, int reglaActual, int reglaNueva)
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

        public int DuplicarSubreglasAgrupadas()
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

        public Dictionary<int, int> DuplicarCondicionesConcurso(KeyValuePair<int, int> SubReglaTotal)
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

        public Dictionary<int, int> DuplicarPremiosConcurso(KeyValuePair<int, int> SubReglaTotal)
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

        /// <summary>
        /// Verifica si el concurso ya tiene liquidaciones. Si tiene liquidaciones devuelve 1.
        /// </summary>
        /// <param name="concurso_id">id del concurso relacionado con la regla.</param>
        /// <returns>0 si el concurso no tiene reglas liquidadas, 1 si al menos una regla tiene liquidaciones.</returns>
        public int VerificarLiquidacionConcurso(int concurso_id)
        {
            int resultado = 0;

            var reglasxconcurso = this.tabla.Reglas.Where(r => r.concurso_id == concurso_id).ToList();
            foreach (var item in reglasxconcurso)
            {
                var q1 = this.tabla.LiquidacionReglas.Where(lr => lr.regla_id == item.id).Count();
                if (q1 > 0)
                {
                    return 1;
                }
            }
            return resultado;
        }

        public void EliminarParticipantesConcurso(int concurso_id)
        {
            var participanteconcursoActual = this.tabla.ParticipanteConcursoes.Where(ParticipanteConcurso => ParticipanteConcurso.concurso_id == concurso_id).ToList();
            foreach (var item in participanteconcursoActual)
            {
                tabla.DeleteObject(item);
            }
            tabla.SaveChanges();
        }

        public void EliminarProductoConcurso(int concurso_id)
        {
            var productoconcursoActual = this.tabla.ProductoConcursoes.Where(ProductoConcurso => ProductoConcurso.concurso_id == concurso_id).ToList();
            foreach (var item in productoconcursoActual)
            {
                tabla.DeleteObject(item);
            }
            tabla.SaveChanges();
        }

        public string EliminarConcursos(int id, Concurso concurso, string Username)
        {
            int estadoEliminar = VerificarLiquidacionConcurso(id);

            if (estadoEliminar == 0)
            {
                Reglas regla = new Reglas();
                var reglasxconcurso = this.tabla.Reglas.Where(r => r.concurso_id == id).ToList();
                foreach (var item in reglasxconcurso)
                {
                    regla.EliminarRegla(item.id, null, Username);
                }
            }

            EliminarParticipantesConcurso(id);
            EliminarProductoConcurso(id);

            SAI_Entities tabla4 = new SAI_Entities();
            var concursoActual = tabla4.Concursoes.Where(Concurso => Concurso.id == id).First();
            tabla4.DeleteObject(concursoActual);

            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Concurso,
    SegmentosInsercion.Personas_Y_Pymes, concursoActual, null);
                tabla4.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }


        #region " Excepciones Generales"


        public List<ExcepcionesGenerale> ListarExcepcionesGeneralesXcompanyXTipoMedida()
        {
            return tabla.ExcepcionesGenerales.Include("Compania").Include("Ramo").Include("TipoMedida").Where(exc => exc.id > 0).ToList();

        }

        public List<ExcepcionesGenerale> ListarExcepcionesGeneralesXTipoMedida()
        {
            return tabla.ExcepcionesGenerales.Include("TipoMedida").Where(exc => exc.id > 0).ToList();

        }

        public List<ExcepcionesGenerale> ListarExcepcionesGenerales()
        {
            return tabla.ExcepcionesGenerales.Where(exc => exc.id > 0).ToList();

        }

        public bool validarExcepcionesGenerales(ExcepcionesGenerale excepcionG)
        {

            bool bandera = false;

            var lstExepcion = tabla.ExcepcionesGenerales.Where(e => e.compania_id == excepcionG.compania_id && e.ramo_id == excepcionG.ramo_id && e.clave == excepcionG.clave).Count();
            bandera = lstExepcion != 0 ? true : false;
            return bandera;
        }

        public int CrearExcepcionesGenerales(ExcepcionesGenerale excepcionG, string Username)
        {
            int resultado = 0;

            tabla.ExcepcionesGenerales.AddObject(excepcionG);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ExcepcionesGenerales,
    SegmentosInsercion.Personas_Y_Pymes, null, excepcionG);
            tabla.SaveChanges();
            resultado = excepcionG.id;
            return resultado;

        }

        public int ActualizarExcepcionesGenerales(ExcepcionesGenerale excepcionG, string Username)
        {
            int bandera = 0;

            var lstExepcion = tabla.ExcepcionesGenerales.Where(e => e.id == excepcionG.id && e.id > 0).First();

            if (lstExepcion.id != 0)
            {
                var ExcepcionGenerales = tabla.ExcepcionesGenerales.Where(e => e.id == lstExepcion.id && e.id > 0).First();
                var pValorAntiguo = ExcepcionGenerales;

                ExcepcionGenerales.fechaInicio = excepcionG.fechaInicio;
                ExcepcionGenerales.fechaFin = excepcionG.fechaFin;
                ExcepcionGenerales.numeroNegocio = excepcionG.numeroNegocio;
                ExcepcionGenerales.fechamodificacion = DateTime.Now;
                ExcepcionGenerales.ano = excepcionG.ano;
                ExcepcionGenerales.usuario = excepcionG.usuario;
                tabla.ExcepcionesGenerales.Attach(ExcepcionGenerales);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionesGenerales,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, ExcepcionGenerales);
                bandera = tabla.SaveChanges();
            }

            return bandera;
        }

        public int EliminarExcepcionGenerales(int id, string Username)
        {

            int bandera = 0;

            var count = tabla.ExcepcionesGenerales.Where(e => e.id == id).First();
            if (count.id != 0)
            {
                tabla.DeleteObject(count);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ExcepcionesGenerales,
    SegmentosInsercion.Personas_Y_Pymes, count, null);
                bandera = tabla.SaveChanges();
            }

            return bandera;

        }

    }

    public class Tipoconcursoes
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<TipoConcurso> ListarTipoconcursoes()
        {
            return tabla.TipoConcursoes.Where(TipoConcurso => TipoConcurso.id > 0 && TipoConcurso.id > 0).ToList();
        }
    }




        #endregion

}