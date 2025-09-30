using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Componentes.Comision
{
    public class ResultadoProcesoExtraccion
    {
        public EstadoEjecucionProceso Estado { get; set; }
        public int RegistrosAfectados { get; set; }
    }

    public enum EstadoEjecucionProceso { 
        Completo = 1,
        Abortado,
        CompletoConErrores,
        ErrorDesconocido
    }

    public class ResultadoOperacionBD
    {
        public int RegistrosAfectados { get; set; }
        public ResultadoOperacion Resultado { get; set; }
        public string MensajeError { get; set; }
        public object IdInsercion { get; set; }
    }

    public enum ResultadoOperacion
    {
        Error = 0,
        Exito = 1
    }
}
