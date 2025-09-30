using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Entidades.CustomEntities;
using ColpatriaSAI.Negocio.Entidades.Informacion;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Calculos
{

    [ServiceContract]
    public interface ICalculos
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAnio"></param>
        /// <param name="pMes"></param>
        /// <param name="pAsesor"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResultadosCalculos> CalculoTalentos(int pAnio, int pMes, int pAsesor, int pIdModelo, string Username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAnio"></param>
        /// <param name="pMes"></param>
        /// <param name="pAsesor"></param>
        /// <param name="pIdModelo"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResultadosCalculos> CalculosNetos(int pAnio, int pMes, int pAsesor, int pIdModelo, string Username);
        
        [OperationContract]
        List<Entidades.ComisionVariableAsesor> ObtenerComisionVariablePorLiquidacion(int liquidacionComisionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.ResultadoProcedimientos.ComisionFijaFacturacion> ObtenerComisionFijaFacturacionPorLiquidacion(int liquidacioncomisionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeloId"></param>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        [OperationContract]
        List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> ObtenerComisionFijaRecaudosPorLiquidacion(int liquidacioncomisionId);


        [OperationContract]
        List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> ObtenerComisionFijaRecaudosPorLiquidacion604(int liquidacioncomisionId);

        [OperationContract]
        ResultadoOperacionBD PreCalcularComisionSegunModelo(int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId);

        [OperationContract]
        ResultadoOperacionBD LiquidarComisionSegunModelo(int modeloId, short anio, byte mes, int liquidacionComisionId);

        [OperationContract]
        List<Entidades.LiquidacionComision> ObtenerHistoricoLiquidaciones();

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginProcesoExtraccionBeneficiarioFacturacionRecaudos(string codigoExtraccion,short anio, byte mes, AsyncCallback callback, object asyncState);

        /// <summary>
        /// 2023-11-26 DAHG: Se comentarea método para conservar BK de versió antigua
        /// </summary>
        /// <param name="parametrosEtlCF"></param>
        /// <param name="parametrosEtlCV"></param>
        /// <param name="modeloId"></param>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <param name="liquidacionComisionId"></param>
        /// <param name="tipoLiquidacionId"></param>
        /// <param name="usuario"></param>
        /// <param name="tipoEjec"></param>
        /// <returns></returns>
        //[OperationContract]
        //ResultadoOperacionBD ExtractCf_CV(Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec);

        /// <summary>
        /// 2023-11-26 DAHG: 2023-11-26 DAHG: Nueva versión del método para ejecutar la extracción sin modelo, y que esta pueda ser reusada por todos los modelos de liquidación
        /// </summary>
        /// <param name="parametrosEtlCF"></param>
        /// <param name="parametrosEtlCV"></param>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <param name="liquidacionComisionId"></param>
        /// <param name="tipoLiquidacionId"></param>
        /// <param name="usuario"></param>
        /// <param name="tipoEjec"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD ExtractCf_CV(Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, short anio, byte mes, byte dia, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec, InfoAplicacion info);

        /// <summary>
        /// 2023-11-26 DAHG: Método para ejecutar la liquidación de comisión por modelo
        /// </summary>
        /// <param name="extraccionId"></param>
        /// <param name="usuario"></param>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD LiquidarComision(int extraccionId, string usuario, int modeloId);

        /// <summary>
        /// 2023-11-26 DAHG: Método para cargar la información de una extracción historica ya existente
        /// </summary>
        /// <param name="extraccionId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoOperacionBD CargarExtraccionHistorico(int extraccionId);

        [OperationContract]
        List<string[]> obtenerPeriodoCalcComision(int anio, int mes);


        string EndProcesoExtraccionBeneficiarioFacturacionRecaudos(IAsyncResult result);

        [OperationContract]
        List<int> ValLiqPendientes();

        [OperationContract]
        ExtraccionComision ValidarExtraccion(int anio, int mes, int dia);

        [OperationContract]
        ExtraccionComision ValidarUltimaExtraccion();

        [OperationContract]
        List<ExtraccionComision> ConsultarHistoricoExtraccion();

        [OperationContract]
        ResultadoOperacionBD LiquidarComisiones(int liquidacionComisionId);

        [OperationContract]
        bool ValidaAnularLiquidacion(int idLiquidacion);

        [OperationContract]
        ResultadoOperacionBD ExtractAnulacion(Dictionary<string, object> parametrosEtlAnulacion, int idLiquidacion);

        //[OperationContract]
        //ResultadoOperacionBD ReprocesarLiquidacion(Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, Dictionary<string, object> parametrosEtlAnulacion, int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec, InfoAplicacion info);

        [OperationContract]
        ResultadoOperacionBD ReprocesarLiquidacion(int idLiquidacion);

        [OperationContract]
        ResultadoOperacionBD ActualizaEstadoReprocesar(int idLiquidacion);

        [OperationContract]
        List<int> ValReprocesoLiq();
    }
}
