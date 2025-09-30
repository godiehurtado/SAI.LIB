using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.Objects;

namespace ColpatriaSAI.Negocio.Componentes.Contratacion
{
    public class Combos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Entidades.Combo> ListarCombos()
        {
            //Relacion de campos de tablas respecto a las variables de navegación en el Modelo de datos x tabla

            return tabla.Comboes.Include("ProductoComboes").Where(c => c.id > 0).ToList();

        }

        public List<Entidades.Combo> ListarCombosPorId(int idCombo)
        {
            return tabla.Comboes.Where(c => c.id == idCombo && c.id > 0).ToList();
        }

        public int InsertarCombo(Combo combo, string Username)
        {
            int resultado = 0;
            if (tabla.Comboes.Where(c => c.nombre == combo.nombre).ToList().Count() == 0)
            {
                tabla.Comboes.AddObject(combo);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Combo,
    SegmentosInsercion.Personas_Y_Pymes, null, combo);
                tabla.SaveChanges();
                resultado = (from m in tabla.Comboes
                             select m.id).Max();
            }
            return resultado;
        }

        public int ActualizarCombo(int id, Combo combo, string Username)
        {
            int resultado = 0;
            var comboActual = this.tabla.Comboes.Where(c => c.id == id).First();
            var pValorAntiguo = comboActual;
            comboActual.nombre = combo.nombre;
            comboActual.descripcion = combo.descripcion;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Combo,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, comboActual);
            tabla.SaveChanges();
            resultado = comboActual.id;
            return resultado;
        }

        public int ActualizarComboValidado(int id, int validado, string Username)
        {
            int resultado = 0;
            var comboActual = this.tabla.Comboes.Where(c => c.id == id).First();
            var pValorAntiguo = comboActual;
            comboActual.validado = validado;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Combo,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, comboActual);
            tabla.SaveChanges();
            resultado = comboActual.id;
            return resultado;
        }

        public int EliminarCombo(int idCombo, string Username)
        {
            int resultado = 0;

            if (!this.ValidarIntegridadCombo(idCombo))
            {

                //ELIMINAMOS REGISTRO HIJOS
                List<ProductoCombo> productoComboList = tabla.ProductoComboes.Where(x => x.combo_id == idCombo).ToList();
                foreach (ProductoCombo productoCombo in productoComboList)
                {
                    tabla.ProductoComboes.DeleteObject(productoCombo);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ProductoCombo,
    SegmentosInsercion.Personas_Y_Pymes, productoCombo, null);
                }


                //ELIMINAMOS EL REGISTRO PADRE
                Combo combo = tabla.Comboes.Where(m => m.id == idCombo).FirstOrDefault();
                tabla.Comboes.DeleteObject(combo);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Combo,
    SegmentosInsercion.Personas_Y_Pymes, null, combo);
                tabla.SaveChanges();
                resultado = 1;
            }

            return resultado;
        }

        public Boolean ValidarIntegridadCombo(int idCombo)
        {
            Boolean integridadReferencial = false;

            //Moneda x Negocio
            var productoCombo = tabla.MonedaxNegocios.Where(x => x.combo_id == idCombo).ToList();

            if (productoCombo.Count() > 0)
                integridadReferencial = true;

            return integridadReferencial;
        }

        public List<ProductoCombo> ListarProductosCombosPorId(int idCombo)
        {
            return tabla.ProductoComboes.Include("Ramo").Include("Compania").Include("Producto").Where(x => x.combo_id == idCombo).ToList();
        }

        public int InsertarProductoCombo(ProductoCombo productocombo, string Username)
        {
            int resultado = 0;

            var totalRegistro = tabla.ProductoComboes.Where(x => x.combo_id == productocombo.combo_id && x.compania_id == productocombo.compania_id && x.ramo_id == productocombo.ramo_id && x.producto_id == productocombo.producto_id).ToList().Count();

            if (totalRegistro == 0)
            {
                tabla.ProductoComboes.AddObject(productocombo);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ProductoCombo,
    SegmentosInsercion.Personas_Y_Pymes, null, productocombo);
                tabla.SaveChanges();
                resultado = (from pm in tabla.ProductoComboes
                             select pm.id).Max();
            }
            return resultado;
        }

        public int ActualizarProductoCombo(int id, ProductoCombo productocombo, string Username)
        {
            int resultado = 0;
            var productoComboActual = this.tabla.ProductoComboes.Where(m => m.id == id).First();
            var pValorAntiguo = productoComboActual;
            productoComboActual.compania_id = productocombo.compania_id;
            productoComboActual.ramo_id = productocombo.ramo_id;
            productoComboActual.producto_id = productocombo.producto_id;
            productoComboActual.es_principal = productocombo.es_principal;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ProductoCombo,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, productoComboActual);
            tabla.SaveChanges();
            resultado = productoComboActual.id;
            return resultado;
        }

        public int EliminarProductoCombo(int idProductoCombo, string Username)
        {
            int resultado = 0;

            //ELIMINAMOS EL REGISTRO PADRE
            ProductoCombo productoCombo = tabla.ProductoComboes.Where(m => m.id == idProductoCombo).FirstOrDefault();
            tabla.ProductoComboes.DeleteObject(productoCombo);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ProductoCombo,
    SegmentosInsercion.Personas_Y_Pymes, productoCombo, null);
            tabla.SaveChanges();

            return resultado;
        }
    }
}
