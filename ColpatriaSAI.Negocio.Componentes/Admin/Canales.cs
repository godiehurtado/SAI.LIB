using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Canales
    {
        private SAI_Entities tabla = new SAI_Entities();


        public List<Canal> ListarCanals()
        {
            return tabla.Canals.Where(c => c.id > 0).ToList();
        }


        public List<Canal> ListarCanalsPorId(int id)
        {
            return tabla.Canals.Where(Canal => Canal.id == id && Canal.id > 0).ToList();
        }


        public int InsertarCanal(Canal canal, string Username)
        {
            int resultado = 0;
            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Canals.Where(Canal => Canal.nombre == canal.nombre).ToList().Count() == 0)
            {
                tabla.Canals.AddObject(canal);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Canal,
                      SegmentosInsercion.Personas_Y_Pymes, null, canal);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }


        public int ActualizarCanal(int id, Canal canal, string Username)
        {
            int resultado = 0;
            if (tabla.Canals.Where(c => c.nombre == canal.nombre && c.id != id).ToList().Count() == 0)
            {
                var canalActual = this.tabla.Canals.Where(Canal => Canal.id == id && Canal.id > 0).First();
                var CanalAntiguo = canalActual;
                canalActual.nombre = canal.nombre;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username,
                    tablasAuditadas.Canal, SegmentosInsercion.Personas_Y_Pymes, CanalAntiguo, canalActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public string EliminarCanal(int id, Canal canal, string Username)
        {
            var canalActual = this.tabla.Canals.Where(Canal => Canal.id == id && Canal.id > 0).First();
            tabla.DeleteObject(canalActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Canal,
                      SegmentosInsercion.Personas_Y_Pymes, canalActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {

                return ex.Message;

            }

            return "";
        }

        public List<CanalDetalle> ListarCanalDetalles()
        {
            return tabla.CanalDetalles.Where(x => x.id > 0).ToList();
        }

    }
}

