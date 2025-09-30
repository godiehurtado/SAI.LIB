using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;


namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    public class ProductoConcursos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<ProductoConcurso> ListarProductoConcursoes()
        {
            return tabla.ProductoConcursoes.Include("Concurso").Include("Compania").Include("LineaNegocio").Include("Ramo").Include("Producto").Where(ProductoConcurso => ProductoConcurso.id > 0).ToList();
        }

        public List<ProductoConcurso> ListarProductoConcursoesPorId(int id)
        {
            return tabla.ProductoConcursoes.Include("Concurso").Include("Compania").Include("LineaNegocio").Include("Ramo").Include("Producto").Where(ProductoConcurso => ProductoConcurso.id == id && ProductoConcurso.id > 0).ToList();
        }

        public int InsertarProductoConcurso(ProductoConcurso productoconcurso, string Username)
        {
            int resultado = 0;
            if (tabla.ProductoConcursoes.Where(ProductoConcurso => ProductoConcurso.producto_id == productoconcurso.producto_id && ProductoConcurso.compania_id == productoconcurso.compania_id &&
                    ProductoConcurso.fecha_inicio == productoconcurso.fecha_inicio && ProductoConcurso.fecha_fin == productoconcurso.fecha_fin && ProductoConcurso.lineaNegocio_id == productoconcurso.lineaNegocio_id && ProductoConcurso.ramo_id == productoconcurso.ramo_id
                    && ProductoConcurso.concurso_id == productoconcurso.concurso_id).ToList().Count() == 0)
            {
                tabla.ProductoConcursoes.AddObject(productoconcurso);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ProductoConcurso,
    SegmentosInsercion.Personas_Y_Pymes, null, productoconcurso);
                tabla.SaveChanges();
                resultado = productoconcurso.id;
            }
            return resultado;
        }

        public int ActualizarProductoConcurso(int id, ProductoConcurso productoconcurso, string Username)
        {
            var productoconcursoActual = this.tabla.ProductoConcursoes.Where(ProductoConcurso => ProductoConcurso.id == id && ProductoConcurso.id > 0).First();
            var pValorAntiguo = productoconcurso;
            productoconcursoActual.compania_id = productoconcurso.compania_id;
            productoconcursoActual.lineaNegocio_id = productoconcurso.lineaNegocio_id;
            productoconcursoActual.ramo_id = productoconcurso.ramo_id;
            productoconcursoActual.producto_id = productoconcurso.producto_id;
            productoconcursoActual.fecha_inicio = productoconcurso.fecha_inicio;
            productoconcursoActual.fecha_fin = productoconcurso.fecha_fin;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ProductoConcurso,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, productoconcursoActual);
            return tabla.SaveChanges();
        }

        public string EliminarProductoConcurso(int id, ProductoConcurso productoconcurso, string Username)
        {
            var productoconcursoActual = this.tabla.ProductoConcursoes.Where(ProductoConcurso => ProductoConcurso.id == id && ProductoConcurso.id > 0).First();
            tabla.DeleteObject(productoconcursoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ProductoConcurso,
    SegmentosInsercion.Personas_Y_Pymes, productoconcursoActual, null);
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




