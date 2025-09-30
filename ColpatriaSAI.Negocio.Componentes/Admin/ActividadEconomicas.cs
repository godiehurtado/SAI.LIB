using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class ActividadEconomicas
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Funcion que se encarga de traer todas las Actividades Economicas
        /// </summary>
        /// <returns>Listado de Actividades Economicas</returns>
        public List<ActividadEconomica> ListarActividadEconomica()
        {
            return tabla.ActividadEconomicas.Where(ActividadEconomica => ActividadEconomica.id > 0).ToList();
        }

        /// <summary>
        /// Funcion que se encarga de traer las actividades Economicas por El ID
        /// </summary>
        /// <param name="idActividadEconomica">ID de las actividades</param>
        /// <returns>Listado de las Actividades</returns>
        public List<ActividadEconomica> ListarActividadEconomicasPorId(int idActividadEconomica)
        {
            return tabla.ActividadEconomicas.Where(ActividadEconomica => ActividadEconomica.id == idActividadEconomica && ActividadEconomica.id > 0).ToList();
        }

        /// <summary>
        /// Funcion que se encarga de traer las actividades Economicas por Compañia
        /// </summary>
        /// <param name="CompaniaID">Compañia con la que se va a trabajar</param>
        /// <returns>Listado de las Actividades Economicas por Compañia</returns>
        public List<ActividadEconomica> ListarActividadEconomicasPorCompania(int compania)
        {
            return tabla.ActividadEconomicas.Where(ActividadEconomica => ActividadEconomica.CompaniaID == compania).OrderBy(ActividadEconomica => ActividadEconomica.ActividadID).ToList();
        }
    }
}

