using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class GrupoEndosos
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de GrupoEndoso
        /// </summary>
        /// <returns>Lista de GrupoEndoso</returns>
        public List<GrupoEndoso> ListarGrupoEndosos()
        {
            return tabla.GrupoEndosoes.ToList();
        }

        /// <summary>
        /// Obtiene listado de GrupoEndoso por id
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a consultar</param>
        /// <returns>Lista de objectos GrupoEndoso</returns>
        public List<GrupoEndoso> ListarGrupoEndososPorId(int id)
        {
            return tabla.GrupoEndosoes.Where(GrupoEndoso => GrupoEndoso.id == id).ToList();
        }

        /// <summary>
        /// Guarda un registro de GrupoEndoso
        /// </summary>
        /// <param name="grupoEndoso">Objecto GrupoEndoso a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarGrupoEndoso(GrupoEndoso grupoEndoso, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.GrupoEndosoes.Where(GrupoEndoso => GrupoEndoso.nombre == grupoEndoso.nombre).ToList().Count() == 0)
            {
                tabla.GrupoEndosoes.AddObject(grupoEndoso);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.GrupoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, null, grupoEndoso);
                resultado = tabla.SaveChanges();
            }
            return resultado;            
        }

        /// <summary>
        /// Actualiza un registro de GrupoEndoso
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a modificar</param>
        /// <param name="grupoEndoso">Objeto GrupoEndoso utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarGrupoEndoso(int id, GrupoEndoso grupoEndoso, string Username)
        {
            var grupoEndosoActual = this.tabla.GrupoEndosoes.Where(GrupoEndoso => GrupoEndoso.id == id).First();
            var pValorAntiguo = grupoEndosoActual;
            grupoEndosoActual.nombre = grupoEndoso.nombre;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.GrupoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, grupoEndosoActual);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de GrupoEndoso
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a eliminar</param>
        /// <param name="grupoEndoso">Objecto GrupoEndoso utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarGrupoEndoso(int id, GrupoEndoso grupoEndoso, string Username)
        {
            var grupoEndosoActual = this.tabla.GrupoEndosoes.Where(GrupoEndoso => GrupoEndoso.id == id).First();
            tabla.DeleteObject(grupoEndosoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.GrupoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, grupoEndosoActual, null);
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

