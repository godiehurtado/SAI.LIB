using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using ColpatriaSAI.Negocio.Entidades;
using System.Data;
using Entidades = ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    [ServiceContract]
    public interface IReportes
    {
        /// <summary>
        /// Metodo que retorna el dataset para le cargue del reporte 
        /// </summary>
        /// <param name="Periodo">Fecha de inicio del periodo a consultar en el reporte</param>
        /// <param name="ClavesAsesor">Clave (s) que posea el asesor</param>
        /// <param name="Director"> Id del Director Asociado al asesor</param>
        /// <param name="Gerente">Id del Gerente</param>
        /// <param name="CanalVentasId">Codigo del Canal de Ventas usado por el asesor</param>
        /// <param name="SubCanalId">Codigo del subcanal</param>
        /// <param name="NoContrato">Numero del contrato a consultar </param>
        /// <param name="TipoPlan">Tipo de plan vendido </param>
        /// <param name="TipoContrato">Tipo de contrato vendido</param>
        /// <param name="Estado">Nuevos o Renovados</param>
        /// <param name="Edad">Edad del beneficiario</param>
        /// <returns></returns>
        [OperationContract]
        List<ReporteBeneficiarioClass> GetReporteBeneficiarios(int año, int mes, string ClavesAsesor, string IdDirector, string IdGerente, int CanalVentasId, int SubCanalId, int TipoPlan, int TipoContrato, int modelo, int sucursal);

        [OperationContract]
        List<ReporteDetalleBeneficiariosClass> GetReporteDetalleBeneficiarios(int anio, int mes, string ClavesAsesor, string IdDirector, string IdGerente, int CanalVentasId, int SubCanalId, int TipoPlan, int TipoContrato, int modelo, int sucursal, string numerocontrato, int estadoBeneficiario);

        [OperationContract]
        List<ReporteTrasnferenciasClass> GetReporteTransferencias(DateTime pFechaPeriodo, int pAsesorInicial = default(int), int pAsesorFinal = default(int),
            int pCanalVentas = default(int), int pSubCanal = default(int), int pDirector = default(int), int pGerOfic = default(int), int pGerReg = default(int),
            string pNumeroContrato = default(string), string pSubContrato = default(string), int pTipoPlan = default(int), int pTipoContrato = default(int), int pTipoTransferencia = default(int), int modelo = default(int), int sucursal = default(int));


        [OperationContract]
        List<ReporteAsesoresClass> GetReporteAsesores(int año = default(int), int mes = default(int), string pClavesAsesor = default(string), int pCanalVenta = default(int),
            int pSubCanal = default(int), int IdGerente = default(int), int IdDirector = default(int), int modelo = default(int), int sucursal = default(int));
        [OperationContract]
        List<ReportBHDeferredClass> GetReportBHDeferred(DateTime DateIni = default(DateTime), DateTime DateFin = default(DateTime), int Format = default(int), int ProcessType = default(int),string UserName = default(string));

        [OperationContract]
        List<ReportBHAccruedClass> GetReportBHAccrued(DateTime DateIni = default(DateTime), DateTime DateFin = default(DateTime), int Format = default(int), int ProcessType = default(int), string UserName = default(string));

        [OperationContract]
        List<ReporteAsesoresClass> GetReporteLiderComercial(int año = default(int), int mes = default(int), string pClavesAsesor = default(string), int pCanalVenta = default(int),
            int pSubCanal = default(int), int IdGerente = default(int), int IdDirector = default(int), int modelo = default(int), int sucursal = default(int));

        [OperationContract]
        int ValidaDirector(String IdDocumento);

        [OperationContract]
        Dictionary<string, string> GetInfoPagoAsesorPortal(int idLiquidacion, string clave);

        [OperationContract]
        Dictionary<int, string> GetLiquidacionesPortalAsesor();



        [OperationContract]
        Dictionary<int, string> GetAvaliablePeriodsBH();
    }
}
