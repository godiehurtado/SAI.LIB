using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class FactorxNotas
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de FactorxNotas
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        public List<FactorxNota> ListarFactorxNotas()
        {
            return tabla.FactorxNotas.Include("Modeloes").Include("TipoEscala").Where(f => f.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de FactorxNotas por id
        /// </summary>
        /// <param name="id">Id de la FactorxNota a consultar</param>
        /// <returns>Lista de objectos FactorxNota</returns>
        public List<FactorxNota> ListarFactorxNotasPorId(int id)
        {
            return tabla.FactorxNotas.Include("Modeloes").Include("TipoEscala").Where(f => f.id == id && f.id > 0).ToList();
        }

        public List<PeriodoFactorxNota> ListarPeriodoFactorxNotasPorFactor(int id)
        {
            return tabla.PeriodoFactorxNotas.Where(f => f.factorxnota_id == id).ToList();
        }

        public List<FactorxNotaDetalle> ListarFactorxNotaDetallesPorPeriodo(int id)
        {
            return tabla.FactorxNotaDetalles.Where(f => f.periodofactorxnota_id == id).ToList();
        }

        /// <summary>
        /// Guarda un registro de FactorxNota
        /// </summary>
        /// <param name="factorxNota">Objecto FactorxNota a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarFactorxNota(FactorxNota factorxNota, string Username)
        {
            if (tabla.FactorxNotas.Where(f => f.nombre == factorxNota.nombre).ToList().Count() == 0)
            {

                tabla.FactorxNotas.AddObject(factorxNota);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorxNota,
    SegmentosInsercion.Personas_Y_Pymes, null, factorxNota);
                tabla.SaveChanges();
                return factorxNota.id;
            }
            return 0;
        }

        public int InsertarPeriodoFactorxNota(PeriodoFactorxNota periodo, string Username)
        {
            //int resultado = 0;
            if (tabla.PeriodoFactorxNotas.Where(f => f.fechaIni == periodo.fechaIni && f.fechaFin == periodo.fechaFin && f.factorxnota_id == periodo.factorxnota_id).ToList().Count() == 0)
            {
                tabla.PeriodoFactorxNotas.AddObject(periodo);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.PeriodoFactorxNota,
    SegmentosInsercion.Personas_Y_Pymes, null, periodo);
                tabla.SaveChanges(); //return 0;
                return periodo.id;
            }
            //resultado = tabla.PeriodoFactorxNotas.Where(f => f.fechaIni == periodo.fechaIni && f.fechaFin == periodo.fechaFin && f.factorxnota_id == periodo.factorxnota_id).ToList()[0].id;
            return 0;
        }

        public int InsertarFactorxNotaDetalle(FactorxNotaDetalle detalle, string Username)
        {
            var detalleInsert = tabla.FactorxNotaDetalles.Where(d => d.factor == detalle.factor && d.nota == detalle.nota && d.periodofactorxnota_id == detalle.periodofactorxnota_id).ToList();
            if (detalleInsert.Count == 0)
            {
                tabla.FactorxNotaDetalles.AddObject(detalle);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorxNotaDetalle,
    SegmentosInsercion.Personas_Y_Pymes, null, detalle);
                tabla.SaveChanges();
                return detalle.id;
            }
            return 0;
        }

        /// <summary>
        /// Actualiza un registro de FactorxNota
        /// </summary>
        /// <param name="id">Id del FactorxNota a modificar</param>
        /// <param name="factorxNota">Objeto FactorxNota utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarFactorxNota(int id, FactorxNota factorxNota, string Username)
        {
            var factorxNotaActual = this.tabla.FactorxNotas
                .Where(f => f.id != id && f.nombre == factorxNota.nombre && f.tipoescala_id == factorxNota.tipoescala_id).ToList();
            if (factorxNotaActual.Count == 0)
            {
                var escala = tabla.FactorxNotas.Single(f => f.id == id);
                var pValorAntiguo = escala;
                escala.nombre = factorxNota.nombre;
                escala.tipoescala_id = factorxNota.tipoescala_id;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.FactorxNota,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, escala);
                tabla.SaveChanges();
                return escala.id;
            }
            return 0;
        }

        /// <summary>
        /// Elimina un registro de FactorxNota
        /// </summary>
        /// <param name="id">Id del FactorxNota a eliminar</param>
        /// <param name="factorxNota">Objecto FactorxNota utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarFactorxNota(int id, FactorxNota factorxNota, string Username)
        {
            var factorxNotaActual = this.tabla.FactorxNotas.Where(f => f.id == id).First();
            tabla.DeleteObject(factorxNotaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorxNota,
    SegmentosInsercion.Personas_Y_Pymes, factorxNotaActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string EliminarPeriodoFactorxNota(int id, PeriodoFactorxNota periodo, string Username)
        {
            var periodoActual = this.tabla.PeriodoFactorxNotas.Where(p => p.id == id && p.id > 0).First();
            tabla.DeleteObject(periodoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.PeriodoFactorxNota,
    SegmentosInsercion.Personas_Y_Pymes, periodoActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string EliminarFactorxNotaDetalle(int id, FactorxNotaDetalle detalle, string Username)
        {
            var detalleActual = this.tabla.FactorxNotaDetalles.Where(f => f.id == id && f.id > 0).First();
            tabla.DeleteObject(detalleActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorxNotaDetalle,
    SegmentosInsercion.Personas_Y_Pymes, detalleActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

