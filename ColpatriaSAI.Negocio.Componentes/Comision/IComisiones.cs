using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Componentes.Comision;
using ColpatriaSAI.Negocio.Componentes.Comision.Administracion;

namespace ColpatriaSAI.Negocio.Componentes
{
    [ServiceContract]
    public interface IComisiones
    {
        #region Extraccion
        [OperationContract]
        List<Entidades.CPRO_ProcesosExtraccion> ListarProcesosExtraccion();

        [OperationContract]
        ResultadoProcesoExtraccion EjecutarPorcesoExtraccion(byte procesoId, DateTime fechaInicio, DateTime fechaFin, string codigoExtraccion);

        [OperationContract]
        List<Entidades.CustomEntities.LogProcesoBH> LogProcesoExtraccion();


        #endregion

        #region Calculos
        [OperationContract]
        ResultadoOperacionBD InsertarLiquidacionComision(Entidades.LiquidacionComision obj, string Username = default(string));
        #endregion

        #region Administracion
        [OperationContract]
        List<ConfigParametros> ObtenerParametros(String[] parametros);

        [OperationContract]
        ResultadoOperacionBD EditarParametro(Int32 id, string valor,string usuario);

        [OperationContract]
        List<parmbhExtractNoveades> ObtenerListNovedades(String[] parametros);

        [OperationContract]
        List<parmbhExtractNoveades> ObtenerListNovedadesAdd(String[] parametros);
        #endregion
    }
}
