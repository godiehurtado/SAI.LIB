using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Componentes.Utilidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class ParametrizacionCambioClaveS
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region CRUD
        /// <summary>
        /// funcion que se encarga de retornar el listado de claves modificadas
        /// </summary>
        /// <returns>Listado de Claves</returns>
        public List<clave_historico> GetHistricoCambioClave()
        {
            return tabla.clave_historico.Where(clave_historico => clave_historico.id > 0).ToList();
        }

        /// <summary>
        /// Funcion que se encarga de guardar los registros en la base de datos
        /// </summary>
        /// <param name="parametrizacionEficienciaARL">Datos para insercion</param>
        /// <returns></returns>
        public Result.result InsertParametrizacionCambiolave(clave_historico parametrizacionCambioClave, string Username)
        {
            try
            {
                ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();

                this.tabla.clave_historico.AddObject(parametrizacionCambioClave);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.clave_historico,
                      SegmentosInsercion.Personas_Y_Pymes, null, parametrizacionCambioClave);
                var temp = this.tabla.SaveChanges();

                if (temp != 0)
                {
                    result.resultado = Utilidades.Result.tipoResultado.resultOK;
                    result.message = "La tarea se ejecuto con exito";
                }
                else
                {
                    result.resultado = Utilidades.Result.tipoResultado.resultNOK;
                    result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
                ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();
                result.resultado = Utilidades.Result.tipoResultado.resultNOK;
                result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
            }
        }

        /// <summary>
        /// Funcion que se encarga de actualizar los registros en la tabla clave historico
        /// </summary>
        /// <param name="id">Identificador del registro a actualizar</param>
        /// <param name="parametrizacionEficiencia">Conjunto de datos para actualizar</param>
        /// <returns></returns>
        public Result.result UpdateParametrizacionCambiolave(int id, clave_historico parametrizacionCambioClave, string Username)
        {
            ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();
            try
            {
                var parametrizacionActual = this.tabla.clave_historico.Where(caparametrizacionCambioClave => caparametrizacionCambioClave.id == id).First();
                var pValorAntiguo = parametrizacionActual;
                parametrizacionActual.clave_old = parametrizacionCambioClave.clave_old;
                parametrizacionActual.clave_new = parametrizacionCambioClave.clave_new;
                parametrizacionActual.fecha = parametrizacionCambioClave.fecha;
                parametrizacionActual.usuario = parametrizacionCambioClave.usuario;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.clave_historico,
                      SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, parametrizacionActual);
                var temp = this.tabla.SaveChanges();
                if (temp != 0)
                {
                    result.resultado = Utilidades.Result.tipoResultado.resultOK;
                    result.message = "La tarea se ejecuto con exito";
                }
                else
                {
                    result.resultado = Utilidades.Result.tipoResultado.resultNOK;
                    result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
                }

                return result;
            }

            catch (Exception ex)
            {
                //throw ex;
                result.resultado = Utilidades.Result.tipoResultado.resultNOK;
                result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
                return result;
            }
        }

        /// <summary>
        /// Funcion que se encarga de borrar el registro que se identifica
        /// </summary>
        /// <param name="id">Identificador del registro a elimar</param>
        /// <param name="parametrizacionEficiencia">Datos a eliminar</param>
        /// <returns></returns>
        //public Result.result DeleteParametrizacionCambiolave(int id, ParametrosEficienciaARL parametrizacionCambioClave)
        //{
        //    ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();
        //    var parametrizacionActual = this.tabla.clave_historico.Where(caparametrizacionCambioClave => caparametrizacionCambioClave.id == id).First();

        //    try
        //    {
        //        this.tabla.DeleteObject(parametrizacionActual);
        //        tabla.SaveChanges();
        //        result.resultado = Utilidades.Result.tipoResultado.resultOK;
        //        result.message = "La tarea se ejecuto con exito";
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        //ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();
        //        result.resultado = Utilidades.Result.tipoResultado.resultNOK;
        //        result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
        //        return result;
        //    }

        //}

        #endregion

        #region OTRAS






        #endregion

    }
}
