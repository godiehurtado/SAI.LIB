using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Entidades.Informacion;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class AuditoriaModulos
    {
        private SAI_Entities tabla = new SAI_Entities();

        
        /// <summary>
        /// Funcion que se encarga de inserta en la base de Datos la auditoria para realizar el seguimiento
        /// </summary>
        /// <param name="tablaModuloProceso">Modulo que se va afectar</param>
        /// <param name="tipoModificacion">Tipo de accion q se va a realizar (Insercion = 1, Actualizacion = 2, Modificacion = 3, Eliminacion = 4)</param>
        /// <param name="fechaInicio">Fecha q se incicio la accion. (Solo aplica para cuando se va a inserta un Nuevo registro)</param>
        /// <param name="fechaFinal">Fecha en que se termino el cambio</param>
        /// <param name="observacion">Observacion del cambio. (Solo aplica para la eliminacion de un registro)</param>
        /// <param name="primeraVersion">Como se encuentra actualmente antes de realizar la accion requerida</param>
        /// <param name="versionFinal">Como queda los datos despues de realizar la acción</param>
        /// <param name="user">Usuario que esta realizando la tarea</param>
        public void InsertarAuditoria(int tablaModuloProceso, int tipoModificacion, DateTime? fechaInicio, DateTime fechaFinal, string observacion, string primeraVersion,
            string versionFinal, string user)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// Funcion que se encarga de traer los datos de la tabla de Auditoria
        /// </summary>
        /// <returns>Listado de Objetos</returns>
        public List<Auditoria> ListarAuditoria(int idTabla, DateTime fechaInicio, DateTime fechaFin, int idEvento, List<int> segmentos)
        {
            List<Auditoria> auditoria = tabla.Auditorias.Where(x => (x.id_TablaAuditada == idTabla || idTabla == 0) &&
                (x.Fecha >= fechaInicio) && (x.Fecha <= fechaFin) && (x.id_EventoTabla == idEvento || idEvento == 0) && segmentos.Contains(x.id_Segmento)).ToList();
            return auditoria;
        }
        
        public List<TablaAuditada> ListarTablasAuditadas()
        {
            return tabla.TablaAuditadas.ToList();
        }

        public List<EventoTabla> ListarEventosTabla()
        {
            return tabla.EventoTablas.ToList();
        }
    }    
}