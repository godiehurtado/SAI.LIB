using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entidades = ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Validadores.ModeloComision
{
    public class ValidadorModeloComision
    {
        public static bool EditableSegunVigenciaModelo(DateTime fechadesdeModelo)
        {
            if ((DateTime.Now.Month < fechadesdeModelo.Month && DateTime.Now.Year == fechadesdeModelo.Year)
                || DateTime.Now.Year < fechadesdeModelo.Year)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
