using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ColpatriaSAI.Negocio.Entidades;
using System.Data;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Modelo
{
    [ServiceContract]
    public interface IModeloComision
    {
        #region Comision Fija
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        List<FactorComisionFija> ListarFactorComisionFijaXModeloId(int modeloId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD InsertarFactorComisionFija(FactorComisionFija objetoDb, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD EliminarFactorComisionFija(FactorComisionFija objetoDb,string Username);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factorId"></param>
        /// <returns></returns>
        [OperationContract]
        FactorComisionFija ObtenerFactorComisionFijaXId(int factorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ActualizarFactorComisionFija(FactorComisionFija objetoDb, string Username);

        [OperationContract]
        Boolean ValidarEdadMaxima(Int32 edad);
        #endregion

        #region Modelo Comision


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.ModeloComision> ListarModelosComision();

        [OperationContract]
        List<LiquidacionComisionClass> ListarLiquidacionComision();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloComisionId"></param>
        /// <returns></returns>
        [OperationContract]
        Entidades.ModeloComision ObtenerModeloComisionXId(int modeloComisionId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.Canal> ListarCanalesDetalle();

        [OperationContract]
        List<Entidades.TipoIntermediario> ListarTipoIntermediario();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.CanalDetalleTipoIntermediario> ListarCanalDetalleTipoIntermediario();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD InsertarModeloComision(Entidades.ModeloComision objetoDB,string Username);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDB"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ActualizarModeloComision(Entidades.ModeloComision objetoDB, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD EliminarModeloComision(int modeloId,string Username);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD DuplicarModeloComision(int modeloId, Entidades.ModeloComision objetoDB, string Username);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.ModeloComision> ListarModeloComisionVigentes();
        #endregion

        #region Excepcion Penalizacion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloComisionId"></param>
        /// <returns></returns>
        [OperationContract]
        List<_ExcepcionPenalizacion> ListarExcepcionesPenalizacionXModeloComision(int modeloComisionId);

        [OperationContract]
        List<_ExcepcionPenalizacion> _lisErrores();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="penalizacionId"></param>
        /// <returns></returns>
        [OperationContract]
        ExcepcionPenalizacion ObtenerExcepcionPenalizacionXId(int penalizacionId);

        /// <summary>
        /// Metodo para la excepcion de penalizacion
        /// </summary>
        /// <param name="penalizacion"></param>
        /// <returns></returns>
        [OperationContract]
        List<_ExcepcionPenalizacion> CargueExcepcionesPenalizacion(List<_ExcepcionPenalizacion> penalizacion);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDB"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD InsertarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion objetoDB, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDB"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ActualizarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion objetoDB,string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDB"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD EliminarExcepcionPenalizacion(ExcepcionPenalizacion objetoDB, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Participante> ListarParticipantes();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="excepcionId"></param>
        /// <param name="modeloId"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD CambiarEstadoExcepcionPenalizacion(int excepcionId, int modeloId, string Username);
        #endregion

        #region Excepcion Fija Variable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloComisionId"></param>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.ExcepcionFijaVariable> ListarExcepcionesFijaVariableXModeloComision(int modeloComisionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="penalizacionId"></param>
        /// <returns></returns>
        [OperationContract]
        Entidades.ExcepcionFijaVariable ObtenerExcepcionFijaVariableXId(int excepcionFijaVariableId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDB"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD InsertarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDB, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDB"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ActualizarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDB, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD EliminarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDb, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResultadoOperacionBD> CargarExcepcionesMasivo(DataTable pData, out string pErrores, string Username,int modelo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="excepcionId"></param>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD CambiarEstadoExcepcionFijaVariable(int excepcionId, int modeloId, string Username);

        #endregion

        #region Comision Variable
        #region Factores Netos
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        List<FactorComisionVariableNeto> ListarFactorNetoComisionVariableXModeloId(int modeloId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD InsertarFactorNetoComisionVariable(FactorComisionVariableNeto dbobj,string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD EliminarFactorNetoComisionVariable(FactorComisionVariableNeto dbobj, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factorId"></param>
        /// <returns></returns>
        [OperationContract]
        FactorComisionVariableNeto ObtenerFactorNetoComisionVariableXId(int factorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ActualizarFactorNetoComisionVariable(FactorComisionVariableNeto dbobj, string Username);
        #endregion

        #region Factores Nuevos
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        List<FactorComisionVariableNuevo> ListarFactorNuevoComisionVariableXModeloId(int modeloId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD InsertarFactorNuevoComisionVariable(FactorComisionVariableNuevo dbobj, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD EliminarFactorNuevoComisionVariable(FactorComisionVariableNuevo dbobj, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factorId"></param>
        /// <returns></returns>
        [OperationContract]
        FactorComisionVariableNuevo ObtenerFactorNuevoComisionVariableXId(int factorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objetoDb"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ActualizarFactorNuevoComisionVariable(FactorComisionVariableNuevo dbobj, string Username);
        #endregion
        #endregion

        #region Matriz Comision Variable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <param name="ejex"></param>
        /// <param name="ejey"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD AdicionarRangosMatriz(int modeloId, List<Entidades.RangosXNeto> ejex, List<Entidades.RangosYNuevo> ejey, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD EliminarMatrizComisionVariableXModeloId(int modeloId, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.MatrizComisionVariable> ListarValoresMatrizComisionVariableXModelo(int modeloId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.RangosYNuevo> ListarRangosYNuevoMatrizComisionVariableXModelo(int modeloId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.RangosXNeto> ListarRangosXNetoMatrizComisionVariableXModelo(int modeloId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbobj"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ActualizarFactorMatrizComisionVariable(Entidades.MatrizComisionVariable dbobj, string Username);
        #endregion

    }
}
