using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class TipoEscalas
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de los tipos de escalas
        /// </summary>
        /// <returns>Lista de TipoEscala</returns>
        public List<TipoEscala> ListarTipoEscalas()
        {
            return tabla.TipoEscalas.Where(TipoEscala => TipoEscala.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de los tipos de escalas por id
        /// </summary>
        /// <param name="id">Id del TipoEscala a consultar</param>
        /// <returns>Lista de objetos TipoEscala</returns>
        public List<TipoEscala> ListarTipoEscalasPorId(int id)
        {
            return tabla.TipoEscalas.Where(TipoEscala => TipoEscala.id == id && TipoEscala.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de TipoEscala
        /// </summary>
        /// <param name="tipoEscala">Objeto TipoEscala a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarTipoEscala(TipoEscala tipoEscala, string Username)
        {
            tabla.TipoEscalas.AddObject(tipoEscala);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.TipoEscala,
    SegmentosInsercion.Personas_Y_Pymes, null, tipoEscala);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Actualiza un registro de TipoEscala
        /// </summary>
        /// <param name="id">Id del TipoEscala a modificar</param>
        /// <param name="tipoEscala">Objeto TipoEscala utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarTipoEscala(int id, TipoEscala tipoEscala, string Username)
        {
            var tipoEscalaActual = this.tabla.TipoEscalas.Where(TipoEscala => TipoEscala.id == id && TipoEscala.id > 0).First();
            var pValorAntiguo = tipoEscalaActual;
            tipoEscalaActual.nombre = tipoEscala.nombre;
            tipoEscalaActual.descripcion = tipoEscala.descripcion;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.TipoEscala,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tipoEscalaActual);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de TipoEscala
        /// </summary>
        /// <param name="id">Id del TipoEscala a eliminar</param>
        /// <param name="tipoEscala">Objeto TipoEscala utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarTipoEscala(int id, TipoEscala tipoEscala, string Username)
        {
            var tipoEscalaActual = this.tabla.TipoEscalas.Where(TipoEscala => TipoEscala.id == id && TipoEscala.id > 0).First();
            tabla.DeleteObject(tipoEscalaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.TipoEscala,
    SegmentosInsercion.Personas_Y_Pymes, tipoEscalaActual, null);
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

