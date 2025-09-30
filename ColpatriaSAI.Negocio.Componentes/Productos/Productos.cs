using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;


namespace ColpatriaSAI.Negocio.Componentes.Productos
{
    public class Productos
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region Producto

        public List<Producto> ListarProductoes()
        {
            return tabla.Productoes.Include("Ramo.Compania").Include("Plazo").Where(Producto => Producto.id > 0).OrderBy(p => p.nombre).ToList();
        }

        public List<Producto> ListarProductoesPorId(int idProducto)
        {
            return tabla.Productoes.Include("Ramo.Compania").Where(Producto => Producto.id == idProducto && Producto.id > 0).ToList();
        }

        public List<Producto> ListarProductosporRamo(int idRamo)
        {
            return tabla.Productoes.Where(Producto => Producto.ramo_id == idRamo && Producto.id > 0).OrderBy(e => e.nombre).ToList();
        }

        public int InsertarProducto(Producto producto, string Username)
        {
            int resultado = 0;
            if (producto.id == 0)
            {
                if (tabla.Productoes.Where(Producto => Producto.nombre == producto.nombre).ToList().Count() == 0)
                {
                    tabla.Productoes.AddObject(producto);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Producto,
    SegmentosInsercion.Personas_Y_Pymes, null, producto);
                    tabla.SaveChanges();
                    resultado = producto.id;
                }
            }
            else
            {
                if (tabla.Productoes.Where(Producto => Producto.nombre == producto.nombre && Producto.id != producto.id).ToList().Count() == 0)
                {
                    Producto productoActual = tabla.Productoes.Single(p => p.id == producto.id);
                    var pValorAntiguo = productoActual;
                    productoActual.nombre = producto.nombre;
                    productoActual.ramo_id = producto.ramo_id;
                    productoActual.plazo_id = producto.plazo_id;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Producto,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, productoActual);
                    tabla.SaveChanges();
                    resultado = producto.id;
                }
            }
            return resultado;
        }

        public string EliminarProducto(int id, string Username)
        {
            var detalleActual = tabla.ProductoDetalles.Where(p => p.producto_id == id);
            foreach (var item in detalleActual) item.producto_id = 0;

            var productoActual = tabla.Productoes.Single(p => p.id == id);
            tabla.Productoes.DeleteObject(productoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Producto,
    SegmentosInsercion.Personas_Y_Pymes, productoActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        #endregion

        #region Producto Detalle

        public List<ProductoDetalle> ListarProductoDetalles(int id)
        {
            if (id != 0)
                return tabla.ProductoDetalles.Include("Producto.Ramo.Compania").Include("RamoDetalle.Compania").Where(e => e.id > 0 && (e.producto_id == id || e.producto_id == 0)).OrderBy(e => e.nombre).ToList();
            else
                return tabla.ProductoDetalles.Include("Producto.Ramo.Compania").Include("RamoDetalle.Compania").Where(e => e.id > 0).OrderBy(e => e.nombre).ToList();
        }

        public int AgruparProductoDetalle(int producto_id, string productosTrue, string productosFalse)
        {
            List<int> idsTrue = new List<int>();
            List<int> idsFalse = new List<int>();
            foreach (string id in productosTrue.Split(','))
            {
                if (id != "") idsTrue.Add(Convert.ToInt32(id));
            }
            foreach (string id in productosFalse.Split(','))
            {
                if (id != "") idsFalse.Add(Convert.ToInt32(id));
            }

            var productoDetalle = tabla.ProductoDetalles;

            foreach (int id in idsTrue) productoDetalle.Single(rd => rd.id == id).producto_id = producto_id;
            foreach (int id in idsFalse) productoDetalle.Single(rd => rd.id == id).producto_id = 0;
            return tabla.SaveChanges();
        }

        public List<ProductoDetalle> ListarProductoDetalleXRamoDetalle(int ramoDetalleId)
        {
            return tabla.ProductoDetalles
                .Include("Producto.Ramo.Compania")
                .Include("RamoDetalle.Compania")
                .Where(e => e.ramoDetalle_id == ramoDetalleId).OrderBy(e => e.nombre).ToList();
        }
        #endregion
    }
}