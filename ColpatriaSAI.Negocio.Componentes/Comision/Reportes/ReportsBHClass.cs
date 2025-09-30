using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class ReportBHDeferredClass
    {
        // COLUMNAS REPORTE BH - SAI DIFERIDO
        
        public string CONTRATO { get; set; }
        public string NVALUE { get; set; }
        public string TIPO { get; set; }
        public string FACTURA { get; set; }
        public string FECHA_EXIGIBLE { get; set; }
        public string TIPO_CONTRATO { get; set; }
        public string REGIONAL { get; set; }
        public string PERIODO_DEVENGADO { get; set; }
        public string ID_NOVEDAD { get; set; }
        public string FECHA_RADICACION { get; set; }
        public string MEM_NCODE { get; set; }
        public string CEDULA { get; set; }
        public string NOMBRE { get; set; }
        public string ANNIO_CREACION { get; set; }
        public string NOVEDAD { get; set; }
        public string CAUSA { get; set; }
        public string CAUSAL_NOTA { get; set; }
        public string ESTADO_CONTRATO_EN_CIERRE { get; set; }
        public string FECHA_INICIO_VIGENCIA { get; set; }
        public string FECHA_FIN_VIGENCIA { get; set; }
        public string CLAVE_ASESOR { get; set; }
        public string MODALIDAD_PAGO { get; set; }
        // DATOS SAI
        public string PORCENTAJE_COMISION { get; set; }
        public string ESTADO_BENEFICIARIO { get; set; }
        public string RANGO_EDAD { get; set; }
        public string MARCA_EXCEPCION { get; set; }
    }



    public class ReportBHAccruedClass
    {
        // COLUMNAS REPORTE BH - SAI DEVENGADOS
        public string CONTRATO { get; set; }
        public string PLAN { get; set; }
        public string NVALUE { get; set; }
        public string CUENTA_CONTABLE { get; set; }
        public string TIPO { get; set; }
        public string FACTURA { get; set; }
        public string FECHA_EXIGIBLE { get; set; }
        public string TIPO_CONTRATO { get; set; }
        public string REGIONAL { get; set; }
        public string PERIODO_DEVENGADO { get; set; }
        public string ID_NOVEDAD { get; set; }
        public string FECHA_RADICACION { get; set; }
        public string MEM_NCODE { get; set; }
        public string CEDULA { get; set; }
        public string NOMBRE { get; set; }
        public string ANNIO_CREACION { get; set; }
        public string ESTADO_CONTRATO_EN_CIERRE { get; set; }
        public string FECHA_INICIO_VIGENCIA { get; set; }
        public string FECHA_FIN_VIGENCIA { get; set; }
        public string CLAVE_ASESOR { get; set; }
        public string MODALIDAD_PAGO { get; set; }
        // DATOS SAI
        public string PORCENTAJE_COMISION { get; set; }
        public string ESTADO_BENEFICIARIO { get; set; }
        public string RANGO_EDAD { get; set; }
        public string MARCA_EXCEPCION { get; set; }
        
    }


    public class ReportBHViewModel
    {
        
        public DateTime DateIni { get; set; }
        
        public DateTime DateFin { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un formato de consulta")]
        public int Format { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un reporte para cargar")]
        public int ProcessType { get; set; }
        
        
        
        [Required(ErrorMessage = "Debe seleccionar un periodo para extraer")]
        public string Period { get; set; }


        public int Visualizar { get; set; }

        public List<SelectListItem> DateIniList;
        public List<SelectListItem> DateFinList;
        public List<SelectListItem> FormatList;
        public List<SelectListItem> ProcessTypeList;

        public List<SelectListItem> AvaliablePeriodsReport;

        public List<ReportBHDeferredClass> lDatos;
        public List<ReportBHAccruedClass> lDatos2;

        public ReportBHViewModel()
        {
            this.DateIniList = new List<SelectListItem>();
            this.DateFinList = new List<SelectListItem>();
            this.FormatList = new List<SelectListItem>();
            this.ProcessTypeList = new List<SelectListItem>();
            this.lDatos = new List<ReportBHDeferredClass>();
            this.lDatos2 = new List<ReportBHAccruedClass>();

            this.AvaliablePeriodsReport = new List<SelectListItem>();


    }
    }





}
