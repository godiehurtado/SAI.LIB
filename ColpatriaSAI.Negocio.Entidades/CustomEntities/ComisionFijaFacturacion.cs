using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ColpatriaSAI.Negocio.Entidades.ResultadoProcedimientos
{
    public class ComisionFijaFacturacion
    {

        [DisplayName("Compania Asesor")]
        public string Compania { get; set; }
        [DisplayName("Clave")]
        public int Clave { get; set; }
        public string Canal { get; set; }
        public string SubCanal { get; set; }
        [DisplayName("% Participacion")]
        public decimal PorcentajeParticipacion { get; set; }
        public string TipoDocumento { get; set; }
        [DisplayName("Documento Beneficiario")]
        public string Documento { get; set; }
        [DisplayName("Edad Beneficiario")]
        public int edad { get; set; }
        public string EstadoBeneficiario { get; set; }
        public string Contrato { get; set; }
        public string SubContrato { get; set; }
        public string ProductoDetalle { get; set; }
        public string PlanDetalle { get; set; }
        [DisplayName("Tipo contrato")]
        public string TipoContrato { get; set; }
        public string Factura { get; set; }
        public string IdNovedad { get; set; }
        public string Novedad { get; set; }
        public Decimal ValorFactura { get; set; }
        public DateTime FechaFactura { get; set; }
        #region C44102 EHBV SE INCLUYE EL CAMPO PERIODO
        /// <summary>
        /// Determina el periodo de facturacion
        /// </summary>
        public DateTime Periodo { get; set; }

        #endregion C44102 EHBV SE INCLUYE EL CAMPO PERIODO
        public int TalentosNuevos { get; set; }
        public int TalentosNetos { get; set; }
        public Decimal PorcentajeComisionFijaSinExcepcion { get; set; }
        public Decimal ValorComisionFija { get; set; }
        public Decimal PorcentajeComisionVariable { get; set; }
        public Decimal ValorComisionVariable { get; set; }
        public string TipoExcepcion { get; set; }
        public Decimal PorcentajeComisionFijaConExcepcion { get; set; }
        public Decimal ValorComisionExcepcion { get; set; }
        public Decimal PorcentajeComisionTotal { get; set; }
        public Decimal ValorComisionTotal { get; set; }
    }
}
