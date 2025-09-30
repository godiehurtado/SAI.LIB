using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using ColpatriaSAI.Negocio.Componentes.Concursos;
using ColpatriaSAI.Negocio.Componentes.Admin;
using ColpatriaSAI.Negocio.Componentes.Contratacion;
using LinqToExcel;
using System.IO;
using ColpatriaSAI.Negocio.Componentes.Utilidades;
using ColpatriaSAI.Negocio.Componentes.Cargue;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class EjecucionPresupuesto
    {
        private SAI_Entities contexto = new SAI_Entities();
        private string stringConexion = System.Configuration.ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
        private string rutaArchivo = System.Configuration.ConfigurationManager.AppSettings["RutaArchivosCargue"] + "Ejecuciones\\";

        public EjecucionDetalle TraerEjecucionDetallePorId(int idEjecucionDetalle)
        {
            EjecucionDetalle ejecucionDetalle = contexto.EjecucionDetalles.Where(e => e.id == idEjecucionDetalle).FirstOrDefault();
            return ejecucionDetalle;
        }

        public int EliminarEjecucionDetallePorIdEjecucion(int idEjecucion)
        {
            int result;
            using (var conexion = new SqlConnection(stringConexion))
            {
                DbCommand command = conexion.CreateCommand();
                command.CommandText = "EliminarEjecucionDetalle";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("idEjecucion", idEjecucion));

                bool openingConnection = command.Connection.State == ConnectionState.Closed;

                if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 240; }

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

        public int GuardarEjecucionDetalle(EjecucionDetalle ejecucionDetalle)
        {
            contexto.EjecucionDetalles.AddObject(ejecucionDetalle);
            return contexto.SaveChanges();
        }

        public int GuardarEjecucionDetalleBatch(String strSQL)
        {
            //PARA EJECUTAR SP
            using (var conexion = new SqlConnection(stringConexion))
            {
                DbCommand command = conexion.CreateCommand();
                command.Connection.Open();

                command = conexion.CreateCommand();
                command.CommandText = strSQL;
                command.ExecuteNonQuery();
            }
            return 1;
        }

        public int ActualizarEjecucionDetalle(EjecucionDetalle ejecucionDetalle)
        {
            var ejecucion = contexto.EjecucionDetalles.Where(e => e.id == ejecucionDetalle.id).FirstOrDefault();
            ejecucion.valor = ejecucionDetalle.valor;
            ejecucion.tipo = ejecucionDetalle.tipo;
            ejecucion.periodo = ejecucionDetalle.periodo;
            ejecucion.nodo_id = ejecucionDetalle.nodo_id;
            ejecucion.meta_id = ejecucionDetalle.meta_id;
            ejecucion.canal_id = ejecucionDetalle.canal_id;
            ejecucion.descripcion = ejecucionDetalle.descripcion;
            ejecucion.fechaAjuste = ejecucionDetalle.fechaAjuste;
            ejecucion.usuario = ejecucionDetalle.usuario;
            contexto.SaveChanges();
            int resultado = ejecucion.id;
            return resultado;
        }

        public Ejecucion TraerEjecucionPorPresupuesto(int idPresupuesto)
        {
            Ejecucion ejecucion = contexto.Ejecucions.Where(e => e.presupuesto_id == idPresupuesto).FirstOrDefault();
            return ejecucion;
        }

        public EjecucionDetalle TraerEjecucionDetalle(int idEjecucion, int idMeta, int idNodo, int periodo, int idCanal)
        {
            EjecucionDetalle ejecucionDetalle = contexto.EjecucionDetalles.Where(e => e.ejecucion_id == idEjecucion && e.meta_id == idMeta && e.nodo_id == idNodo && e.canal_id == idCanal && e.periodo.Value.Month == periodo).FirstOrDefault();
            return ejecucionDetalle;
        }

        public void CargueManualEjecucionDetalle(string nombreArchivo, int idPresupuesto)
        {
            var rutaLocal = Path.Combine(rutaArchivo, nombreArchivo);

            if (FtpUtil.descargarArchivo(rutaArchivo, nombreArchivo))
            {
                FtpUtil.eliminarArchivo(rutaArchivo, nombreArchivo);

                //Obtenemos la ejecucion del presupuesto
                Ejecucion ejecucion = TraerEjecucionPorPresupuesto(idPresupuesto);

                if (ejecucion != null)
                {
                    ProcesoLiquidacion proceso = new ProcesoLiquidacion()
                    {
                        tipo = 6,
                        liquidacion_id = ejecucion.id,
                        fechaInicio = DateTime.Now,
                        estadoProceso_id = 19
                    };
                    int proceso_id = Proceso.registrarProceso(proceso);

                    //Borramos la ejecucion detalle manual de la ejecucion
                    EliminarEjecucionDetallePorIdEjecucion(ejecucion.id);

                    LogCargues.BorrarLogCargue(ejecucion.id, 2);

                    string anio = ejecucion.anio.ToString();
                    string msjError = string.Empty;

                    int nodoId = 0;
                    int canalId = 0;
                    List<CargaEjecuciones> temp = new List<CargaEjecuciones>();
                    List<Participante> participantes = new ParticipanteConcursos().ListarParticipantes().ToList();
                    List<JerarquiaDetalle> jerarquiaDetalle = new Jerarquias().ListarJerarquiaDetalle().ToList();
                    List<Meta> metas = new Metas().ListarMetas().ToList();
                    Presupuesto presupuesto = new Presupuestos().ListarPresupuestos().Where(p => p.id == idPresupuesto).First();
                    List<PresupuestoDetalle> presupuestoDetalle = new Presupuestos().ListarPresupuestoDetalle(idPresupuesto);
                    List<MetaxNodo> metasxNodos = new MetasNodos().ListarMetasNodos();
                    List<EjecucionDetalle> listEjecucionDetalle = new List<EjecucionDetalle>();

                    int registrosProcesados = 0; int registrosError = 0; int registro = 1; int registroprueba=0;
                    Boolean error = false;
                    var excel = new ExcelQueryFactory(rutaLocal);
                    excel.StrictMapping = false;

                    string codNivelTemp = string.Empty;
                    string hojaTemp = string.Empty;
                    try
                    {
                        List<string> hojasCargue = excel.GetWorksheetNames().ToList();

                        foreach (string hoja in hojasCargue)
                        {
                            if (!hoja.Contains("_xlnm"))
                            {
                                hojaTemp = hoja;
                                temp = (from pre in excel.Worksheet<CargaEjecuciones>(hoja) select pre).ToList();
                                registro = 0;

                                foreach (CargaEjecuciones cargaEjecuciones in temp)
                                {
                                    registroprueba++;
                                    error = false;
                                    nodoId = 0;
                                    canalId = 0;
                                    codNivelTemp = cargaEjecuciones.CodigoNivel;

                                    if (codNivelTemp == "GTEPAL")
                                    {
                                        string algo = "aqui";
                                    }

                                    //VALIDAMOS SI EL CODIGO NIVEL EXISTE EN LA JERARQUIA
                                    var jerarquiaDetalleTotal = jerarquiaDetalle.Where(x => x.codigoNivel == cargaEjecuciones.CodigoNivel).Count();
                                    if (jerarquiaDetalleTotal == 0)
                                    {
                                        registrosError++;
                                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 104: En Registro " + cargaEjecuciones.CodigoNivel + " Hoja: " + hoja, cargue_id = ejecucion.id, cargue_tipo = 2 });
                                        error = true;
                                    }
                                    else
                                    {
                                        nodoId = jerarquiaDetalle.Where(x => x.codigoNivel == cargaEjecuciones.CodigoNivel).First().id;
                                        canalId = jerarquiaDetalle.Where(x => x.codigoNivel == cargaEjecuciones.CodigoNivel).First().canal_id;
                                    }

                                    //VALIDAMOS QUE LA META TENGA PRESUPUESTO
                                    var metasPresupuestoTotal = presupuestoDetalle.Where(x => x.meta_id == cargaEjecuciones.MetaId).ToList().Count();
                                    if (metasPresupuestoTotal == 0)
                                    {
                                        registrosError++;
                                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 105: En Registro " + cargaEjecuciones.CodigoNivel + " Hoja: " + hoja, cargue_id = ejecucion.id, cargue_tipo = 2 });
                                        error = true;
                                    }
                                    else
                                    {

                                        //VALIDAMOS EL ID DE JERARQUIA TIENE ASOCIADAS LA META
                                        if (nodoId != 0)
                                        {
                                            var metasNodoTotal = metasxNodos.Where(x => x.meta_id == cargaEjecuciones.MetaId && x.anio == Convert.ToInt32(anio)).Count();
                                            if (metasNodoTotal == 0)
                                            {
                                                registrosError++;
                                                LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 101: En Registro " + cargaEjecuciones.CodigoNivel + " Hoja: " + hoja, cargue_id = ejecucion.id, cargue_tipo = 2 });
                                                error = true;
                                            }
                                        }

                                        //VALIDAMOS QUE LA META EXISTA
                                        var metasTotal = metas.Where(x => x.id == cargaEjecuciones.MetaId).ToList().Count();
                                        if (metasTotal == 0)
                                        {
                                            registrosError++;
                                            LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 102: En Registro " + cargaEjecuciones.CodigoNivel + " Hoja: " + hoja, cargue_id = ejecucion.id, cargue_tipo = 2 });
                                            error = true;
                                        }

                                        //VALIDAMOS QUE LA META CARGADA SEA MANUAL
                                        var metasManualTotal = metas.Where(x => x.id == cargaEjecuciones.MetaId && x.automatica == false).ToList().Count();
                                        if (metasManualTotal == 0 && metasTotal > 0)
                                        {
                                            registrosError++;
                                            LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 103: En Registro " + cargaEjecuciones.CodigoNivel + " Hoja: " + hoja, cargue_id = ejecucion.id, cargue_tipo = 2 });
                                            error = true;
                                        }
                                    }

                                    if (!error)
                                    {
                                        //RECORREMOS TODOS LOS MESES
                                        int mes = 1;
                                        var periodo = string.Empty;
                                        String valor = string.Empty;
                                        while (mes <= 12)
                                        {
                                            periodo = anio + "-" + mes.ToString() + "-01";

                                            if (mes == 1 && cargaEjecuciones.Enero != null && cargaEjecuciones.Enero.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Enero.ToString().Trim();
                                            else if (mes == 2 && cargaEjecuciones.Febrero != null && cargaEjecuciones.Febrero.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Febrero.ToString().Trim();
                                            else if (mes == 3 && cargaEjecuciones.Marzo != null && cargaEjecuciones.Marzo.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Marzo.ToString().Trim();
                                            else if (mes == 4 && cargaEjecuciones.Abril != null && cargaEjecuciones.Abril.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Abril.ToString().Trim();
                                            else if (mes == 5 && cargaEjecuciones.Mayo != null && cargaEjecuciones.Mayo.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Mayo.ToString().Trim();
                                            else if (mes == 6 && cargaEjecuciones.Junio != null && cargaEjecuciones.Junio.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Junio.ToString().Trim();
                                            else if (mes == 7 && cargaEjecuciones.Julio != null && cargaEjecuciones.Julio.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Julio.ToString().Trim();
                                            else if (mes == 8 && cargaEjecuciones.Agosto != null && cargaEjecuciones.Agosto.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Agosto.ToString().Trim();
                                            else if (mes == 9 && cargaEjecuciones.Septiembre != null && cargaEjecuciones.Septiembre.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Septiembre.ToString().Trim();
                                            else if (mes == 10 && cargaEjecuciones.Octubre != null && cargaEjecuciones.Octubre.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Octubre.ToString().Trim();
                                            else if (mes == 11 && cargaEjecuciones.Noviembre != null && cargaEjecuciones.Noviembre.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Noviembre.ToString().Trim();
                                            else if (mes == 12 && cargaEjecuciones.Diciembre != null && cargaEjecuciones.Diciembre.Trim() != string.Empty)
                                                valor = cargaEjecuciones.Diciembre.ToString().Trim();

                                            //DETERMINAMOS SI EN VALOR ES VACIO O TIENE LA COLUMNA NA
                                            if (valor != string.Empty && valor != "NA" && valor != null && valor != "-")
                                            {
                                                //valor = valor.Replace('.', ',');

                                                //GUARDAMOS EL REGISTRO    
                                                EjecucionDetalle ejecucionDetalle = new EjecucionDetalle
                                                {
                                                    ejecucion_id = ejecucion.id,
                                                    meta_id = cargaEjecuciones.MetaId,
                                                    nodo_id = nodoId,
                                                    periodo = Convert.ToDateTime(periodo),
                                                    valor = float.Parse(valor),
                                                    tipo = 3,
                                                    canal_id = canalId
                                                };
                                                valor = string.Empty;
                                                listEjecucionDetalle.Add(ejecucionDetalle);
                                            }

                                            mes++;
                                        }

                                        registrosProcesados++;
                                    }
                                    registro++;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        registrosError++;
                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error: Hoja: " + hojaTemp + " - Codigo Nivel " + codNivelTemp + " - " + ex.Message, cargue_id = ejecucion.id, cargue_tipo = 2 });
                    }

                    //CARGAMOS LOS DATOS DEL ARCHIVO A PARTIR DE LA LISTA TEMPORAL
                    if (listEjecucionDetalle.Count() > 0)
                    {
                        int totalRegister = 1;
                        int maxRegisterExecuteSQL = 150;
                        string strSQL = string.Empty;
                        foreach (EjecucionDetalle ejecucionDetalle in listEjecucionDetalle)
                        {
                            //GUARDAMOS LA SENTENCIA SQL
                            string periodo = ejecucionDetalle.periodo.Value.Year + "-" + ejecucionDetalle.periodo.Value.Month + "-" + ejecucionDetalle.periodo.Value.Day;
                            strSQL += "(" + ejecucionDetalle.ejecucion_id.ToString() + "," + ejecucionDetalle.meta_id.ToString() + "," + ejecucionDetalle.nodo_id.ToString() + ",'" + periodo + "'," + ejecucionDetalle.valor.ToString().Replace(',', '.') + "," + ejecucionDetalle.tipo.ToString() + "," + ejecucionDetalle.canal_id.ToString() + "),";

                            //EJECUTAMOS LA CONSULTA
                            if (totalRegister == maxRegisterExecuteSQL)
                            {
                                strSQL = strSQL.Substring(0, strSQL.Length - 1).Trim();
                                strSQL = "INSERT INTO EjecucionDetalle(ejecucion_id, meta_id, nodo_id, periodo, valor, tipo, canal_id) VALUES " + strSQL;
                                GuardarEjecucionDetalleBatch(strSQL);
                                totalRegister = 0;
                                strSQL = string.Empty;
                            }

                            totalRegister++;
                        }

                        if (totalRegister > 0 && !string.IsNullOrEmpty(strSQL))
                        {
                            strSQL = strSQL.Substring(0, strSQL.Length - 1).Trim();
                            strSQL = "INSERT INTO EjecucionDetalle(ejecucion_id, meta_id, nodo_id, periodo, valor, tipo, canal_id) VALUES " + strSQL;
                            GuardarEjecucionDetalleBatch(strSQL);
                        }
                    }

                    LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Nodos Procesados: " + (registro - 1), cargue_id = ejecucion.id, cargue_tipo = 2 });
                    LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Registros Insertados con éxito: " + listEjecucionDetalle.Count(), cargue_id = ejecucion.id, cargue_tipo = 2 });
                    LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Registros Procesados con error: " + registrosError, cargue_id = ejecucion.id, cargue_tipo = 2 });
                    if (registrosError > 0)
                    {
                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 101: El codigo nivel no tiene asociada la meta para el año(" + anio + ") de ejecucion del presupuesto.", cargue_id = ejecucion.id, cargue_tipo = 2 });
                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 102: La meta no existe en la BD.", cargue_id = ejecucion.id, cargue_tipo = 2 });
                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 103: La meta esta configurada para calcularse de forma automatica y no para ser cargada de forma manual.", cargue_id = ejecucion.id, cargue_tipo = 2 });
                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 104: El codigo nivel no existe en ninguna jerarquia.", cargue_id = ejecucion.id, cargue_tipo = 2 });
                        LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Error 105: Meta no tiene presupuesto asignado.", cargue_id = ejecucion.id, cargue_tipo = 2 });
                    }
                    File.Delete(rutaLocal);
                    Proceso.eliminarProceso(proceso_id);
                }
            }
        }
    }

    public class CargaEjecuciones
    {
        public String CodigoNivel { get; set; }
        public String NombreNodo { get; set; }
        public Int32 MetaId { get; set; }
        public String Enero { get; set; }
        public String Febrero { get; set; }
        public String Marzo { get; set; }
        public String Abril { get; set; }
        public String Mayo { get; set; }
        public String Junio { get; set; }
        public String Julio { get; set; }
        public String Agosto { get; set; }
        public String Septiembre { get; set; }
        public String Octubre { get; set; }
        public String Noviembre { get; set; }
        public String Diciembre { get; set; }
    }
}