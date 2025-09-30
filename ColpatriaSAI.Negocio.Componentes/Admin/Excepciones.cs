using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class Excepciones
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<Entidades.Excepcion> ListarExcepcionesporId(int idException)
        {
            return contexto.Excepcions.Include("Localidad").Include("Compania").Include("LineaNegocio").Include("Ramo").Include("Producto").Include("Participante").Include("Zona").Include("TipoVehiculo").Where(e => e.id == idException).OrderByDescending(e => e.id).ToList();
        }

        public List<Entidades.Excepcion> ListarExcepciones(int idfranquicia)
        {
            return contexto.Excepcions.Include("Compania").Include("Ramo").Include("Producto").Where(e => e.Localidad_id == idfranquicia).ToList();
        }

        public List<Entidades.Excepcion> ListarExcepcionesEspeciales()
        {
            return contexto.Excepcions.Include("Localidad").Include("Localidad1").Include("Compania").Include("Ramo").Include("Producto").Where(e => e.excepcion_recaudo == true).ToList();
        }

        public int InsertarException(Entidades.Excepcion excepcion, string Username)
        {
            contexto.Excepcions.AddObject(excepcion);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Excepcion,
    SegmentosInsercion.Personas_Y_Pymes, null, excepcion);
            return contexto.SaveChanges();

        }
        public int EliminarException(int idexcepcion, string Username)
        {
            Entidades.Excepcion excepcion = contexto.Excepcions.Where(e => e.id == idexcepcion).FirstOrDefault();
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Excepcion,
    SegmentosInsercion.Personas_Y_Pymes, excepcion, null);
            contexto.Excepcions.DeleteObject(excepcion);
            return contexto.SaveChanges();

        }

        public List<Entidades.Excepcion> TraerExceptionporId(int idexcepcion)
        {
            List<Entidades.Excepcion> excepciones = contexto.Excepcions.Where(e => e.id == idexcepcion).ToList();
            return excepciones;
        }

        public int ActualizarExcepcion(Entidades.Excepcion exception, string Username)
        {
            var excepcion = contexto.Excepcions.Where(e => e.id == exception.id).FirstOrDefault();
            var pValorAntiguo = excepcion;

            excepcion.fecha_ini = exception.fecha_ini;
            excepcion.fecha_fin = exception.fecha_fin;
            excepcion.compania_id = exception.compania_id;
            excepcion.ramo_id = exception.ramo_id;
            excepcion.producto_id = exception.producto_id;
            excepcion.lineaNegocio_id = exception.lineaNegocio_id;
            excepcion.negocio_id = exception.negocio_id;
            excepcion.poliza = exception.poliza;
            excepcion.Porcentaje = exception.Porcentaje;
            excepcion.Estado = exception.Estado;
            excepcion.Localidad_id = exception.Localidad_id;
            excepcion.localidad_de_id = exception.localidad_de_id;
            excepcion.codigoAgrupador = exception.codigoAgrupador;
            excepcion.participante_id = exception.participante_id;
            excepcion.tipoVehiculo_id = exception.tipoVehiculo_id;
            excepcion.clave = exception.clave;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Excepcion,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, excepcion);
            return contexto.SaveChanges();

        }

    }
}
