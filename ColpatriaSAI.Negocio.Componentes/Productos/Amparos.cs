using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Productos
{
    public class Amparos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Amparo> ListarAmparo()
        {
            return tabla.Amparoes.Where(Amparo => Amparo.id > 0).OrderBy(p => p.nombre).ToList();
        }

        public List<Amparo> ListarAmparoesPorId(int idAmparo)
        {
            return tabla.Amparoes.Where(Amparo => Amparo.id == idAmparo).OrderByDescending(a => a.id).ToList();
        }

        public int Insertar(Amparo amparo, ref string mensajeDeError, string Username)
        {
            int id = 0;
            try
            {
                if (amparo.id == 0)
                {
                    if (tabla.Amparoes.Where(a => a.nombre == amparo.nombre).ToList().Count == 0)
                    {
                        tabla.Amparoes.AddObject(amparo);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Amparo,
      SegmentosInsercion.Personas_Y_Pymes, amparo, null);
                        tabla.SaveChanges();
                        id = amparo.id;
                    }

                }
                else
                {

                    var amparoActual = tabla.Amparoes.Single(a => a.id == amparo.id);
                    amparoActual.nombre = amparo.nombre;
                    amparoActual.codigoCore = amparo.codigoCore;
                    tabla.Amparoes.Attach(amparoActual);
                    tabla.SaveChanges();
                    id = amparo.id;


                }
            }
            catch (Exception ex)
            {
                mensajeDeError = string.Format("Se presento un error : {0}, de tipo : {1}", ex.Message, ex.StackTrace);
            }



            return id;
        }

        public void Eliminar(int id, ref string mensajeDeError, string Username)
        {

            if (id != 0)
            {

                var DetalleAmparo = tabla.AmparoDetalles.Where(da => da.amparo_id == id).ToList();

                if (DetalleAmparo.Count != 0)
                { mensajeDeError = "Este Amparo no se puede Eliminar, tiene informacion asociada"; }
                else
                {

                    try
                    {
                        var amp = tabla.Amparoes.Where(a => a.id == id).First();
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Amparo,
     SegmentosInsercion.Personas_Y_Pymes, null, amp);
                        tabla.DeleteObject(amp);
                        tabla.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        mensajeDeError = string.Format("Se presento un error : {0}, de tipo : {1}", ex.Message, ex.StackTrace);
                    }

                }

            }
            else { mensajeDeError = "Este amparo ya fue eliminado"; }
        }


        public List<AmparoDetalle> ListarAmparoDetalle()
        {
            return tabla.AmparoDetalles.ToList();
        }

        public List<AmparoDetalle> ListarAmparoDetallePorId(int idAmparo)
        {
            return tabla.AmparoDetalles.Where(ad => ad.amparo_id == idAmparo).ToList();
        }

        public int InsertarAmparoDetalle(AmparoDetalle amparoDetalle, string Username)
        {
            int id = 0;

            var AD = tabla.AmparoDetalles.Where(ad => ad.id == amparoDetalle.id).First();
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.AmparoDetalle,
     SegmentosInsercion.Personas_Y_Pymes, null, amparoDetalle);
            AD.amparo_id = amparoDetalle.amparo_id;
            id = tabla.SaveChanges();

            return id;
        }

        public int EliminarAmparoDetalle(int amparoid, ref string mensajeDeError, string Username)
        {

            int result = 0;
            var lstDetalleAmparo = tabla.AmparoDetalles.Where(ad => ad.amparo_id == amparoid).ToList();

            try
            {
                foreach (AmparoDetalle item in lstDetalleAmparo)
                {
                    var detamparo = tabla.AmparoDetalles.Where(ad => ad.id == item.id).First();
                    detamparo.amparo_id = 0;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.AmparoDetalle,
     SegmentosInsercion.Personas_Y_Pymes, detamparo, null);
                    result = tabla.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                mensajeDeError = string.Format("Se presento un error : {0}, de tipo : {1}", ex.Message, ex.StackTrace);
            }

            return result;
        }


    }
}

