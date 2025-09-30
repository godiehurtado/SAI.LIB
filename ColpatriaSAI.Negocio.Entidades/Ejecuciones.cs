using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Entidades
{
    public class Ejecuciones
    {
        public int id_ejecucion;
        public DateTime? fechaIncioEjecucion;
        public DateTime? fechaFinEjecucion;
        public string estadoEjecucion;
        public int id_proceso;
        public string nombreProceso;
        public DateTime? fechaInicioProceso;
        public DateTime? fechaFinProceso;
        public string estadoProceso;
        public string detalleProceso;
    }
}
