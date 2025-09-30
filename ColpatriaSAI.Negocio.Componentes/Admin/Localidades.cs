using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Web.Mvc;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Localidades
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Localidad> ListarLocalidades()
        {
            return tabla.Localidads.Include("Zona").Include("TipoLocalidad").Where(Localidad => Localidad.id > 0).ToList();
        }

        public List<Localidad> ListarLocalidadesPorId(int idLocalidad)
        {
            return tabla.Localidads.Include("Zona").Include("TipoLocalidad").Where(Localidad => Localidad.id == idLocalidad && Localidad.id > 0).ToList();
        }

        public List<Localidad> ListarLocalidadesPorZona(int idZona)
        {
            return tabla.Localidads.Where(Localidad => Localidad.zona_id == idZona && Localidad.id > 0).ToList();
        }

        public bool validarCodigosInsert(Localidad localidad)
        {
            bool guardar = false;

            if (tabla.Localidads.Where(l => l.codigo_SISE == localidad.codigo_SISE && l.codigo_SISE != null && l.codigo_SISE != "").Count() == 0)
            {
                guardar = true;
                if (localidad.codigo_SISE == "") localidad.codigo_SISE = null;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_CAPI == localidad.codigo_CAPI && l.codigo_CAPI != null).Count() == 0)
            {
                guardar = true;
                if (localidad.codigo_CAPI == -999) localidad.codigo_CAPI = null;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_BH == localidad.codigo_BH && l.codigo_BH != null && l.codigo_BH != "").Count() == 0)
            {
                guardar = true;
                if (localidad.codigo_BH == "") localidad.codigo_BH = null;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_EMERMEDICA == localidad.codigo_EMERMEDICA && l.codigo_EMERMEDICA != null && l.codigo_EMERMEDICA != "").Count() == 0)
            {
                guardar = true;
                if (localidad.codigo_EMERMEDICA == "") localidad.codigo_EMERMEDICA = null;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_ARP == localidad.codigo_ARP && l.codigo_ARP != null && l.codigo_ARP != "").Count() == 0)
            {
                guardar = true;
                if (localidad.codigo_ARP == "") localidad.codigo_ARP = null;
            }
            else return false;

            if (guardar == true) return true; else return false;
        }

        public int InsertarLocalidad(Localidad localidad, string Username)
        {
            int resultado = 0;
            if (tabla.Localidads.Where(l => l.nombre == localidad.nombre).Count() == 0)
            {
                if (validarCodigosInsert(localidad) == true)
                {
                    tabla.Localidads.AddObject(localidad);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Localidad,
                        SegmentosInsercion.Personas_Y_Pymes, null, localidad);
                    resultado = tabla.SaveChanges();
                }
            }
            return resultado;
        }

        public bool validarCodigosUpdate(Localidad localidad, int id)
        {
            bool guardar = false;

            if (localidad.codigo_SISE == "") localidad.codigo_SISE = null;
            if (localidad.codigo_CAPI == -999) localidad.codigo_CAPI = null;
            if (localidad.codigo_BH == "") localidad.codigo_BH = null;
            if (localidad.codigo_EMERMEDICA == "") localidad.codigo_EMERMEDICA = null;
            if (localidad.codigo_ARP == "") localidad.codigo_ARP = null;

            if (tabla.Localidads.Where(l => l.codigo_SISE == localidad.codigo_SISE && l.id != id).Count() == 0)
            {
                guardar = true;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_CAPI == localidad.codigo_CAPI && l.id != id).Count() == 0)
            {
                guardar = true;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_BH == localidad.codigo_BH && l.id != id).Count() == 0)
            {
                guardar = true;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_EMERMEDICA == localidad.codigo_EMERMEDICA && l.id != id).Count() == 0)
            {
                guardar = true;
            }
            else return false;

            if (tabla.Localidads.Where(l => l.codigo_ARP == localidad.codigo_ARP && l.id != id).Count() == 0)
            {
                guardar = true;
            }
            else return false;

            if (guardar == true) return true; else return false;
        }

        public int ActualizarLocalidad(int id, Localidad localidad, string Username)
        {
            if (tabla.Localidads.Where(l => l.nombre == localidad.nombre && l.id != id).Count() == 0)
            {
                if (validarCodigosUpdate(localidad, id) == true)
                {
                    var localidadActual = tabla.Localidads.Single(l => l.id == id && l.id > 0);
                    var pValorAntiguo = localidadActual;
                    localidadActual.nombre = localidad.nombre;
                    localidadActual.zona_id = localidad.zona_id;
                    localidadActual.tipo_localidad_id = localidad.tipo_localidad_id;
                    localidadActual.codigo_SISE = localidad.codigo_SISE;
                    localidadActual.codigo_CAPI = localidad.codigo_CAPI;
                    localidadActual.codigo_BH = localidad.codigo_BH;
                    localidadActual.codigo_EMERMEDICA = localidad.codigo_EMERMEDICA;
                    localidadActual.codigo_ARP = localidad.codigo_ARP;
                    localidadActual.clavePago = localidad.clavePago;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Localidad,
                        SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, localidadActual);
                    tabla.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }

        public string EliminarLocalidad(int id, Localidad localidad, string Username)
        {
            var localidadActual = this.tabla.Localidads.Where(Localidad => Localidad.id == id && Localidad.id > 0).First();
            tabla.DeleteObject(localidadActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Localidad,
                        SegmentosInsercion.Personas_Y_Pymes, localidadActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }

    public class TipoLocalidades
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<TipoLocalidad> ListarTipoLocalidad()
        {
            return tabla.TipoLocalidads.Where(TipoLocalidad => TipoLocalidad.id > 0 && TipoLocalidad.id > 0).ToList();
        }
    }
}
