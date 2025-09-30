using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class EscalaNotas
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de EscalaNota
        /// </summary>
        /// <returns>Lista de EscalaNotas</returns>
        public List<EscalaNota> ListarEscalaNotas()
        {
            return tabla.EscalaNotas.Include("TipoEscala").Where(EscalaNota => EscalaNota.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de EscalaNota por id
        /// </summary>
        /// <param name="id">Id de la EscalaNota a consultar</param>
        /// <returns>Lista de objectos EscalaNota</returns>
        public List<EscalaNota> ListarEscalaNotasPorId(int id)
        {
            return tabla.EscalaNotas.Include("TipoEscala").Where(EscalaNota => EscalaNota.id == id && EscalaNota.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de EscalaNota
        /// </summary>
        /// <param name="escalaNota">Objecto EscalaNota a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarEscalaNota(EscalaNota escalaNota, string Username)
        {
            tabla.EscalaNotas.AddObject(escalaNota);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.EscalaNota,
    SegmentosInsercion.Personas_Y_Pymes, null, escalaNota);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Actualiza un registro de EscalaNota
        /// </summary>
        /// <param name="id">Id de la EscalaNota a modificar</param>
        /// <param name="escalaNota">Objeto EscalaNota utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarEscalaNota(int id, EscalaNota escalaNota, string Username)
        {
            var escalaNotaActual = this.tabla.EscalaNotas.Where(EscalaNota => EscalaNota.id == id && EscalaNota.id > 0).First();
            var pValorAntiguo = escalaNotaActual;
            escalaNotaActual.nota = escalaNota.nota;
            escalaNotaActual.porcentaje = escalaNota.porcentaje;
            escalaNotaActual.tipoEscala_id = escalaNota.tipoEscala_id;
            escalaNotaActual.limiteInferior = escalaNota.limiteInferior;
            escalaNotaActual.limiteSuperior = escalaNota.limiteSuperior;
            escalaNotaActual.fechaIni = escalaNota.fechaIni;
            escalaNotaActual.fechaFin = escalaNota.fechaFin;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.EscalaNota,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, escalaNotaActual);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de EscalaNota
        /// </summary>
        /// <param name="id">Id de la EscalaNota a eliminar</param>
        /// <param name="escalaNota">Objecto EscalaNota utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarEscalaNota(int id, EscalaNota escalaNota, string Username)
        {
            var escalaNotaActual = this.tabla.EscalaNotas.Where(EscalaNota => EscalaNota.id == id && EscalaNota.id > 0).First();
            tabla.DeleteObject(escalaNotaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.EscalaNota,
    SegmentosInsercion.Personas_Y_Pymes, null, escalaNotaActual);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {

                return ex.Message;

            }

            return "";
        }
    }
}

