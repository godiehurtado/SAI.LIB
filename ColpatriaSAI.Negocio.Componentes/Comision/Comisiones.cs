using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLLProceso = ColpatriaSAI.Negocio.Componentes.Comision.Procesos.Extraccion;
using BLLCalculo = ColpatriaSAI.Negocio.Componentes.Comision.Variable;
using ColpatriaSAI.Negocio.Componentes.Comision;
using adminComision= ColpatriaSAI.Negocio.Componentes.Comision.Administracion;

namespace ColpatriaSAI.Negocio.Componentes
{
    public class Comisiones : IComisiones
    {
        public List<Entidades.CPRO_ProcesosExtraccion> ListarProcesosExtraccion()
        {
            BLLProceso.Proceso bll = new BLLProceso.Proceso();
            return bll.ListarProcesosExtracion();
        }

        public ResultadoProcesoExtraccion EjecutarPorcesoExtraccion(byte procesoId, DateTime fechaInicio, DateTime fechaFin, string codigoExtraccion)
        {
            BLLProceso.Proceso bll = new BLLProceso.Proceso();
            return bll.EjecutarProceso(procesoId, fechaInicio, fechaFin, codigoExtraccion);
        }

        public ResultadoOperacionBD InsertarLiquidacionComision(Entidades.LiquidacionComision obj, string Username = default(string))
        {
            BLLCalculo.CalculoComisionVariable bll = new BLLCalculo.CalculoComisionVariable();
            return bll.InsertarLiquidacionComision(obj, Username);
        }


        public List<Entidades.CustomEntities.LogProcesoBH> LogProcesoExtraccion()
        {
            throw new NotImplementedException();
        }

        #region Administración comisiones
        public List<adminComision.ConfigParametros> ObtenerParametros(String[] parametros)
        {
            return new adminComision.Administracion().ObtenerParametros(parametros);
        }

        public ResultadoOperacionBD EditarParametro(Int32 id, string valor,string usuario)
        {
            return new adminComision.Administracion().EditarParametro(id,valor,usuario);
        }

        public List<adminComision.parmbhExtractNoveades> ObtenerListNovedades(String[] parametros)
        {
            return new adminComision.Administracion().ObtenerListNovedades(parametros);
        }

        public List<adminComision.parmbhExtractNoveades> ObtenerListNovedadesAdd(String[] parametros)
        {
            return new adminComision.Administracion().ObtenerListNovedadesAdd(parametros);
        }

        #endregion
    }
}
