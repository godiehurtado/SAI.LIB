using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class PersistenciaCAPI
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<PersistenciadeCAPIDetalle> ListarPersistenciaCAPIDetalle(string numeroNegocio, string clave)
        {
            var mesAbiertoCAPI = (from pc in tabla.PeriodoCierres
                                  where (pc.estado == 1 && pc.compania_id == 3)
                                  select pc.mesCierre).First();

            var anioAbiertoCAPI = (from pc in tabla.PeriodoCierres
                                   where (pc.estado == 1 && pc.compania_id == 3)
                                   select pc.anioCierre).First();

            if (mesAbiertoCAPI == 1)
            {
                return tabla.PersistenciadeCAPIDetalles.Include("Participante").Include("Plazo").Include("Localidad").Include("PlanDetalle").Include("Zona").Where(pc => pc.anioCierreNegocio == (anioAbiertoCAPI - 1)
                && (pc.numeroNegocio == numeroNegocio || pc.clave == clave)
                && (pc.mesCierre == 10) || (pc.mesCierre == 11) || (pc.mesCierre == 12)).ToList();
            }

            else
            {
                return tabla.PersistenciadeCAPIDetalles.Include("Participante").Include("Plazo").Include("Localidad").Include("PlanDetalle").Include("Zona").Where(pc => pc.anioCierreNegocio == anioAbiertoCAPI
                && (pc.numeroNegocio == numeroNegocio || pc.clave == clave)
                && (pc.mesCierre == (mesAbiertoCAPI - 3))).ToList();
            }
        }

        public List<PersistenciadeCAPIDetalle> ListarPersistenciaCAPIDetallePorId(int id)
        {
            return tabla.PersistenciadeCAPIDetalles.Include("Participante").Include("Plazo").Include("Localidad").Include("PlanDetalle").Include("Zona").Where(pc => pc.id == id).ToList();
        }

        public int ActualizarPersistenciaCAPIDetalle(int id, PersistenciadeCAPIDetalle persistenciacapidetalle, string Username)
        {
            var persistenciacapidetalleActual = this.tabla.PersistenciadeCAPIDetalles.Where(pc => pc.id == id).First();
            var pValorAntiguo = persistenciacapidetalleActual;
            persistenciacapidetalleActual.cumple = persistenciacapidetalle.cumple;
            persistenciacapidetalleActual.comentarios = persistenciacapidetalle.comentarios;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PersistenciadeCAPIDetalle,
                      SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, persistenciacapidetalleActual);
            return tabla.SaveChanges();
        }
    }
}
