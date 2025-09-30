using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Nivels
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de los Niveles
        /// </summary>
        /// <returns>Lista de Nievles</returns>
        public List<Nivel> ListarNivels()
        {
            return tabla.Nivels.Where(Nivel => Nivel.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de los Niveles por id
        /// </summary>
        /// <param name="idNivel">Id del Nivel a consultar</param>
        /// <returns>Lista de objectos Niveles</returns>
        public List<Nivel> ListarNivelsPorId(int idNivel)
        {
            return tabla.Nivels.Where(Nivel => Nivel.id == idNivel && Nivel.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de Nivel
        /// </summary>
        /// <param name="zona">Objecto Nivel a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarNivel(Nivel nivel, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Nivels.Where(Nivel => Nivel.nombre == nivel.nombre).ToList().Count() == 0)
            {
                tabla.Nivels.AddObject(nivel);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Nivel,
    SegmentosInsercion.Personas_Y_Pymes, null, nivel);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de Nivel
        /// </summary>
        /// <param name="id">Id del Nivel a modificar</param>
        /// <param name="nivel">Objeto Nivel utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarNivel(int id, Nivel nivel, string Username)
        {
            int resultado = 0;
            if (tabla.Nivels.Where(Nivel => Nivel.nombre == nivel.nombre).ToList().Count() == 0)
            {
                var nivelActual = this.tabla.Nivels.Where(Nivel => Nivel.id == id && Nivel.id > 0).First();
                var pValorAntiguo = nivelActual;
                nivelActual.nombre = nivel.nombre;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Nivel,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, nivelActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de Nivel
        /// </summary>
        /// <param name="id">Id del Nivel a eliminar</param>
        /// <param name="nivel">Objecto Nivel utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarNivel(int id, Nivel nivel, string Username)
        {
            var nivelActual = this.tabla.Nivels.Where(Nivel => Nivel.id == id && Nivel.id > 0).First();
            tabla.DeleteObject(nivelActual);

            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Nivel,
    SegmentosInsercion.Personas_Y_Pymes, nivelActual, null);
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

