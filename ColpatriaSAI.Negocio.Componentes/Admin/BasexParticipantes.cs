using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class BasexParticipantes
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de BasexParticipante
        /// </summary>
        /// <returns>Lista de BasexParticipante</returns>
        public List<BasexParticipante> ListarBasexParticipantes()
        {
            return tabla.BasexParticipantes.Include("Participante.BasexParticipantes").ToList();
        }

        /// <summary>
        /// Obtiene listado de los Participantes
        /// </summary>
        /// <returns>Lista de Participantes</returns>
        public List<Participante> ListarBaseYParticipantes()
        {
            return tabla.Participantes.Include("Participante.BasexParticipantes").ToList();
        }

        /// <summary>
        /// Obtiene listado de BasexParticipante por id
        /// </summary>
        /// <param name="idRed">Id de la BasexParticipante a consultar</param>
        /// <returns>Lista de objectos BasexParticipante</returns>
        public List<BasexParticipante> ListarBasexParticipantesPorId(int id)
        {
            return tabla.BasexParticipantes.Where(BasexParticipante => BasexParticipante.id == id).ToList();
        }

        /// <summary>
        /// Guarda un registro de BasexParticipante
        /// </summary>
        /// <param name="zona">Objecto BasexParticipante a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarBasexParticipante(BasexParticipante basexParticipante, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.BasexParticipantes.Where(BasexParticipante => BasexParticipante.participante_id == basexParticipante.participante_id).ToList().Count() == 0)
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.BasexParticipante,
    SegmentosInsercion.Personas_Y_Pymes, null, basexParticipante);
                tabla.BasexParticipantes.AddObject(basexParticipante);
                resultado = tabla.SaveChanges();
            }
            return resultado;

        }

        /// <summary>
        /// Actualiza un registro de BasexParticipante
        /// </summary>
        /// <param name="id">Id de BasexParticipante a modificar</param>
        /// <param name="zona">Objeto BasexParticipante utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarBasexParticipante(int id, BasexParticipante basexParticipante, string Username)
        {
            var basexParticipanteActual = this.tabla.BasexParticipantes.Where(BasexParticipante => BasexParticipante.id == id).First();
            var basexParticipanteAntigua = basexParticipanteActual;
            basexParticipanteActual.@base = basexParticipante.@base;
            basexParticipanteActual.salario = basexParticipante.salario;
            basexParticipanteActual.@base = basexParticipante.@base;
            basexParticipanteActual.participante_id = basexParticipante.participante_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.BasexParticipante,
                SegmentosInsercion.Personas_Y_Pymes, basexParticipanteAntigua, basexParticipanteActual);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de BasexParticipante
        /// </summary>
        /// <param name="id">Id de BasexParticipante a eliminar</param>
        /// <param name="zona">Objecto BasexParticipante utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarBasexParticipante(int id, BasexParticipante basexParticipante, string Username)
        {
            var basexParticipanteActual = this.tabla.BasexParticipantes.Where(BasexParticipante => BasexParticipante.id == id).First();
            tabla.DeleteObject(basexParticipanteActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.BasexParticipante,
    SegmentosInsercion.Personas_Y_Pymes, basexParticipanteActual, null);
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

