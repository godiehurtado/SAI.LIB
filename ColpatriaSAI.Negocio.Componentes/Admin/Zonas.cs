using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Zonas
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Guarda un registro de zona
        /// </summary>
        /// <param name="zona">Objecto Zona a insertar</param>
        /// <returns>Numero de registros guardados</returns>
        public int InsertarZona(Zona zona, string Username)
        {
            int resultado = 0;
            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Zonas.Where(Zona => Zona.nombre == zona.nombre).ToList().Count() == 0)
            {
                tabla.Zonas.AddObject(zona);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, 
                    tablasAuditadas.Zona, SegmentosInsercion.Empresas_Y_Estado, null, zona);
                resultado = tabla.SaveChanges();
            }
            return resultado;
                
        }

        /// <summary>
        /// Obtiene listado de zonas
        /// </summary>
        /// <returns>Lista de zonas</returns>
        public List<Zona> ListarZonas()
        {
            return tabla.Zonas.Where(Zona => Zona.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de zonas por id
        /// </summary>
        /// <returns>Lista de zonas</returns>
        public List<Zona> ListarZonasPorId(int idZona)
        {
            return tabla.Zonas.Where(Zona => Zona.id == idZona).ToList();
        }

        /// <summary>
        /// Actualiza un registro de zona
        /// </summary>
        /// <param name="id">Id de la zona a modificar</param>
        /// <param name="zona">Objeto Zona utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarZona(int id, Zona zona, string Username)
        {
            int resultado = 0;
            var zonaActual = this.tabla.Zonas.Where(Zona => Zona.id == id).First();
            var pValorAntiguo = zonaActual;
            if (this.tabla.Zonas.Where(Zona => Zona.nombre == zona.nombre && Zona.id != id).ToList().Count() == 0)
            {
                zonaActual.nombre = zona.nombre;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username,
                    tablasAuditadas.Zona, SegmentosInsercion.Empresas_Y_Estado, pValorAntiguo, zonaActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de zona
        /// </summary>
        /// <param name="id">Id de la zona a eliminar</param>
        /// <param name="zona">Objecto Zona utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarZona(int id, Zona zona, string Username)
        {
            var zonaActual = this.tabla.Zonas.Where(Zona => Zona.id == id).First();
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Zona,
                SegmentosInsercion.Empresas_Y_Estado, zonaActual, null);
            tabla.DeleteObject(zonaActual);
            try {
                tabla.SaveChanges();
            }
            catch (Exception ex) {
                return ex.Message;
            }
            return "";
        }
    }
}

