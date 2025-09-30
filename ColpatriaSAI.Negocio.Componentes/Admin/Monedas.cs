using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Monedas
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de las Monedas
        /// </summary>
        /// <returns>Lista de Monedas</returns>
        public List<Moneda> ListarMonedas()
        {
            return tabla.Monedas.Include("UnidadMedida").Include("Segmento").Where(Moneda => Moneda.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de las Moneda por id
        /// </summary>
        /// <param name="idRed">Id de la Moneda a consultar</param>
        /// <returns>Lista de objectos Moneda</returns>
        public List<Moneda> ListarMonedasPorId(int idMoneda)
        {
            return tabla.Monedas.Where(Moneda => Moneda.id == idMoneda && Moneda.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de Moneda
        /// </summary>
        /// <param name="zona">Objecto Moneda a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarMoneda(Moneda moneda, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Monedas.Where(Moneda => Moneda.nombre == moneda.nombre && Moneda.segmento_id == moneda.segmento_id && Moneda.unidadmedida_id == moneda.unidadmedida_id).ToList().Count() == 0)
            {
                tabla.Monedas.AddObject(moneda);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Moneda,
    SegmentosInsercion.Personas_Y_Pymes, null, moneda);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de Moneda
        /// </summary>
        /// <param name="id">Id de la Moneda a modificar</param>
        /// <param name="zona">Objeto Moneda utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarMoneda(int id, Moneda moneda, string Username)
        {
            int resultado = 0;
            if (tabla.Monedas.Where(Moneda => Moneda.nombre == moneda.nombre && Moneda.segmento_id == moneda.segmento_id && Moneda.unidadmedida_id == moneda.unidadmedida_id).ToList().Count() == 0)
            {
                var monedaActual = this.tabla.Monedas.Where(Moneda => Moneda.id == id && Moneda.id > 0).First();
                var pValorAntiguo = monedaActual;
                monedaActual.nombre = moneda.nombre;
                monedaActual.unidadmedida_id = moneda.unidadmedida_id;
                monedaActual.segmento_id = moneda.segmento_id;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Moneda,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, monedaActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de Moneda
        /// </summary>
        /// <param name="id">Id de la Moneda a eliminar</param>
        /// <param name="zona">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarMoneda(int id, Moneda moneda, string Username)
        {
            var monedaActual = this.tabla.Monedas.Where(Moneda => Moneda.id == id && Moneda.id > 0).First();
            tabla.DeleteObject(monedaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.CondicionxPremioSubregla,
    SegmentosInsercion.Personas_Y_Pymes, monedaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {

                return ex.Message;

            }

            return "";
        }

        /// <summary>
        /// Obtiene listado de las unidades de medida
        /// </summary>
        /// <returns>Lista de Unidades de medida</returns>
        public List<UnidadMedida> ListarUnidadesMedida()
        {
            return tabla.UnidadMedidas.Where(UnidadMedida => UnidadMedida.id > 0).ToList();
        }
    }
}