using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class GrupoTipoEndosos
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region Metodos EXCEPCIONESXGRUPOTIPOENDOSO

        public List<ExcepcionesxGrupoTipoEndoso> ListarExcepcionesxGrupoTipoEndoso()
        {
            return tabla.ExcepcionesxGrupoTipoEndosoes.Include("Compania").Include("GrupoEndoso").Include("TipoEndoso").Where(eg => eg.id > 0).ToList();
        }

        public List<ExcepcionesxGrupoTipoEndoso> ListarExcepcionesxGrupoTipoEndosoPorId(int id)
        {
            return tabla.ExcepcionesxGrupoTipoEndosoes.Include("Compania").Include("GrupoEndoso").Include("TipoEndoso").Where(eg => eg.id == id && eg.id > 0).ToList();
        }

        public int InsertarExcepcionesxGrupoTipoEndoso(ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.ExcepcionesxGrupoTipoEndosoes.Where(eg => eg.grupoEndoso_id == excepcionesporGrupoTipoEndoso.grupoEndoso_id
                && eg.tipoEndoso_id == excepcionesporGrupoTipoEndoso.tipoEndoso_id).ToList().Count() == 0)
            {
                tabla.ExcepcionesxGrupoTipoEndosoes.AddObject(excepcionesporGrupoTipoEndoso);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ExcepcionesxGrupoTipoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, excepcionesporGrupoTipoEndoso, null);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public int ActualizarExcepcionesxGrupoTipoEndoso(int id, ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username)
        {
            var exgrupoTipoEndosoActual = this.tabla.ExcepcionesxGrupoTipoEndosoes.Where(eg => eg.id == id).First();
            var pValorAntiguo = exgrupoTipoEndosoActual;
            exgrupoTipoEndosoActual.grupoEndoso_id = excepcionesporGrupoTipoEndoso.grupoEndoso_id;
            exgrupoTipoEndosoActual.tipoEndoso_id = excepcionesporGrupoTipoEndoso.tipoEndoso_id;
            exgrupoTipoEndosoActual.estado = excepcionesporGrupoTipoEndoso.estado;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionesxGrupoTipoEndoso,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, exgrupoTipoEndosoActual);
            return tabla.SaveChanges();
        }

        public string EliminarExcepcionesxGrupoTipoEndoso(int id, ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username)
        {
            var exgrupoTipoEndosoActual = this.tabla.ExcepcionesxGrupoTipoEndosoes.Where(eg => eg.id == id).First();
            tabla.DeleteObject(exgrupoTipoEndosoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, exgrupoTipoEndosoActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }
        #endregion
    }
}

