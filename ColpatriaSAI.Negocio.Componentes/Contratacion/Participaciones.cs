using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Contratacion
{
    public class Participaciones
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region Participaciones de Gerentes

        public List<Participacione> ListarParticipaciones()
        {
            return tabla.Participaciones.Include("Compania").Include("LineaNegocio").Include("Ramo").ToList();
        }

        public List<Ramo> ListarRamosXCompania(int id)
        {
            return tabla.Ramoes.Where(r => r.compania_id == id).OrderBy(c => c.nombre).ToList();
        }

        public List<Participacione> ListarParticipacionPorId(int id)
        {
            return tabla.Participaciones.Where(p => p.id == id && p.id > 0).ToList();
        }

        public int InsertarParticipacion(Participacione part, string Username)
        {
            int resultado = 0;
            if (tabla.Participaciones.Where(p => p.fechaIni == part.fechaIni &&
                                                p.fechaFin == part.fechaFin &&
                                                p.lineaNegocio_id == part.lineaNegocio_id &&
                                                p.ramo_id == part.ramo_id &&
                                                p.mesesAntiguedad == part.mesesAntiguedad &&
                                                p.compania_id == part.compania_id &&
                                                p.porcentaje == part.porcentaje).ToList().Count() == 0)
            {
                tabla.Participaciones.AddObject(part);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Participaciones,
    SegmentosInsercion.Personas_Y_Pymes, null, part);
                tabla.SaveChanges();
            }
            resultado = part.id;
            return resultado;
        }

        public int ActualizarParticipacion(int id, Participacione part, string Username)
        {
            int resultado = 0;
            if (tabla.Participaciones.Where(p => p.fechaIni == part.fechaIni &&
                                                p.fechaFin == part.fechaFin &&
                                                p.lineaNegocio_id == part.lineaNegocio_id &&
                                                p.ramo_id == part.ramo_id &&
                                                p.mesesAntiguedad == part.mesesAntiguedad &&
                                                p.compania_id == part.compania_id &&
                                                p.porcentaje == part.porcentaje).ToList().Count() == 0)
            {
                var partActual = this.tabla.Participaciones.Where(p => p.id == id).First();
                var pValorAntiguo = partActual;
                partActual.fechaIni = part.fechaIni;
                partActual.fechaFin = part.fechaFin;
                partActual.compania_id = part.compania_id;
                partActual.lineaNegocio_id = part.lineaNegocio_id;
                partActual.ramo_id = part.ramo_id;
                partActual.mesesAntiguedad = part.mesesAntiguedad;
                partActual.porcentaje = part.porcentaje;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Participaciones,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, partActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public string EliminarParticipacion(int id, Participacione part, string Username)
        {
            var partActual = this.tabla.Participaciones.Where(p => p.id == id).First();
            tabla.DeleteObject(partActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Participaciones,
    SegmentosInsercion.Personas_Y_Pymes, partActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Participaciones de Directores

        public List<ParticipacionDirector> ListarParticipacionesDirector()
        {
            return tabla.ParticipacionDirectors.Include("Compania").Include("JerarquiaDetalle").Include("Canal").ToList();
        }

        public List<ParticipacionDirector> ListarParticipacionDirectorPorId(int id)
        {
            return tabla.ParticipacionDirectors.Include("Compania").Include("JerarquiaDetalle").Where(p => p.id == id).ToList();
        }

        public List<Participante> ListarParticipantesPorNivel(string texto, int inicio, int cantidad, int nivel)
        {
            return tabla.Participantes.Where(p => (p.nombre.Contains(texto) || p.apellidos.Contains(texto) || p.clave.Contains(texto)) && p.nivel_id == nivel).OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
        }

        public int InsertarParticipacionDirector(ParticipacionDirector part, string Username)
        {
            int resultado = 0;
            if (tabla.ParticipacionDirectors.Where(p => p.fechaIni == part.fechaIni && p.fechaFin == part.fechaFin &&
                                                        p.compania_id == part.compania_id &&
                                                        p.canal_id == part.canal_id &&
                                                        p.jerarquiaDetalle_id == part.jerarquiaDetalle_id &&
                                                        p.porcentaje == part.porcentaje).ToList().Count() == 0)
            {
                tabla.ParticipacionDirectors.AddObject(part);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ParticipacionDirector,
    SegmentosInsercion.Personas_Y_Pymes, null, part);
                tabla.SaveChanges();
            }
            resultado = part.id;//tabla.ParticipacionDirectors.Where(p => p.compania_id == part.compania_id).ToList()[0].id;
            return resultado;
        }

        public int ActualizarParticipacionDirector(int id, ParticipacionDirector part, string Username)
        {
            int resultado = 0;
            if (tabla.ParticipacionDirectors.Where(p => p.fechaIni == part.fechaIni && p.fechaFin == part.fechaFin &&
                                                        p.compania_id == part.compania_id &&
                                                        p.canal_id == part.canal_id &&
                                                        p.jerarquiaDetalle_id == part.jerarquiaDetalle_id &&
                                                        p.porcentaje == part.porcentaje && p.id != id).ToList().Count() == 0)
            {
                var partActual = this.tabla.ParticipacionDirectors.Where(p => p.id == id).First();
                var pValorAntiguo = partActual;
                partActual.fechaIni = part.fechaIni;
                partActual.fechaFin = part.fechaFin;
                partActual.compania_id = part.compania_id;
                partActual.canal_id = part.canal_id;
                partActual.jerarquiaDetalle_id = part.jerarquiaDetalle_id;
                partActual.porcentaje = part.porcentaje;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ParticipacionDirector,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, partActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public string EliminarParticipacionDirector(int id, ParticipacionDirector part, string Username)
        {
            var partActual = this.tabla.ParticipacionDirectors.Where(p => p.id == id).First();
            tabla.DeleteObject(partActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ParticipacionDirector,
    SegmentosInsercion.Personas_Y_Pymes, partActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
