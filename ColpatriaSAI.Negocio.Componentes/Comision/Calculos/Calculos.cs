using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talentos = ColpatriaSAI.Negocio.Componentes.Comision.Calculos;
using System.Threading;
using ColpatriaSAI.Negocio.Entidades.CustomEntities;
using System.Configuration;
using ColpatriaSAI.Negocio.Entidades.Informacion;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Calculos
{
    public class Calculos : ICalculos
    {
        public List<ResultadosCalculos> CalculoTalentos(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int), string Username = default(string))
        {
            return new Talentos.CalculosRepository().CalculoTalentos(pAnio, pMes, pIdModelo, pAsesor, Username);
        }


        public List<ResultadosCalculos> CalculosNetos(int pAnio, int pMes, int pIdModelo, int pAsesor = default(int), string Username = default(string))
        {
            return new Talentos.CalculosRepository().CalculosNetos(pAnio, pMes, pIdModelo, pAsesor, Username);
        }

        public List<Entidades.ComisionVariableAsesor> ObtenerComisionVariablePorLiquidacion(int liquidacionComisionId)
        {
            var res = new Talentos.CalculosRepository().ObtenerComisionVariablePorLiquidacion(liquidacionComisionId);
            return res;
        }

        public List<Entidades.ResultadoProcedimientos.ComisionFijaFacturacion> ObtenerComisionFijaFacturacionPorLiquidacion(int liquidacionComisionId)
        {
            return new Talentos.CalculosRepository().ObtenerComisionFijaFacturacionPorLiquidacion(liquidacionComisionId);
        }

        public List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> ObtenerComisionFijaRecaudosPorLiquidacion(int liquidacionComisionId)
        {
            return new Talentos.CalculosRepository().ObtenerComisionFijaRecaudosPorLiquidacion(liquidacionComisionId);
        }

        public List<Entidades.ResultadoProcedimientos.ComisionFijaRecaudos> ObtenerComisionFijaRecaudosPorLiquidacion604(int liquidacionComisionId)
        {
            return new Talentos.CalculosRepository().ObtenerComisionFijaRecaudosPorLiquidacion604(liquidacionComisionId);
        }

        public ResultadoOperacionBD PreCalcularComisionSegunModelo(int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId)
        {
            return new Talentos.CalculosRepository().PreCalcularComisionSegunModelo(modeloId, anio, mes,liquidacionComisionId,tipoLiquidacionId);
        }

        public ResultadoOperacionBD LiquidarComisionSegunModelo(int modeloId, short anio, byte mes, int liquidacionComisionId)
        {
            return new Talentos.CalculosRepository().LiquidarComisionSegunModelo(modeloId, anio, mes, liquidacionComisionId);
        }

        public List<Entidades.LiquidacionComision> ObtenerHistoricoLiquidaciones()
        {
            return new Talentos.CalculosRepository().ObtenerHistoricoLiquidaciones();
        }

        public IAsyncResult BeginProcesoExtraccionBeneficiarioFacturacionRecaudos(string codigoExtraccion, short anio, byte mes, AsyncCallback callback, object asyncState)
        {
            string msg = String.Format("BeginProcesoExtraccionBeneficiarioFacturacionRecaudos called with: {0}{1}{2} ",
                codigoExtraccion,
                anio.ToString(),
                mes.ToString());
            Console.WriteLine(msg);
            return new CompletedAsyncResult<string>(msg);
        }

        public string EndProcesoExtraccionBeneficiarioFacturacionRecaudos(IAsyncResult r)
        {
            CompletedAsyncResult<string> result = r as CompletedAsyncResult<string>;
            Console.WriteLine("EndProcesoExtraccionBeneficiarioFacturacionRecaudos called with: " + result.Data);
            return result.Data;
        }

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
        //public ResultadoOperacionBD ExtractCf_CV(Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec)
        //{
        //    AppSettingsReader reader = new AppSettingsReader();
        //    string idApp = reader.GetValue("idPackagesExecutionService", String.Empty.GetType()).ToString();

        //    return new Talentos.CalculosRepository().ExtractCf_CV(idApp, parametrosEtlCF, parametrosEtlCV, modeloId, anio, mes, liquidacionComisionId, tipoLiquidacionId, usuario, tipoEjec);
        //}

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
        public ResultadoOperacionBD ExtractCf_CV(Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, short anio, byte mes, byte dia, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec, InfoAplicacion info)
        {
            AppSettingsReader reader = new AppSettingsReader();
            string idApp = reader.GetValue("idPackagesExecutionService", String.Empty.GetType()).ToString();

            return new Talentos.CalculosRepository().ExtractCf_CV(idApp, parametrosEtlCF, parametrosEtlCV, anio, mes, dia, liquidacionComisionId, tipoLiquidacionId, usuario, tipoEjec, info);
        }

        /// <summary>
        /// 2023-11-26 DAHG: Método para ejecutar la liquidación de comisión por modelo
        /// </summary>
        /// <param name="extraccionId"></param>
        /// <param name="usuario"></param>
        /// <param name="modeloId"></param>
        /// <returns></returns>
        public ResultadoOperacionBD LiquidarComision(int extraccionId, string usuario, int modeloId)
        {
            return new Talentos.CalculosRepository().LiquidarComision(extraccionId,usuario,modeloId);
        }

        /// <summary>
        /// 2023-11-26 DAHG: Método para cargar la información de una extracción historica ya existente
        /// </summary>
        /// <param name="extraccionId"></param>
        /// <returns></returns>
        public ResultadoOperacionBD CargarExtraccionHistorico(int extraccionId)
        {
            return new Talentos.CalculosRepository().CargarExtraccionHistorico(extraccionId);
        }

        public List<int> ValLiqPendientes()
        {
            return new Talentos.CalculosRepository().ValLiqPendientes();
        }

        /// <summary>
        /// 2023-11-27 DAHG: Método para validar proceso de extracción por fecha
        /// </summary>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <param name="dia"></param>
        /// <returns></returns>
        public ExtraccionComision ValidarExtraccion(int anio, int mes, int dia)
        {
            return new Talentos.CalculosRepository().ValidarExtraccion(anio, mes, dia);
        }

        /// <summary>
        /// 2023-11-27 DAHG: Método para validar proceso de extracción por fecha
        /// </summary>
        /// <returns></returns>
        public ExtraccionComision ValidarUltimaExtraccion()
        {
            return new Talentos.CalculosRepository().ValidarUltimaExtraccion();
        }

        /// <summary>
        /// 2024-04-16 DAHG: Método para consultar el histórico de extracciones
        /// </summary>
        /// <returns></returns>
        public List<ExtraccionComision> ConsultarHistoricoExtraccion()
        {
            return new Talentos.CalculosRepository().ConsultarHistoricoExtraccion();
        }



        /// <summary>
        /// Obtiene un periodo  de calculo de comisiòn basado en un año y me epsecifico
        /// </summary>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        public List<string[]> obtenerPeriodoCalcComision(int anio, int mes)
        {
            return new Talentos.CalculosRepository().obtenerPeriodoCalcComision(anio,mes);
        }

        public ResultadoOperacionBD LiquidarComisiones(int liquidacionComisionId)
        {
            AppSettingsReader reader = new AppSettingsReader();
            string idApp = reader.GetValue("idPackagesExecutionService", String.Empty.GetType()).ToString();
            return new Talentos.CalculosRepository().LiquidarComisiones(idApp,liquidacionComisionId);
        }

        
        public bool ValidaAnularLiquidacion(int idLiquidacion)
        {
            return new Talentos.CalculosRepository().ValidaAnularLiquidacion(idLiquidacion);
        }


        public ResultadoOperacionBD ExtractAnulacion(Dictionary<string, object> parametrosEtlAnulacion, int idLiquidacion)
        {
            AppSettingsReader reader = new AppSettingsReader();
            string idApp = reader.GetValue("idPackagesExecutionService", String.Empty.GetType()).ToString();
            return new Talentos.CalculosRepository().ExtractAnulacion(idApp, parametrosEtlAnulacion, idLiquidacion);
        }


        //public ResultadoOperacionBD ReprocesarLiquidacion(Dictionary<string, object> parametrosEtlCF, Dictionary<string, object> parametrosEtlCV, Dictionary<string, object> parametrosEtlAnulacion, int modeloId, short anio, byte mes, int liquidacionComisionId, byte tipoLiquidacionId, string usuario, int tipoEjec, InfoAplicacion info)
        //{
        //    AppSettingsReader reader = new AppSettingsReader();
        //    string idApp = reader.GetValue("idPackagesExecutionService", String.Empty.GetType()).ToString();
        //    return new Talentos.CalculosRepository().ReprocesarLiquidacion(idApp, parametrosEtlCF, parametrosEtlCV, parametrosEtlAnulacion, modeloId, anio, mes, liquidacionComisionId, tipoLiquidacionId, usuario, tipoEjec, info);
        //}

        public ResultadoOperacionBD ReprocesarLiquidacion(int idLiquidacion)
        {
            return new Talentos.CalculosRepository().ReprocesarLiquidacion(idLiquidacion);
        }


        public ResultadoOperacionBD ActualizaEstadoReprocesar(int idLiquidacion)
        {
            return new Talentos.CalculosRepository().ActualizaEstadoReprocesar(idLiquidacion);
        }


        public List<int> ValReprocesoLiq()
        {
            return new Talentos.CalculosRepository().ValReprocesoLiq();
        }
    }

    class CompletedAsyncResult<T> : IAsyncResult
    {
        T data;

        public CompletedAsyncResult(T data)
        { this.data = data; }

        public T Data
        { get { return data; } }

        #region IAsyncResult Members
        public object AsyncState
        { get { return (object)data; } }

        public WaitHandle AsyncWaitHandle
        { get { throw new Exception("The method or operation is not implemented."); } }

        public bool CompletedSynchronously
        { get { return true; } }

        public bool IsCompleted
        { get { return true; } }
        #endregion
    }
}
