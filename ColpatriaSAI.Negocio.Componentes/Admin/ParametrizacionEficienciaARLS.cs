using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Componentes.Utilidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class ParametrizacionEficienciaARLS
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region CRUD

        /// <summary>
        /// Funcion que se encarga de guardar los registros en la base de datos
        /// </summary>
        /// <param name="parametrizacionEficienciaARL">Datos para insercion</param>
        /// <returns></returns>
        //public Result.result InsertParametrizacionEficienciaARL(ParametrosEficienciaARL parametrizacionEficienciaARL)
        //{
        //    try
        //    {
        //        ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();

        //        this.tabla.ParametrosEficienciaARLs.AddObject(parametrizacionEficienciaARL);
        //        var temp = this.tabla.SaveChanges();

        //        if (temp != 0)
        //        {
        //            result.resultado = Utilidades.Result.tipoResultado.resultOK;
        //            result.message = "La tarea se ejecuto con existo";
        //        }
        //        else
        //        {
        //            result.resultado = Utilidades.Result.tipoResultado.resultNOK;
        //            result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //        ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();
        //        result.resultado = Utilidades.Result.tipoResultado.resultNOK;
        //        result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
        //    }
        //}

        /// <summary>
        /// Funcion que se encarga de actualizar los registros en la tabla Parametrizacion Eficiencia ARL
        /// </summary>
        /// <param name="id">Identificador del registro a actualizar</param>
        /// <param name="parametrizacionEficiencia">Conjunto de datos para actualizar</param>
        /// <returns></returns>
        //public Result.result UpdateParametrizacionEficienciaARL(int id, ParametrosEficienciaARL parametrizacionEficiencia)
        //{
        //    ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();
        //    try
        //    {
        //        var parametrizacionActual = this.tabla.ParametrosEficienciaARLs.Where(parametrizacionEficienciaARL => parametrizacionEficienciaARL.id == id).First();
        //        parametrizacionActual.nombreEtapa = parametrizacionEficiencia.nombreEtapa;
        //        parametrizacionActual.mesInicial = parametrizacionEficiencia.mesInicial;
        //        parametrizacionActual.mesFinal = parametrizacionEficiencia.mesFinal;
        //        parametrizacionActual.año = parametrizacionEficiencia.año;

        //        var temp = this.tabla.SaveChanges();
        //        if (temp != 0)
        //        {
        //            result.resultado = Utilidades.Result.tipoResultado.resultOK;
        //            result.message = "La tarea se ejecuto con existo";
        //        }
        //        else
        //        {
        //            result.resultado = Utilidades.Result.tipoResultado.resultNOK;
        //            result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
        //        }

        //        return result;
        //    }

        //    catch (Exception ex)
        //    {
        //        //throw ex;
        //        result.resultado = Utilidades.Result.tipoResultado.resultNOK;
        //        result.message = "Error al guardar los registros, intente de nuevo si el problema persiste comuniquese con el administrador";
        //        return result;
        //    }
        //}

        /// <summary>
        /// Funcion que se encarga de borrar el registro que se identifica
        /// </summary>
        /// <param name="id">Identificador del registro a elimar</param>
        /// <param name="parametrizacionEficiencia">Datos a eliminar</param>
        /// <returns></returns>
        //public Result.result DeleteParametrizacionEficienciaARL(int id, ParametrosEficienciaARL parametrizacionEficiencia)
        //{
        //    ColpatriaSAI.Negocio.Componentes.Utilidades.Result.result result = new Utilidades.Result.result();
        //    var parametrizacionActual = this.tabla.ParametrosEficienciaARLs.Where(parametrizacionEficienciaARL => parametrizacionEficienciaARL.id == id).First();

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

        #region Otras

        /// <summary>
        /// Funcion que se encarga de traer la parametrizacion de eficiencia de ARL
        /// </summary>
        /// <returns>Listado de parametrizacion</returns>
        //public List<ParametrosEficienciaARL> ListarParametrizacionEficienciaARL()
        //{
        //    return tabla.ParametrosEficienciaARLs.Where(ParametrizacionEficienciaARL => ParametrizacionEficienciaARL.id > 0).ToList();
        //}

        /// <summary>
        /// Funcion que se encarga de traer la parametrizacion de la eficiencia ARL
        /// </summary>
        /// <param name="id">Identificador de la eficiencia a mostrar</param>
        /// <returns></returns>
        //public ParametrosEficienciaARL ListarParametrizacionEficienciaARLByID(int id)
        //{
        //    return tabla.ParametrosEficienciaARLs.Where(ParametrizacionEficienciaARL => ParametrizacionEficienciaARL.id == id).First();
        //}

        #endregion

    }
}
