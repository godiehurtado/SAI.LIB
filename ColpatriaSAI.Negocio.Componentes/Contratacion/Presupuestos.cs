using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using LinqToExcel;
using ColpatriaSAI.Negocio.Componentes.Utilidades;
using System.IO;
using ColpatriaSAI.Negocio.Componentes.Cargue;

namespace ColpatriaSAI.Negocio.Componentes.Contratacion
{
    public class Presupuestos
    {
        private SAI_Entities tabla = new SAI_Entities();
        private string stringConexion = System.Configuration.ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
        private int pagina = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["pagina"]);
        private string rutaArchivo = System.Configuration.ConfigurationManager.AppSettings["RutaArchivosCargue"] + "Presupuesto\\";

        public List<Presupuesto> ListarPresupuestos()
        {
            return tabla.Presupuestoes.Include("Ejecucions").ToList();
        }

        public int InsertarPresupuesto(Presupuesto presupuesto, string Username)
        {
            int resultado = 0;
            List<Presupuesto> presActual = tabla.Presupuestoes.Where(p => p.fechaInicio == presupuesto.fechaInicio && p.fechaFin == presupuesto.fechaFin).ToList();
            if (presActual.Count == 0)
            {
                tabla.Presupuestoes.AddObject(presupuesto);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Presupuesto,
    SegmentosInsercion.Personas_Y_Pymes, null, presupuesto);
                tabla.SaveChanges();
                resultado = presupuesto.id;
            }
            else
            {
                var pValorAntiguo = presActual[0];
                presActual[0].fechaModificacion = DateTime.Now;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Presupuesto,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, presActual[0]);
                tabla.SaveChanges();
                resultado = tabla.Presupuestoes.Where(p => p.fechaInicio == presupuesto.fechaInicio && p.fechaFin == presupuesto.fechaFin).ToList()[0].id;
            }
            return resultado;
        }

        public int InsertarDetallePresupuesto(List<PresupuestoDetalles> detalle, int idPresupuesto, int anio, string hojaActual, int esUltimaHoja, int fila, string Username)
        {

            int resultado = 0, conteo = 1;

            using (var conexion = new SqlConnection(stringConexion))
            {
                DbCommand comando = conexion.CreateCommand();
                comando.CommandText = "Presupuesto_CargarInfo";
                comando.CommandType = CommandType.StoredProcedure;

                var table = new DataTable();
                table.Columns.Add("id", typeof(string));//    table.Columns.Add("nombreNodo", typeof(string));
                table.Columns.Add("codigoNivel", typeof(string));//   table.Columns.Add("nombre", typeof(string));
                table.Columns.Add("codigoMeta", typeof(string)); table.Columns.Add("anio", typeof(string));
                table.Columns.Add("Enero", typeof(double)); table.Columns.Add("Febrero", typeof(double)); table.Columns.Add("Marzo", typeof(double));
                table.Columns.Add("Abril", typeof(double)); table.Columns.Add("Mayo", typeof(double)); table.Columns.Add("Junio", typeof(double));
                table.Columns.Add("Julio", typeof(double)); table.Columns.Add("Agosto", typeof(double)); table.Columns.Add("Septiembre", typeof(double));
                table.Columns.Add("Octubre", typeof(double)); table.Columns.Add("Noviembre", typeof(double)); table.Columns.Add("Diciembre", typeof(double));

                foreach (var item in detalle)
                {
                    table.Rows.Add(fila + conteo, item.CodNivel, item.Codigo_meta, item.Año, item.Enero, item.Febrero, item.Marzo, item.Abril,
                        item.Mayo, item.Junio, item.Julio, item.Agosto, item.Septiembre, item.Octubre, item.Noviembre, item.Diciembre);
                    conteo++;
                }
                var pTabla = new SqlParameter("@hoja", SqlDbType.Structured);
                pTabla.TypeName = "PresupuestoDetalle";
                pTabla.Value = table;

                comando.Parameters.Add(new SqlParameter("año", anio));
                comando.Parameters.Add(new SqlParameter("presupuesto_id", idPresupuesto));
                comando.Parameters.Add(pTabla);
                comando.Parameters.Add(new SqlParameter("hojaActual", hojaActual));
                comando.Parameters.Add(new SqlParameter("esUltimaHoja", esUltimaHoja));

                bool abrirConexion = comando.Connection.State == ConnectionState.Closed;
                if (abrirConexion) { comando.Connection.Open(); comando.CommandTimeout = 1200; }

                try
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Presupuesto,
    SegmentosInsercion.Personas_Y_Pymes, null, null);
                    resultado = comando.ExecuteNonQuery();
                }
                catch { }
            }
            return resultado;
        }

        public int CalcularMetasCompuestas(int presupuesto_id)
        {
            int resultado = 0;
            using (var conexion = new SqlConnection(stringConexion))
            {
                DbCommand comando = conexion.CreateCommand();
                comando.CommandText = "CalcularMetasCompuestasPresupuesto";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(new SqlParameter("presupuestoId", presupuesto_id));

                bool abrirConexion = comando.Connection.State == ConnectionState.Closed;
                if (abrirConexion) { comando.Connection.Open(); comando.CommandTimeout = 1200; }

                try { resultado = comando.ExecuteNonQuery(); }
                catch { }
                finally { if (abrirConexion && comando.Connection.State == ConnectionState.Open) { comando.Connection.Close(); } }
            }
            return resultado;
        }

        public int CalcularMetasAcumuladas(int presupuesto_id)
        {
            int resultado = 0;
            using (var conexion = new SqlConnection(stringConexion))
            {
                DbCommand comando = conexion.CreateCommand();
                comando.CommandText = "CalcularMetasAcumuladasPresupuesto";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(new SqlParameter("presupuestoId", presupuesto_id));

                bool abrirConexion = comando.Connection.State == ConnectionState.Closed;
                if (abrirConexion) { comando.Connection.Open(); comando.CommandTimeout = 1200; }

                try { resultado = comando.ExecuteNonQuery(); }
                catch { }
                finally { if (abrirConexion && comando.Connection.State == ConnectionState.Open) { comando.Connection.Close(); } }
            }
            return resultado;
        }

        public int CalcularEjecucionPresupuesto(int presupuesto_id)
        {
            int resultado = 0;
            using (var conexion = new SqlConnection(stringConexion))
            {
                DbCommand comando = conexion.CreateCommand();
                comando.CommandText = "CalcularEjecucionPresupuesto";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(new SqlParameter("presupuesto_id", presupuesto_id));

                bool abrirConexion = comando.Connection.State == ConnectionState.Closed;
                if (abrirConexion) { comando.Connection.Open(); comando.CommandTimeout = 1200; }

                try { resultado = comando.ExecuteNonQuery(); }
                catch { }
                finally { if (abrirConexion && comando.Connection.State == ConnectionState.Open) { comando.Connection.Close(); } }
            }
            return resultado;
        }

        public List<DetallePresupuesto> ListarDetallePresupuestoPorId(int id)
        {
            return tabla.DetallePresupuestoes.Where(l => l.presupuesto_id == id).ToList();
        }

        public string EliminarPresupuesto(int id, string Username)
        {
            if (id != 0)
            {
                var presupuestoActual = this.tabla.Presupuestoes.Where(p => p.id == id);
                if (presupuestoActual.Count() > 0) tabla.DeleteObject(this.tabla.Presupuestoes.Single(p => p.id == id));
                try
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Presupuesto,
    SegmentosInsercion.Personas_Y_Pymes, presupuestoActual, null);
                    return tabla.SaveChanges().ToString();
                }
                catch (Exception ex) { return ex.Message; }
            }
            return "";
        }

        public int BorrarPresupuestoACargar(int anio, int segmento_id, string Username)
        {
            int resultado = 0;
            using (var conexion = new SqlConnection(stringConexion))
            {
                DbCommand comando = conexion.CreateCommand();
                comando.CommandText = "Presupuesto_BorrarInfo";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(new SqlParameter("año", anio));
                comando.Parameters.Add(new SqlParameter("segmento_id", segmento_id));

                bool abrirConexion = comando.Connection.State == ConnectionState.Closed;
                if (abrirConexion) { comando.Connection.Open(); comando.CommandTimeout = 1200; }

                try
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Presupuesto,
    SegmentosInsercion.Personas_Y_Pymes, null, null);
                    resultado = comando.ExecuteNonQuery();
                }
                catch { }
                finally { if (abrirConexion && comando.Connection.State == ConnectionState.Open) { comando.Connection.Close(); } }
            }
            return resultado;
        }

        public int ExistePresupuesto(DetallePresupuesto detalle)
        {
            return tabla.DetallePresupuestoes.Where(d => d.meta_id == detalle.meta_id).Count();
        }

        public List<PresupuestoDetalle> ListarPresupuestoDetalle(int id)
        {
            //List<PresupuestoDetalle> detalle = tabla.PresupuestoDetalles.Where(p => p.presupuesto_id == id).ToList();
            //List<PresupuestoDetalle> detalle = (from d in tabla.PresupuestoDetalles where (d.presupuesto_id == id) select d).ToList();
            tabla.CommandTimeout = 200;
            return tabla.PresupuestoDetalles.Where(p => p.presupuesto_id == id).ToList();
        }

        public void CargarPresupuesto(string nombreArchivo, string anio, int segmento_id, string Username)
        {
            var rutaLocal = Path.Combine(rutaArchivo, nombreArchivo);

            if (FtpUtil.descargarArchivo(rutaArchivo, nombreArchivo))
            {
                FtpUtil.eliminarArchivo(rutaArchivo, nombreArchivo);

                List<PresupuestoDetalles> temp = new List<PresupuestoDetalles>();
                List<PresupuestoDetalles> temp1 = new List<PresupuestoDetalles>();
                List<string> hojas = new List<string>();
                int ultimaHoja = 0, paginado = 0, idPres = 0, conteo = 0;
                var excel = new ExcelQueryFactory(rutaLocal);
                excel.StrictMapping = false;

                try { hojas = excel.GetWorksheetNames().ToList<string>(); }
                catch (Exception ex) { generarLog(null, idPres); }

                // Crea el encabezado del presupuesto
                Presupuesto presupuesto = new Presupuesto()
                {
                    fechaInicio = Convert.ToDateTime("01/01/" + anio, new CultureInfo("es-CO")),
                    fechaFin = Convert.ToDateTime("31/12/" + anio, new CultureInfo("es-CO")),
                    fechaModificacion = DateTime.Now,
                    segmento_id = segmento_id
                };
                try { idPres = InsertarPresupuesto(presupuesto, Username); }
                catch
                {
                    EliminarPresupuesto(idPres, Username);
                    generarLog(null, idPres);
                }

                ProcesoLiquidacion proceso = new ProcesoLiquidacion()
                {
                    tipo = 5,
                    liquidacion_id = idPres,
                    fechaInicio = DateTime.Now,
                    estadoProceso_id = 18
                };
                int proceso_id = Proceso.registrarProceso(proceso);

                try
                {
                    foreach (var hoja in hojas)
                    { // Recorre cada una de las hojas del archivo
                        if (!hoja.Contains("xlnm"))
                        {
                            temp.Clear();
                            try { temp = (from pre in excel.Worksheet<PresupuestoDetalles>(hoja) select pre).ToList(); }
                            catch
                            {
                                LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Hoja '" + hoja + "', La hoja no tiene el formato correcto!", cargue_id = idPres, cargue_tipo = 1 });
                            }

                            if ((temp.Count >= 1))
                            {
                                if (idPres != 0)
                                {
                                    if (hoja == hojas.Last()) ultimaHoja = 1;
                                    while (paginado <= temp.Count)
                                    {
                                        temp1 = temp.Skip(paginado).Take(pagina).ToList();
                                        if (temp1.Count > 0)
                                        {
                                            InsertarDetallePresupuesto(temp1, idPres, Convert.ToInt32(anio), hoja, ultimaHoja, paginado, Username);
                                        }
                                        paginado += pagina;
                                    }
                                    paginado = 0;
                                }
                            }
                            else if (!(temp.Count >= 1))
                            {
                                LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Hoja '" + hoja + "', La hoja no tiene presupuestos para cargar!", cargue_id = idPres, cargue_tipo = 1 });
                            }
                            else
                            {
                                LogCargues.InsertarLogCargue(new LogCargue() { descripcion = "Hoja '" + hoja + "', La hoja no tiene el formato correcto!", cargue_id = idPres, cargue_tipo = 1 });
                            }
                            conteo += temp.Count;
                        }
                    }
                }
                catch { EliminarPresupuesto(idPres, Username); }
                File.Delete(rutaLocal);
                Proceso.eliminarProceso(proceso_id);
            }
        }

        public void generarLog(int? total, int idPres)
        {
            List<LogCargue> logDetalles = LogCargues.ListarLogCargue(idPres, 1);
            int? Cargados = total - logDetalles.Count;
            if (Cargados == 0) EliminarPresupuesto(idPres, string.Empty);
        }
    }
}