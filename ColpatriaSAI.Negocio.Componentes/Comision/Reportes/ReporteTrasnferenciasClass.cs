using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class ReporteTrasnferenciasClass
    {
        public string Fecha_Periodo { get; set; }
        public DateTime Fecha_Transferencia { get; set; }
        public string Clave_Inicial { get; set; }
        public string Nombre_Asesor_Inicial { get; set; }
        public string Canal_Ventas { get; set; }
        public string SubCanal { get; set; }
        public string sucursal_old { get; set; }
        public string Director { get; set; }
        public string Gerente_Oficina { get; set; }
        public string Gerente_Regional { get; set; }
        public string Clave_Final { get; set; }
        public string Nombre_Asesor_Final { get; set; }
        public string Canal_Ventas_New { get; set; }
        public string SubCanal_new { get; set; }
        public string sucursal_new { get; set; }
        public string Director_new { get; set; }
        public string Gerente_Oficina_new { get; set; }
        public string Gerente_Regional_new { get; set; }
        public string Contrato { get; set; }
        public string SubContrato { get; set; }
        public string Tipo_Plan { get; set; }
        public string Tipo_Contrato { get; set; }
        public string Tipo_Transferencia { get; set; }
        public int beneficiarios_trasn { get; set; }
    }

    public class ReporteTransferenciasViewModel
    {
        [Required(ErrorMessage = "Debe existir una fecha de inicio de consulta")]
        public DateTime Fecha_Periodo { get; set; }
        public int Clave_Inicial_Asesor { get; set; }
        public string Nom_Asesor_Inicial { get; set; }
        public int Id_Asesor_Inicial { get; set; }
        public int Clave_Final_Asesor { get; set; }
        public string Nom_Asesor_Final { get; set; }
        public int Id_Asesor_Final { get; set; }
        [Required(ErrorMessage = "El apellido es requerido")]
        public int Canal_Venta { get; set; }
        public string Nom_CanalVenta { get; set; }
        public int SubCanal { get; set; }
        public string Nom_Subcanal { get; set; }
        public string Director { get; set; }
        public int Id_Director { get; set; }
        public string Gerente_Oficina { get; set; }
        public int Id_GerenteOficina { get; set; }
        public string Gerente_Regional { get; set; }
        public int Id_GerenteRegional { get; set; }
        public string Contrato { get; set; }
        public string Subcontrato { get; set; }
        public int Tipo_Plan { get; set; }
        public string NomTipoPlan { get; set; }
        public int Tipo_Contrato { get; set; }
        public int NomTipoContrato { get; set; }
        public int Tipo_Transferencia { get; set; }
        public int Visualizar { get; set; }
        public int TipoReporte { get; set; }
        public int modelo { get; set; }
        public int sucursal { get; set; }

        public List<SelectListItem> Sucursal;
        public List<SelectListItem> Canales;
        public List<SelectListItem> Modelo;
        public List<SelectListItem> SubCanales;
        public List<SelectListItem> Planes;
        public List<SelectListItem> Contratos;
        public List<SelectListItem> TipoTransferencias;
        public List<ReporteTrasnferenciasClass> lDatos;

        public ReporteTransferenciasViewModel()
        {
            this.Sucursal = new List<SelectListItem>();
            this.Canales = new List<SelectListItem>();
            this.SubCanales = new List<SelectListItem>();
            this.Planes = new List<SelectListItem>();
            this.Contratos = new List<SelectListItem>();
            this.Modelo = new List<SelectListItem>();

            this.TipoTransferencias = new List<SelectListItem>();
            TipoTransferencias.Add(new SelectListItem() { Text = "Autorizada", Value = "1" });
            TipoTransferencias.Add(new SelectListItem() { Text = "No Autorizada", Value = "2" });
            TipoTransferencias.Add(new SelectListItem() { Text = "Director-Asesor/Homologación", Value = "3" });
            TipoTransferencias.Insert(0, new SelectListItem() { Value = "-1", Text = "Sin Selección" });
            this.lDatos = new List<ReporteTrasnferenciasClass>();
        }

    }
}
