using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class TipoEndosos
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de las redes
        /// </summary>
        /// <returns>Lista de redes</returns>
        public List<TipoEndoso> ListarTipoEndoso()
        {
            return tabla.TipoEndosoes.Where(TipoEndoso => TipoEndoso.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de los tipos de endoso por id
        /// </summary>
        /// <param name="id">Id de la red a consultar</param>
        /// <returns>Lista de objectos TipoEndoso</returns>
        public List<TipoEndoso> ListarTipoEndososPorId(int id)
        {
            return tabla.TipoEndosoes.Where(TipoEndoso => TipoEndoso.id == id && TipoEndoso.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de TipoEndoso
        /// </summary>
        /// <param name="tipoEndoso">Objecto TipoEndoso a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarTipoEndoso(TipoEndoso tipoEndoso, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.TipoEndosoes.Where(TipoEndoso => TipoEndoso.nombre == tipoEndoso.nombre).ToList().Count() == 0)
            {
                tabla.TipoEndosoes.AddObject(tipoEndoso);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.TipoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, null, tipoEndoso);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de TipoEndoso
        /// </summary>
        /// <param name="id">Id del TipoEndoso a modificar</param>
        /// <param name="tipoEndoso">Objeto TipoEndoso utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarTipoEndoso(int id, TipoEndoso tipoEndoso, string Username)
        {
            var tipoEndosoActual = this.tabla.TipoEndosoes.Where(TipoEndoso => TipoEndoso.id == id && TipoEndoso.id > 0).First();
            var pValorAntiguo = tipoEndosoActual;
            tipoEndosoActual.nombre = tipoEndoso.nombre;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.TipoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tipoEndosoActual);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de TipoEndoso
        /// </summary>
        /// <param name="id">Id del TipoEndoso a eliminar</param>
        /// <param name="tipoEndoso">Objecto TipoEndoso utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarTipoEndoso(int id, TipoEndoso tipoEndoso, string Username)
        {
            var tipoEndosoActual = this.tabla.TipoEndosoes.Where(Red => Red.id == id && Red.id > 0).First();
            tabla.DeleteObject(tipoEndosoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.TipoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, tipoEndosoActual, null);
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

