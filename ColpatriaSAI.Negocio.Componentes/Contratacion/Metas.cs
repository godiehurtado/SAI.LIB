using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.Objects;

namespace ColpatriaSAI.Negocio.Componentes.Contratacion
{
    public class Metas
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Meta> ListarMetas()
        {
            //Relacion de campos de tablas respecto a las variables de navegación en el Modelo de datos x tabla

            return tabla.Metas.Include("TipoMeta").Include("TipoMedida").Include("TipoMetaCalculo")
                .Where(Meta => Meta.id > 0).ToList();

        }

        public List<Meta> ListarMetasPorId(int idMeta)
        {
            return tabla.Metas.Include("TipoMeta").Include("TipoMedida").Include("TipoMetaCalculo")
                .Where(Meta => Meta.id == idMeta && Meta.id > 0).ToList();
        }

        public List<Meta> ListarMetasMensuales(int idMeta)
        {
            List<Meta> meta = tabla.Metas.ToList<Meta>();
            List<Meta> meta1 = tabla.Metas.ToList<Meta>();

            List<Meta> metasMensuales = (
                from m in meta
                where m.meta_id.HasValue == false &&
                !(
                    from m1 in meta1
                    where m1.meta_id.HasValue == true && m1.id != idMeta
                    select m1.meta_id
                    )
                .Contains(m.id)
                select m
                ).ToList();

            return metasMensuales;
        }

        public int InsertarMeta(Meta meta, string Username)
        {
            int resultado = 0;
            if (tabla.Metas.Where(m => m.nombre == meta.nombre).ToList().Count() == 0)
            {
                tabla.Metas.AddObject(meta);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Meta,
    SegmentosInsercion.Personas_Y_Pymes, null, meta);
                tabla.SaveChanges();
                resultado = (from m in tabla.Metas
                             select m.id).Max();
            }
            return resultado;
        }

        public int ActualizarMeta(int id, Meta meta, string Username)
        {
            int resultado = 0;
            var metaActual = this.tabla.Metas.Where(m => m.id == id).First();
            var pValorAntiguo = metaActual;
            metaActual.nombre = meta.nombre;
            metaActual.tipoMedida_id = meta.tipoMedida_id;
            metaActual.tipoMeta_id = meta.tipoMeta_id;
            metaActual.automatica = meta.automatica;
            metaActual.tipoMetaCalculo_id = meta.tipoMetaCalculo_id;
            metaActual.meta_id = meta.meta_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Meta,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, metaActual);
            tabla.SaveChanges();
            resultado = metaActual.id;
            return resultado;
        }

        public int ActualizarMetaAcumulada(int id, Meta meta, string Username)
        {
            int resultado = 0;

            var metaAcumuladaActual = this.tabla.Metas.Where(m => m.meta_id == id).FirstOrDefault();
            var pValorAntiguo = metaAcumuladaActual;

            // Hay una meta acumulada asociada a la meta recibida
            if (metaAcumuladaActual != null)
            {
                metaAcumuladaActual.tipoMedida_id = meta.tipoMedida_id;
                metaAcumuladaActual.tipoMeta_id = meta.tipoMeta_id;
                metaAcumuladaActual.automatica = meta.automatica;
                metaAcumuladaActual.tipoMetaCalculo_id = meta.tipoMetaCalculo_id;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Meta,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, metaAcumuladaActual);
                tabla.SaveChanges();
                resultado = metaAcumuladaActual.id;
            }
            else if (metaAcumuladaActual == null)
            {
                resultado = meta.id;
            }
            return resultado;
        }

        public int EliminarMeta(int idMeta, string Username)
        {
            int resultado = 0;

            if (!this.ValidarIntegridadMeta(idMeta))
            {

                //ELIMINAMOS REGISTRO HIJOS
                List<MetaCompuesta> metaCompuestaList = tabla.MetaCompuestas.Where(x => x.metaOrigen_id == idMeta).ToList();
                foreach (MetaCompuesta metaCompuesta in metaCompuestaList)
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.MetaCompuesta,
    SegmentosInsercion.Personas_Y_Pymes, metaCompuesta, null);
                    tabla.MetaCompuestas.DeleteObject(metaCompuesta);
                }

                List<ProductosMeta> productosMetaList = tabla.ProductosMetas.Where(x => x.meta_id == idMeta).ToList();
                foreach (ProductosMeta productosMeta in productosMetaList)
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ProductosMeta,
    SegmentosInsercion.Personas_Y_Pymes, productosMeta, null);
                    tabla.ProductosMetas.DeleteObject(productosMeta);
                }

                //ELIMINAMOS EL REGISTRO PADRE
                Meta meta = tabla.Metas.Where(m => m.id == idMeta).FirstOrDefault();
                tabla.Metas.DeleteObject(meta);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Meta,
    SegmentosInsercion.Personas_Y_Pymes, meta, null);
                tabla.SaveChanges();
                resultado = 1;
            }

            return resultado;
        }

        public Boolean ValidarIntegridadMeta(int idMeta)
        {
            Boolean integridadReferencial = false;

            //Meta Compuesta
            var metaCompuesta = tabla.MetaCompuestas.Where(x => x.metaDestino_id == idMeta).ToList();

            if (metaCompuesta.Count() > 0)
                integridadReferencial = true;

            //Detalle Presupuesto
            var presupuesto = tabla.DetallePresupuestoes.Where(x => x.meta_id == idMeta).ToList();

            if (presupuesto.Count() > 0)
                integridadReferencial = true;

            //Contratacion / Modelos x Meta
            var contratacion = tabla.ModeloxMetas.Where(x => x.meta_id == idMeta).ToList();

            if (contratacion.Count() > 0)
                integridadReferencial = true;

            //Meta x Nodo
            var metaNodo = tabla.MetaxNodoes.Where(x => x.meta_id == idMeta).ToList();

            if (metaNodo.Count() > 0)
                integridadReferencial = true;

            return integridadReferencial;
        }

        public List<ProductosMeta> ListarProductosMetaPorId(int idMeta)
        {
            return tabla.ProductosMetas.Include("Ramo").Include("Compania").Include("Producto").Include("LineaNegocio").Include("ModalidadPago").Include("Amparo").Where(x => x.meta_id == idMeta).ToList();
        }

        public int InsertarProductoMeta(ProductosMeta productometa, string Username)
        {
            int resultado = 0;

            var totalRegistro = tabla.ProductosMetas.Where(x => x.meta_id == productometa.meta_id && x.compania_id == productometa.compania_id && x.ramo_id == productometa.ramo_id && x.producto_id == productometa.producto_id && x.lineaNegocio_id == productometa.lineaNegocio_id && x.modalidadPago_id == productometa.modalidadPago_id && x.amparo_id == productometa.amparo_id).ToList().Count();

            if (totalRegistro == 0)
            {
                tabla.ProductosMetas.AddObject(productometa);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ProductosMeta,
    SegmentosInsercion.Personas_Y_Pymes, null, productometa);
                tabla.SaveChanges();
                resultado = (from pm in tabla.ProductosMetas
                             select pm.id).Max();
            }
            return resultado;
        }

        public int ActualizarProductoMeta(int id, ProductosMeta productometa, string Username)
        {
            int resultado = 0;
            var productoMetaActual = this.tabla.ProductosMetas.Where(m => m.id == id).First();
            var pValorAntiguo = productoMetaActual;
            productoMetaActual.compania_id = productometa.compania_id;
            productoMetaActual.ramo_id = productometa.ramo_id;
            productoMetaActual.producto_id = productometa.producto_id;
            productoMetaActual.lineaNegocio_id = productometa.lineaNegocio_id;
            productoMetaActual.modalidadPago_id = productometa.modalidadPago_id;
            productoMetaActual.amparo_id = productometa.amparo_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ProductosMeta,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, productoMetaActual);
            tabla.SaveChanges();
            resultado = productoMetaActual.id;
            return resultado;
        }

        public int EliminarProductoMeta(int idMeta, string Username)
        {
            int resultado = 0;

            //ELIMINAMOS EL REGISTRO PADRE
            ProductosMeta productoMeta = tabla.ProductosMetas.Where(m => m.id == idMeta).FirstOrDefault();
            tabla.ProductosMetas.DeleteObject(productoMeta);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ProductosMeta,
    SegmentosInsercion.Personas_Y_Pymes, productoMeta, null);
            tabla.SaveChanges();

            return resultado;
        }

        public List<MetaCompuesta> ListarMetaCompuestaPorId(int idMeta)
        {
            return tabla.MetaCompuestas.Include("Meta1.TipoMeta").Include("Meta1.TipoMedida").Where(x => x.metaOrigen_id == idMeta).ToList();
        }

        public int InsertarMetaCompuesta(int idMetaDestino, int idMetaOrigen, string Username)
        {
            int resultado = 0;

            MetaCompuesta metaCompuesta = new MetaCompuesta()
            {
                metaDestino_id = idMetaDestino,
                metaOrigen_id = idMetaOrigen
            };

            tabla.MetaCompuestas.AddObject(metaCompuesta);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.MetaCompuesta,
    SegmentosInsercion.Personas_Y_Pymes, null, metaCompuesta);
            tabla.SaveChanges();
            resultado = (from mc in tabla.MetaCompuestas
                         select mc.id).Max();

            return resultado;
        }

        public int EliminarMetaCompuesta(int idMeta, string Username)
        {
            int resultado = 0;

            //ELIMINAMOS EL REGISTRO PADRE
            MetaCompuesta meta = tabla.MetaCompuestas.Where(m => m.id == idMeta).FirstOrDefault();
            tabla.MetaCompuestas.DeleteObject(meta);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.MetaCompuesta,
    SegmentosInsercion.Personas_Y_Pymes, meta, null);
            tabla.SaveChanges();

            return resultado;
        }

        //EL REQUERIMIENTO SE DIJO QUE NO SE IBA HACER. 
        //SE DEJA COMENTADO POR SI HAY LA POSIBILIDAD QUE EL NEGOCIO LO APRUEBE NUEVAMENTE
        /*public List<MetaValidacionCumplimiento> ListarMetaValidacionCumplimientoPorId(int idMeta)
        {
            return tabla.MetaValidacionCumplimientoes.Include("Meta1").Where(x => x.metaValidacion_id == idMeta).ToList();
        }

        public int InsertarMetaValidacionCumplimiento(int idMetaValidacion, int idMetaReponderacion)
        {
            int resultado = 0;

            MetaValidacionCumplimiento metaValidacionCumplimiento = new MetaValidacionCumplimiento()
            {
                metaValidacion_id = idMetaValidacion,
                metaReponderacion_id = idMetaReponderacion
            };

            tabla.MetaValidacionCumplimientoes.AddObject(metaValidacionCumplimiento);
            tabla.SaveChanges();

            return resultado;
        }

        public int EliminarMetaValidacionCumplimiento(int idMetaValidacion)
        {
            int resultado = 0;

            //ELIMINAMOS EL REGISTRO
            MetaValidacionCumplimiento metaValidacion = tabla.MetaValidacionCumplimientoes.Where(m => m.id == idMetaValidacion).FirstOrDefault();
            tabla.MetaValidacionCumplimientoes.DeleteObject(metaValidacion);
            tabla.SaveChanges();

            return resultado;
        }*/
    }

    public class TipoMetas
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<TipoMeta> ListarTipometas()
        {
            return tabla.TipoMetas.Where(TipoMeta => TipoMeta.id > 0).ToList();
        }

    }
}
