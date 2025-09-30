using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Productos
{
    public class Ramos
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region RAMOAGRUPADO

        public List<Ramo> ListarRamos()
        {
            return tabla.Ramoes.Include("Compania").Where(Ramo => Ramo.id > 0).OrderBy(e => e.nombre).ToList();
        }

        public List<Ramo> ListarRamosPorId(int idRamo)
        {
            return tabla.Ramoes.Include("Compania").Where(Ramo => Ramo.id == idRamo && Ramo.id > 0).ToList();
        }

        public List<Ramo> ListarRamosPorCompania(int idCompania)
        {
            return tabla.Ramoes.Where(Ramo => Ramo.compania_id == idCompania && Ramo.id > 0).OrderBy(e => e.nombre).ToList();
        }

        public int InsertarRamo(Ramo ramo, string Username)
        {
            int resultado = 0;

            if (tabla.Ramoes.Where(Ramo => Ramo.nombre == ramo.nombre).ToList().Count() == 0)
            {
                tabla.Ramoes.AddObject(ramo);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Ramo,
      SegmentosInsercion.Personas_Y_Pymes, null, ramo);
                tabla.SaveChanges();
                resultado = ramo.id;
            }

            return resultado;
        }

        public int ActualizarRamo(int id, Ramo ramo, string Username)
        {
            int resultado = 0;

            if (tabla.Ramoes.Where(Ramo => Ramo.nombre == ramo.nombre && Ramo.id != id).ToList().Count() == 0)
            {
                var ramoActual = this.tabla.Ramoes.Where(Ramo => Ramo.id == id && Ramo.id > 0).First();
                var pValorAntiguo = ramoActual;
                ramoActual.nombre = ramo.nombre;
                ramoActual.compania_id = ramo.compania_id;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Ramo,
      SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, ramoActual);
                tabla.SaveChanges();
                resultado = ramo.id;
            }

            return resultado;
        }

        public string EliminarRamo(int id, Ramo ramo, string Username)
        {
            var detalleActual = tabla.RamoDetalles.Where(p => p.ramo_id == id);
            foreach (var item in detalleActual) item.ramo_id = 0;

            var ramoActual = tabla.Ramoes.Single(p => p.id == id);
            tabla.DeleteObject(ramoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Ramo,
      SegmentosInsercion.Personas_Y_Pymes, ramoActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        #endregion

        #region RAMODETALLE

        public List<RamoDetalle> ListarRamoDetalleXCompania(int companiaId)
        {
            return tabla.RamoDetalles
                .Include("Compania")
                .Include("Ramo").Where(r => r.compania_id == companiaId).OrderBy(e => e.nombre).ToList();
        }
        public List<RamoDetalle> ListarRamoDetalle(int id)
        {
            if (id != 0)
                return tabla.RamoDetalles.Include("Compania").Include("Ramo").Where(r => r.id > 0 && (r.ramo_id == id || r.ramo_id == 0)).OrderBy(e => e.nombre).ToList();
            else
                return tabla.RamoDetalles.Include("Compania").Include("Ramo").Where(r => r.id > 0).OrderBy(e => e.nombre).ToList();
        }

        public int AgruparRamoDetalle(int ramo_id, string ramosTrue, string ramosFalse)
        {
            List<int> idsTrue = new List<int>();
            List<int> idsFalse = new List<int>();
            foreach (string id in ramosTrue.Split(','))
            {
                if (id != "") idsTrue.Add(Convert.ToInt32(id));
            }
            foreach (string id in ramosFalse.Split(','))
            {
                if (id != "") idsFalse.Add(Convert.ToInt32(id));
            }

            var ramosDetalle = tabla.RamoDetalles;

            foreach (int id in idsTrue) ramosDetalle.Single(rd => rd.id == id).ramo_id = ramo_id;
            foreach (int id in idsFalse) ramosDetalle.Single(rd => rd.id == id).ramo_id = 0;
            return tabla.SaveChanges();
        }
        #endregion
    }
}
