using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    class CategoriasxRegla
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<CategoriaxRegla> ListarCategoriasxRegla(int regla_id)
        {
            return tabla.CategoriaxReglas.Include("Categoria").Where(c => c.regla_id == regla_id).ToList();
        }

        public int ActualizarCategoriaxRegla(List<CategoriaxRegla> categoriasxRegla, string Username)
        {
            foreach (CategoriaxRegla cr in categoriasxRegla)
            {
                if (cr.id == 0)
                {
                    tabla.CategoriaxReglas.AddObject(cr);
                }
                else if (cr.id != 0 && cr.esRecaudo == false && cr.esColquin == false)
                {
                    var categoriaxReglaActual = this.tabla.CategoriaxReglas.Where(c => c.id == cr.id).First();
                    tabla.CategoriaxReglas.DeleteObject(categoriaxReglaActual);

                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.CategoriaxRegla,
    SegmentosInsercion.Personas_Y_Pymes, categoriaxReglaActual, null);
                }
                else
                {
                    var categoriaxReglaActual = this.tabla.CategoriaxReglas.Where(c => c.id == cr.id).First();
                    var pValorAntiguo = categoriaxReglaActual;
                    categoriaxReglaActual.esColquin = cr.esColquin;
                    categoriaxReglaActual.esRecaudo = cr.esRecaudo;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CategoriaxRegla,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, categoriaxReglaActual);
                }
            }

            return tabla.SaveChanges();
        }
    }
}