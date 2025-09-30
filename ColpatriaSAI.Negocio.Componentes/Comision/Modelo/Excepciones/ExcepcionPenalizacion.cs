using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Validadores.ModeloComision;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Modelo.Excepciones
{
    public class ExcepcionPenalizacion
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public ResultadoOperacionBD InsertarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion dbobj, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var query = _dbcontext.ExcepcionPenalizacions.Where(x =>
                    x.aplica == dbobj.aplica
                    && x.modelo_id == dbobj.modelo_id
                    && x.numerocontrato.Trim() == dbobj.numerocontrato.Trim()
                    && x.participanteorigen_id == dbobj.participanteorigen_id
                    && x.participantedestino_id == dbobj.participantedestino_id
                    );
                if (query.Count() == 0)
                {

                    var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == dbobj.modelo_id).First();
                    _dbcontext.ExcepcionPenalizacions.AddObject(dbobj);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ExcepcionPenalizacion,
SegmentosInsercion.Personas_Y_Pymes, null, dbobj);
                    result.RegistrosAfectados = 1;
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede agregar la excepción. Ya existe una penalización definida de esta manera dentro del modelo";
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        public ResultadoOperacionBD ActualizarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var query = _dbcontext.ExcepcionPenalizacions.Where(x =>
                  x.aplica == objetoDB.aplica
                  && x.modelo_id == objetoDB.modelo_id
                  && x.numerocontrato.Trim() == objetoDB.numerocontrato.Trim()
                  && x.participanteorigen_id == objetoDB.participanteorigen_id
                  && x.participantedestino_id == objetoDB.participantedestino_id
                  );
                if (query.Count() == 0)
                {
                    query = _dbcontext
                                        .ExcepcionPenalizacions
                                        .Where(x =>
                                                x.id == objetoDB.id
                                                && x.modelo_id == objetoDB.modelo_id
                                                );
                    var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == objetoDB.modelo_id).First();
                    if (!ValidadorModeloComision.EditableSegunVigenciaModelo(querymodel.fechadesde))
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                    }
                    else
                    {
                        if (query.Count() != 1)
                        {
                            result.RegistrosAfectados = 0;
                            result.Resultado = ResultadoOperacion.Error;
                            result.MensajeError = "Excepción Penalización no existente.";
                        }
                        else
                        {
                            var tmpobj = query.First();
                            var pValorAntiguo = tmpobj;
                            tmpobj.numerocontrato = objetoDB.numerocontrato;
                            tmpobj.participanteorigen_id = objetoDB.participanteorigen_id;
                            tmpobj.participantedestino_id = objetoDB.participantedestino_id;
                            tmpobj.aplica = objetoDB.aplica;

                            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionPenalizacion,
 SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tmpobj);

                            result.RegistrosAfectados = 1;
                            result.Resultado = ResultadoOperacion.Exito;
                            _dbcontext.SaveChanges();
                        }
                    }
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar la penalización. Ya existe una penalización definida de esta manera dentro del modelo";
                }

            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        public ResultadoOperacionBD EliminarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion objetoDb, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == objetoDb.modelo_id).First();
                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(querymodel.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else
                {

                    objetoDb = _dbcontext.ExcepcionPenalizacions.FirstOrDefault(exp => exp.id == objetoDb.id);
                    _dbcontext.ExcepcionPenalizacions.DeleteObject(objetoDb);

                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ExcepcionPenalizacion,
SegmentosInsercion.Personas_Y_Pymes, objetoDb, null);
                    result.RegistrosAfectados = 1;
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Metodo para registro de las penalizaciones
        /// </summary>
        /// <param name="Penalizaciones">Listado de penalizaciones</param>
        /// <returns></returns>
        public List<_ExcepcionPenalizacion> CargueExcepcionPenalizacion(List<_ExcepcionPenalizacion> Penalizaciones)
        {
            RemoverLog();
            int ContInsertado = 0;
            try
            {

                List<_ExcepcionPenalizacion> lisPenalizacion = new List<_ExcepcionPenalizacion>();
                List<_ExcepcionPenalizacion> _inserPenalizacion = new List<_ExcepcionPenalizacion>();
                List<_ExcepcionPenalizacion> _Existentes = new List<_ExcepcionPenalizacion>();
                ///Se genera la lista de penalizaciones sin datos repetidos
                foreach (_ExcepcionPenalizacion item in Penalizaciones)
                {
                    if (lisPenalizacion.Count == 0)
                    {
                        lisPenalizacion.Add(item);
                    }
                    else
                    {
                        var data = lisPenalizacion.Where(h => h.NUMERO_CONTRATO == item.NUMERO_CONTRATO &&
                            h.CLAVE_ORIGEN == item.CLAVE_ORIGEN && h.MODELO == item.MODELO &&
                            h.CLAVE_DESTINO == item.CLAVE_DESTINO).FirstOrDefault();

                        if (data == null)
                        {
                            lisPenalizacion.Add(item);
                        }
                        else
                        {
                            item.DESCRIPCION_ERROR = "Dato Duplicado";
                            registroLog(item,"E");
                        }

                    }
                }

                /// se realiza la validacion 
                ///que existan las claves asignadas para realizar la extraccion del id para el asesor
                foreach (var origen in lisPenalizacion)
                {
                    var execionError = lisPenalizacion.Where(t => 
                        t.EXCEPCION == origen.EXCEPCION).ToList();
                    var  NO= lisPenalizacion.Where(t => 
                        t.EXCEPCION == origen.EXCEPCION).ToList();
                    var CO = lisPenalizacion.Where(t =>
                        t.EXCEPCION == origen.EXCEPCION).ToList();
                    if (execionError[0].EXCEPCION == "Execepcion no existente")
                    {
                        origen.DESCRIPCION_ERROR = "Excepcion no existente";
                        registroLog(origen,"E");
                    }
                    else if (NO[0].DESCRIPCION_ERROR == "NO")
                    {
                        origen.DESCRIPCION_ERROR = "El número de contratro contiene caracteres que no son númericos";
                        registroLog(origen, "E");
                    }
                    else if (CO[0].DESCRIPCION_ERROR == "CO")
                    {
                        origen.DESCRIPCION_ERROR = "la clave origen es igual a la clave destino";
                        registroLog(origen, "E");
                    }
                    else
                    {

                        int id = IdAsesor(origen.CLAVE_ORIGEN);
                        if (id != 0)
                        {
                            origen.CLAVE_ORIGEN = id;
                            int IdClavedestino = IdAsesor(origen.CLAVE_DESTINO);
                            origen.CLAVE_DESTINO = IdClavedestino;
                            _inserPenalizacion.Add(origen);

                        }
                    }

                }

                if (_inserPenalizacion.Count != 0)
                {


                    foreach (_ExcepcionPenalizacion item in _inserPenalizacion)
                    {
                        Entidades.ExcepcionPenalizacion _data = new Entidades.ExcepcionPenalizacion();
                        ///se valida que el registro no exista en la base de datos
                        var _consult = _dbcontext.ExcepcionPenalizacions.Where(e =>
                            e.numerocontrato == item.NUMERO_CONTRATO &&
                            e.modelo_id == item.MODELO &&
                            e.participanteorigen_id == item.CLAVE_ORIGEN &&
                            e.participantedestino_id == item.CLAVE_DESTINO).FirstOrDefault();

                        if (_consult == null)
                        {
                             
                            _data.numerocontrato = item.NUMERO_CONTRATO;
                            _data.participanteorigen_id = item.CLAVE_ORIGEN;
                            _data.participantedestino_id = item.CLAVE_DESTINO;
                            _data.aplica = item.EXCEPCION;
                            _data.modelo_id = item.MODELO;
                            _data.activo = true;
                            _dbcontext.ExcepcionPenalizacions.AddObject(_data);
                            item.DESCRIPCION_ERROR = "Registro Exitoso";
                            registroLog(item,"C");

                             ContInsertado = ContInsertado + 1;
                        }
                        else
                        {
                            ////dato existente
                            item.DESCRIPCION_ERROR = "Registro existente";
                            registroLog(item,"C");

                        }

                    }
                    _dbcontext.SaveChanges();
                }
                if (ContInsertado == 0)
                {
                    return _inserPenalizacion;
                }
                else
                {
                    return _inserPenalizacion;
                };
            }
            catch (Exception)
            {

                throw;
            }
            return null;

        }

        public int RemoverLog() {
            try
            {
                 EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                 SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                 entityConn.Open();
                 entityConn.CreateCommand();
                 string sql = "delete from logErroresExcepcionesPenalizacion";
                 SqlCommand cmd = new SqlCommand(sql, sqlConn);
                 cmd.ExecuteNonQuery();
                 entityConn.Close();
                 return 1;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public  List<_ExcepcionPenalizacion> _lisErrores(){
          try
          {

              List<_ExcepcionPenalizacion> lResult = new List<_ExcepcionPenalizacion>();
                DataSet DatasetR = new DataSet();
                EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                entityConn.Open();
                string sql = "select * from logErroresExcepcionesPenalizacion";
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                SqlDataAdapter daReport = new SqlDataAdapter(cmd);
                daReport.Fill(DatasetR);
                entityConn.Close();
                if (DatasetR.Tables[0].Rows.Count <= 0)
                {
                    return lResult;
                }

                else
                {
                    foreach (DataRow lRow in DatasetR.Tables[0].Rows)
                    {
                        _ExcepcionPenalizacion datos = new _ExcepcionPenalizacion();
                        datos.NUMERO_CONTRATO = (lRow["NUMERO_CONTRATO"] == DBNull.Value ? string.Empty : lRow["NUMERO_CONTRATO"].ToString());
                        datos.CLAVE_ORIGEN = (lRow["CLAVE_ORIGEN"] == DBNull.Value ? 0 : int.Parse(lRow["CLAVE_ORIGEN"].ToString()));
                        datos.CLAVE_DESTINO = (lRow["CLAVE_DESTINO"] == DBNull.Value ? 0 : int.Parse(lRow["CLAVE_DESTINO"].ToString()));
                        datos.MODELO = (lRow["MODELO"] == DBNull.Value ? 0 : int.Parse(lRow["MODELO"].ToString()));
                        datos.EXCEPCION = (lRow["EXCEPCION"] == DBNull.Value ? string.Empty : lRow["EXCEPCION"].ToString());
                        datos.DESCRIPCION_ERROR = (lRow["DESCRIPCION_ERROR"] == DBNull.Value ? string.Empty : lRow["DESCRIPCION_ERROR"].ToString());
                        lResult.Add(datos);

                    }
                }
                return lResult;
      
	        }
	        catch (Exception)
	        {
		
		        throw;
	        }   
        }



        protected void registroLog(_ExcepcionPenalizacion entidad,string tab) {
            try
            {
                 string clave = "";
                 string claveD = "";
                 EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
                 SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
                 entityConn.Open();
                 entityConn.CreateCommand();
                 if (tab == "C")
                 {
                     clave = ClaveAsesor(entidad.CLAVE_ORIGEN);
                     if (clave != null)
                     {
                         claveD = ClaveAsesor(entidad.CLAVE_DESTINO);
                     }
                 }
                 else
                 {
                     clave = Convert.ToString(entidad.CLAVE_ORIGEN);
                     claveD = Convert.ToString(entidad.CLAVE_DESTINO);

                 }
                
                     string sql = "INSERT INTO logErroresExcepcionesPenalizacion(NUMERO_CONTRATO,CLAVE_ORIGEN,CLAVE_DESTINO,MODELO,EXCEPCION,DESCRIPCION_ERROR) VALUES(@param1,@param2,@param3,@param4,@param5,@param6)";
                     SqlCommand cmd = new SqlCommand(sql, sqlConn);
                     cmd.Parameters.AddWithValue("@param1", entidad.NUMERO_CONTRATO);
                     cmd.Parameters.AddWithValue("@param2", clave);
                     cmd.Parameters.AddWithValue("@param3", claveD);
                     cmd.Parameters.AddWithValue("@param4", entidad.MODELO);
                     cmd.Parameters.AddWithValue("@param5", entidad.EXCEPCION);
                     cmd.Parameters.AddWithValue("@param6", entidad.DESCRIPCION_ERROR);
                     cmd.ExecuteNonQuery();
                     entityConn.Close();

                 
               }
            catch (Exception)
            {
                
               throw;
            }
        }

        /// <summary>
        /// Meotod para consultar el id de asesor
        /// </summary>
        /// <param name="clave">Calve asesor</param>
        /// <returns></returns>
        protected int IdAsesor(int clave)
        {

            try
            {
                string claveC = Convert.ToString(clave);
                var id = _dbcontext.Participantes.Where(c => c.clave == claveC).ToList();
                if (id.Count != 0)
                {
                    return id[0].id;
                }

                return 0;
            }
            catch (Exception)
            {

                throw;
                return 0;
            }
        }

        protected string ClaveAsesor(int id)
        {

            try
            {
                string claveC = Convert.ToString(id);
                var clave = _dbcontext.Participantes.Where(c => c.id == id).ToList();
                if (clave.Count != 0)
                {
                    return clave[0].clave;
                }

                return null;
            }
            catch (Exception)
            {

                throw;
                return null;
            }
        }

        internal ResultadoOperacionBD CambiarEstadoExcepcionPenalizacion(int excepcionId, int modeloId, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var objetoDb = _dbcontext.ExcepcionPenalizacions.FirstOrDefault(x => x.id == excepcionId && x.modelo_id == modeloId);
                objetoDb.activo = objetoDb.activo.HasValue ? !objetoDb.activo.Value : false;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
    SegmentosInsercion.Personas_Y_Pymes, objetoDb, null);
                result.RegistrosAfectados = 1;
                result.Resultado = ResultadoOperacion.Exito;
                _dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }
    }
}
