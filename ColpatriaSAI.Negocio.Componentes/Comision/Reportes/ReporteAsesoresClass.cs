using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class ReporteAsesoresClass
    {
        public string CLAVE { get; set; }
        public string INTERMEDIARIO { get; set; }
        public string SUCURSAL { get; set; }
        public string TOTAL_RECAUDO { get; set; }
        public string PORCENTAJE_PROMEDIO_COMISION_FIJA { get; set; }
        public string TOTAL_VALOR_COMISION_FIJA { get; set; }
        public string TOTAL_USUARIO_NUEVOS { get; set; }
        public string TOTAL_EQUIVALENCIAS_X_USUARIO_NUEVOS { get; set; }
        public string CANTIDAD_USUARIOS_FINAL { get; set; }
        public string CANTIDAD_USUARIOS_INICIAL { get; set; }
        public string TOTAL_USUARIOS_NETOS { get; set; }
        public string TOTAL_EQUIVALENCIAS_X_NETOS { get; set; }
        public string PORCENTAJE_COMISION_VARIABLE_DEL_MES { get; set; }
        public string PORCENTAJE_PROMEDIO_COMISION_VARIABLE_PAGADA { get; set; }
        public string TOTAL_VALOR_COMISION_VARIABLE_PAGADA { get; set; }
        public string PORCENTAJE_PROMEDIO_COMISION_TOTAL { get; set; }
        public string VALOR_COMISION_TOTAL { get; set; }
    }

    public class ReporteAsesoresViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un año de consulta")]
        public int anio { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un mes de consulta")]
        public int mes { get; set; }
        public string ClavesAsesor { get; set; }
        public string Apellidogerente { get; set; }
        public string ApellidoDirector { get; set; }
        public string IdAsesores { get; set; }
        public int CanalVentas { get; set; }
        public int SubCanal { get; set; }
        public string NombreDirector { get; set; }
        public string ClaveDirector { get; set; }
        public int IdDirector { get; set; }
        public string NombreGerente { get; set; }
        public string ClaveGerente { get; set; }
        public int IdGerente { get; set; }
        public double PorcentajeComisionFija { get; set; }
        public double PorcentajeComisionVariable { get; set; }
        public double PorcentajeComisionTotal { get; set; }
        public int TipoPlan { get; set; }
        public int TipoContrato { get; set; }
        public int EstadoBeneficiario { get; set; }
        public int EdadDesde { get; set; }
        public int EdadHasta { get; set; }
        public int Visualizar { get; set; }
        public int TipoReporte { get; set; }
        public int modelo { get; set; }
        public int sucursal { get; set; }
        public string IdLiqComision { get; set; }

        public List<SelectListItem> AnioList;
        public List<SelectListItem> MesList;
        public List<SelectListItem> Modelo;
        public List<SelectListItem> Canales;
        public List<SelectListItem> SubCanales;
        public List<SelectListItem> Planes;
        public List<SelectListItem> Contratos;
        public List<SelectListItem> Sucursal;
        public List<SelectListItem> LiquidacionComision;

        public List<ReporteAsesoresClass> lDatos;

        public ReporteAsesoresViewModel()
        {
            this.AnioList = new List<SelectListItem>();
            this.MesList = new List<SelectListItem>();
            this.Modelo = new List<SelectListItem>();
            this.Sucursal = new List<SelectListItem>();
            this.Canales = new List<SelectListItem>();
            this.SubCanales = new List<SelectListItem>();
            this.Planes = new List<SelectListItem>();
            this.Contratos = new List<SelectListItem>();
            this.lDatos = new List<ReporteAsesoresClass>();
            this.LiquidacionComision = new List<SelectListItem>();
        }
    }
}
