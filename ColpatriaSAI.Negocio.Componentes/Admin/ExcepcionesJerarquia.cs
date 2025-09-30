using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class ExcepcionesJerarquia
    {

        private SAI_Entities contexto = new SAI_Entities();

        public List<Entidades.ExcepcionJerarquiaDetalle> ListarExcepcionesJerarquiaporId(int idJerarquia)
        {
            return contexto.ExcepcionJerarquiaDetalles.Include("JerarquiaDetalle").Include("Meta").Where(e => e.excepcionJerarquiaOrigen_id == idJerarquia).ToList();
        }

        public List<Entidades.ExcepcionJerarquiaDetalle> ListarExcepcionesJerarquiaporIdDestino(int idJerarquia)
        {
            return contexto.ExcepcionJerarquiaDetalles.Include("JerarquiaDetalle").Include("Meta").Where(e => e.excepcionJerarquiaDestino_id == idJerarquia).ToList();
        }

        public int InsertarExceptionJerarquia(Entidades.ExcepcionJerarquiaDetalle excepcionJerarquia, string Username)
        {
            contexto.ExcepcionJerarquiaDetalles.AddObject(excepcionJerarquia);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ExcepcionJerarquiaDetalle,
    SegmentosInsercion.Personas_Y_Pymes, null, excepcionJerarquia);
            return contexto.SaveChanges();

        }
        public int EliminarExceptionJerarquia(int idExcepcionJerarqua, string Username)
        {
            Entidades.ExcepcionJerarquiaDetalle excepcion = contexto.ExcepcionJerarquiaDetalles.Where(e => e.id == idExcepcionJerarqua).FirstOrDefault();
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ExcepcionJerarquiaDetalle,
    SegmentosInsercion.Personas_Y_Pymes, excepcion, null);
            contexto.ExcepcionJerarquiaDetalles.DeleteObject(excepcion);
            return contexto.SaveChanges();

        }

        public Entidades.ExcepcionJerarquiaDetalle TraerExceptionJerarquiaporId(int idExcepcionJerarquia)
        {
            Entidades.ExcepcionJerarquiaDetalle excepcion = contexto.ExcepcionJerarquiaDetalles.Where(e => e.id == idExcepcionJerarquia).FirstOrDefault();
            return excepcion;
        }

        public int ActualizarExcepcionJerarquia(Entidades.ExcepcionJerarquiaDetalle exceptionJerarquia, string Username)
        {
            var excepcion = contexto.ExcepcionJerarquiaDetalles.Where(e => e.id == exceptionJerarquia.id).FirstOrDefault();
            var pValorAntiguo = excepcion;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionJerarquiaDetalle,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, excepcion);

            return contexto.SaveChanges();
        }

    }

}


