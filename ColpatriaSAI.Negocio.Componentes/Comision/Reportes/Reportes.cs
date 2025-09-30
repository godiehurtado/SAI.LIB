using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Entidades = ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class Reportes : IReportes
    {       

        public List<ReporteBeneficiarioClass> GetReporteBeneficiarios(int año, int mes, string ClavesAsesor, string IdDirector, string IdGerente, int CanalVentasId, int SubCanalId, int TipoPlan, int TipoContrato,int modelo,int sucursal)
        {
            return new ReporteBeneficiario().GetReporte(año, mes, ClavesAsesor, IdDirector, IdGerente, CanalVentasId, SubCanalId, TipoPlan, TipoContrato, modelo, sucursal);
        }

        public List<ReporteDetalleBeneficiariosClass> GetReporteDetalleBeneficiarios(int anio, int mes, string ClavesAsesor, string IdDirector, string IdGerente, int CanalVentasId, int SubCanalId, int TipoPlan, int TipoContrato, int modelo, int sucursal, string numerocontrato, int estadoBeneficiario)
        {
            return new ReporteDetalleBeneficiarios().GetReporte(anio, mes, ClavesAsesor, IdDirector, IdGerente, CanalVentasId, SubCanalId, TipoPlan, TipoContrato, modelo, sucursal, numerocontrato, estadoBeneficiario);
        }

        public List<ReporteTrasnferenciasClass> GetReporteTransferencias(DateTime pFechaPeriodo, int pAsesorInicial = default(int), int pAsesorFinal = default(int),
          int pCanalVentas = default(int), int pSubCanal = default(int), int pDirector = default(int), int pGerOfic = default(int), int pGerReg = default(int),
          string pNumeroContrato = default(string), string pSubContrato = default(string), int pTipoPlan = default(int), int pTipoContrato = default(int), int pTipoTransferencia = default(int), int modelo = default(int), int sucursal = default(int))
        {
            return new ReporteTransferencia().GetReporteTransferencias(pFechaPeriodo, pAsesorInicial, pAsesorFinal, pCanalVentas, pSubCanal, pDirector, pGerOfic, pGerReg, pNumeroContrato, pSubContrato, pTipoPlan, pTipoContrato, pTipoTransferencia,modelo,sucursal);
        }

        public List<ReporteAsesoresClass> GetReporteAsesores(int año = default(int), int mes = default(int), string pClavesAsesor = default(string), int pCanalVenta = default(int),
            int pSubCanal = default(int), int IdGerente = default(int), int IdDirector = default(int), int modelo = default(int), int sucursal = default(int))
        {
            return new ReporteAsesores().GetReporteAsesores(año,mes, pClavesAsesor, pCanalVenta, pSubCanal, IdGerente, IdDirector,modelo,sucursal);
        }


        public List<ReporteAsesoresClass> GetReporteLiderComercial(int año = default(int), int mes = default(int), string pClavesAsesor = default(string), int pCanalVenta = default(int),
            int pSubCanal = default(int), int IdGerente = default(int), int IdDirector = default(int), int modelo = default(int), int sucursal = default(int))
        {
            return new ReporteAsesores().GetReporteLiderComercial(año, mes, pClavesAsesor, pCanalVenta, pSubCanal, IdGerente, IdDirector, modelo, sucursal);
        }


        public int ValidaDirector(string IdDocumento )
        {
            return new ReporteAsesores().ValidaDirector(IdDocumento);
        }

        public Dictionary<string, string> GetInfoPagoAsesorPortal(int idLiquidacion, string clave)
        {
            return new ReporteAsesores().GetInfoPagoAsesorPortal(idLiquidacion, clave);
        }

        public Dictionary<int, string> GetLiquidacionesPortalAsesor()
        {
            return new ReporteAsesores().GetLiquidacionesPortalAsesor();
        }

        public List<ReportBHDeferredClass> GetReportBHDeferred(DateTime DateIni = default, DateTime DateFin = default, int Format = 0, int ProcessType = 0, string UserName = null)
        {
            return new ReportsBH().GetReportBHDeferred(DateIni, DateFin, Format, ProcessType, UserName);
        }

        public List<ReportBHAccruedClass> GetReportBHAccrued(DateTime DateIni = default, DateTime DateFin = default, int Format = 0, int ProcessType = 0, string UserName = null)
        {
            return new ReportsBH().GetReportBHAccrued(DateIni, DateFin, Format, ProcessType, UserName);
        }

        public Dictionary<int, string> GetAvaliablePeriodsBH()
        {
            return new ReportsBH().GetAvaliablePeriodsBH();
        }
        
    }
}
