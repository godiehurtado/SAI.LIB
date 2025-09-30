using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ColpatriaSAI.Negocio.Entidades;
using System.Data;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Consultas
{
    [ServiceContract]
    public interface IBitacoraCalculo
    {
        #region consultas
        [OperationContract]
        List<ListadoUsuarios> ListadoUsuariosCalculoComison();

        [OperationContract]
        List<ListadoUsuarios> ListadoUsuariosCalculoComisonPorFechas(string fechIni, string fechFin);

        [OperationContract]
        List<ListadoUsuarios> ListadoFacturasCalculoComison();

        [OperationContract]
        List<ListadoUsuarios> ListadoRecaudosCalculoComison();

        [OperationContract]
        List<DetalleUsuarios> DetalleUsuariosCalculoComison(string codExtraccion);

        [OperationContract]
        List<LogExtraccionBH> ExcelDetalleUsuario(string codExtraccion, string tipoLog, string idUsuario);

        [OperationContract]
        List<DetalleFacturas> DetalleFactura(string codExtraccion);

        [OperationContract]
        List<LogExtFacturacion> ExcelDetalleFactura(string codExtraccion, string tipoLog);
        
        [OperationContract]
        List<DetalleFacturas> DetalleRecaudo(string codExtraccion);

        [OperationContract]
        List<LogExtracionRecaudo> ExcelDetalleRecaudo(string codExtraccion, string tipoLog);
        #endregion
    }
}
