using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Reportes
{
    public class ReporteBeneficiarioClass
    {
        public string CLAVE { get; set; }
        public string SUCURSAL { get; set; }
        public string PRODUCTO { get; set; }
        public string PLAN { get; set; }
        public string TIPO_CONTRATO { get; set; }
        public string BENEFICIARIOS_NUEVOS { get; set; }
        public string EQUIVALENCIAS_POR_NUEVOS { get; set; }
        public string BENEFICIARIOS_AÑO_ACTUAL { get; set; }
        public string BENEFICIARIOS_AÑO_ANTERIOR { get; set; }
        //public string DIFERENCIA_BENEFICIARIOS { get; set; }
        //public string EQUIVALENCIAS_POR_DIFERENCIA_DE_BENEFICIARIOS { get; set; }
        //public string BENEFICIARIOS_TRANSFERIDOS { get; set; }
        //public string EQUIVALENCIAS_POR_TRANSFERENCIAS { get; set; } 
        //public string BENEFICIARIOS_CON_EXCEPCION_DE_TRANSFERENCIA { get; set; }
        //public string EQUIVALENCIAS_POR_EXCEPCIONES { get; set; }
        public string TOTAL_BENEFICIARIOS_NETOS { get; set; }
        public string TOTAL_EQUIVALENCIAS_POR_NETOS { get; set; }
    }
}
