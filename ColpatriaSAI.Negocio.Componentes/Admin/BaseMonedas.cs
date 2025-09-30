using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class BaseMonedas
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de las Monedas
        /// </summary>
        /// <returns>Lista de Monedas</returns>
        public List<BaseMoneda> ListarBaseMoneda()
        {
            return tabla.BaseMonedas.Include("Moneda").Where(BaseMoneda => BaseMoneda.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de las BaseMonedas por id
        /// </summary>
        /// <param name="idRed">Id de la BaseMoneda a consultar</param>
        /// <returns>Lista de objectos BaseMoneda</returns>
        public List<BaseMoneda> ListarBaseMonedasPorId(int id)
        {
            return tabla.BaseMonedas.Where(Moneda => Moneda.id == id && Moneda.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de BaseMoneda
        /// </summary>
        /// <param name="zona">Objecto BaseMoneda a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarBaseMoneda(BaseMoneda baseMoneda, string Username)
        {
            int resultado = 0;
            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.BaseMonedas.Where(BM =>
                (BM.fecha_inicioVigencia <= baseMoneda.fecha_inicioVigencia && BM.fecha_finVigencia > baseMoneda.fecha_inicioVigencia)
                || (BM.fecha_inicioVigencia < baseMoneda.fecha_inicioVigencia && BM.fecha_finVigencia > baseMoneda.fecha_finVigencia)
                || (BM.fecha_inicioVigencia > baseMoneda.fecha_inicioVigencia && BM.fecha_finVigencia < baseMoneda.fecha_finVigencia)
                || (BM.fecha_inicioVigencia < baseMoneda.fecha_finVigencia && BM.fecha_finVigencia > baseMoneda.fecha_finVigencia)).ToList().Count() == 0)
            {
                tabla.BaseMonedas.AddObject(baseMoneda);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.BaseMoneda,
    SegmentosInsercion.Personas_Y_Pymes, null, baseMoneda);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de BaseMoneda
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a modificar</param>
        /// <param name="zona">Objeto BaseMoneda utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarBaseMoneda(int id, BaseMoneda baseMoneda, string UserName)
        {
            int resultado = 0;
            if (tabla.BaseMonedas.Where(BM => (
                (BM.fecha_inicioVigencia <= baseMoneda.fecha_inicioVigencia && BM.fecha_finVigencia > baseMoneda.fecha_inicioVigencia)
                || (BM.fecha_inicioVigencia < baseMoneda.fecha_inicioVigencia && BM.fecha_finVigencia > baseMoneda.fecha_finVigencia)
                || (BM.fecha_inicioVigencia > baseMoneda.fecha_inicioVigencia && BM.fecha_finVigencia < baseMoneda.fecha_finVigencia)
                || (BM.fecha_inicioVigencia < baseMoneda.fecha_finVigencia && BM.fecha_finVigencia > baseMoneda.fecha_finVigencia))
                && (BM.id != baseMoneda.id)).ToList().Count() == 0)
            //if (tabla.BaseMonedas.Where(BaseMoneda => BaseMoneda.fecha_inicioVigencia == baseMoneda.fecha_inicioVigencia && BaseMoneda.fecha_finVigencia == baseMoneda.fecha_finVigencia && BaseMoneda.moneda_id == baseMoneda.moneda_id).ToList().Count() == 1)
            {
                var baseMonedaActual = this.tabla.BaseMonedas.Where(BaseMoneda => BaseMoneda.id == id && BaseMoneda.id > 0).First();
                var baseMonedaAntigua = baseMonedaActual;
                baseMonedaActual.fecha_inicioVigencia = baseMoneda.fecha_inicioVigencia;
                baseMonedaActual.fecha_finVigencia = baseMoneda.fecha_finVigencia;
                baseMonedaActual.@base = baseMoneda.@base;
                baseMonedaActual.moneda_id = baseMoneda.moneda_id;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, UserName, tablasAuditadas.BaseMoneda, SegmentosInsercion.Personas_Y_Pymes, baseMonedaAntigua, baseMonedaActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de BaseMoneda
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a eliminar</param>
        /// <param name="zona">Objecto BaseMoneda utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarBaseMoneda(int id, BaseMoneda baseMoneda, string Username)
        {
            var baseMonedaActual = this.tabla.BaseMonedas.Where(BaseMoneda => BaseMoneda.id == id && BaseMoneda.id > 0).First();
            tabla.DeleteObject(baseMonedaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.BaseMoneda,
    SegmentosInsercion.Personas_Y_Pymes, baseMonedaActual, null);
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

