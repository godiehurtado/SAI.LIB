using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Componentes.Utilidades
{
    public class Result
    {
        public class result
        {
            /// <summary>
            /// Mensaje del resultado
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// Tipo de resultado
            /// </summary>
            public tipoResultado resultado { get; set; }
        }

        /// <summary>
        /// Tipo de resultado a mostrar
        /// </summary>
        public enum tipoResultado
        { 
            resultOK = 0,
            resultNOK = 1
        }
    }
}
