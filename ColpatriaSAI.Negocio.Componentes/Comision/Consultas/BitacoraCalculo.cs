using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponenteCalculosTablero = ColpatriaSAI.Negocio.Componentes.Comision.Fija;
using ComponenteComisionVariable = ColpatriaSAI.Negocio.Componentes.Comision.Variable;
using ComponenteModeloComision = ColpatriaSAI.Negocio.Componentes.Comision.Modelo;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Datos;
using Entidades = ColpatriaSAI.Negocio.Entidades;
using System.Data.Entity;
using ComponenteMatrizComisionVariable = ColpatriaSAI.Negocio.Componentes.Comision.Variable;
using System.Data;
using Componentes = ColpatriaSAI.Negocio.Componentes;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Consultas
{
    public class BitacoraCalculo:IBitacoraCalculo
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        #region consultas
        public List<ListadoUsuarios> ListadoUsuariosCalculoComison()
        {

            return new Tablero().traer();
        }

        public List<ListadoUsuarios> ListadoUsuariosCalculoComisonPorFechas(string fechIni, string fechFin)
        {
            return new Tablero().traerFiltroLogUsuarios(fechIni, fechFin);
        }

        public List<ListadoUsuarios> ListadoFacturasCalculoComison()
        {
            return new Tablero().ListarFacturasEntrantes();
        }

        public List<ListadoUsuarios> ListadoRecaudosCalculoComison()
        {
            return new Tablero().ListarRecaudosEntrantes();
        }

        public List<DetalleUsuarios> DetalleUsuariosCalculoComison(string codExtraccion)
        {
            return new Tablero().ListarDetalleUsuarios(codExtraccion);
        }

        public List<LogExtraccionBH> ExcelDetalleUsuario(string codExtraccion, string tipoLog, string idUsuario)
        {
            return new Tablero().ExcelDetalleUsuario(codExtraccion, tipoLog, idUsuario);
        }

        public List<DetalleFacturas> DetalleFactura(string codExtraccion)
        {
            return new Tablero().ListarDetalleFacturacion(codExtraccion);
        }


        public List<LogExtFacturacion> ExcelDetalleFactura(string codExtraccion, string tipoLog)
        {
            return new Tablero().ExcelDetalleFactura(codExtraccion, tipoLog);
        }

        public List<DetalleFacturas> DetalleRecaudo(string codExtraccion)
        {
            return new Tablero().ListarDetalleRecaudo(codExtraccion);
        }

       public List<LogExtracionRecaudo > ExcelDetalleRecaudo(string codExtraccion, string tipoLog)
        {
            return new Tablero().ExcelDetalleRecaudo(codExtraccion, tipoLog);
        }

        #endregion

    }
}
