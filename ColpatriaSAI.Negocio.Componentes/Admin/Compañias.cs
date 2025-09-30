using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Companias
    {
        private SAI_Entities tabla = new SAI_Entities();


        public int InsertarCompania(Compania compañia, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Companias.Where(Compania => Compania.nombre == compañia.nombre).ToList().Count() == 0)
            {
                tabla.Companias.AddObject(compañia);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Compania,
                        SegmentosInsercion.Personas_Y_Pymes, null, compañia);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public List<Compania> ListarCompanias()
        {
            return tabla.Companias.Where(Compañia => Compañia.id > 0).OrderBy(e => e.nombre).ToList();
        }

        public List<Compania> ListarCompaniasPorId(int idCompañia)
        {
            return tabla.Companias.Where(Compañia => Compañia.id == idCompañia && Compañia.id > 0).ToList();
        }

        public int ActualizarCompania(int id, Compania compania, string Username)
        {
            var companiaActual = this.tabla.Companias.Where(Compania => Compania.id == id && Compania.id > 0).First();
            var pValorAntiguo = companiaActual;
            companiaActual.nombre = compania.nombre;
            companiaActual.codigoCore = compania.codigoCore;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Compania,
                       SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, companiaActual);
            return tabla.SaveChanges();
        }


        public string EliminarCompania(int id, Compania compania, string Username)
        {
            var companiaActual = this.tabla.Companias.Where(Compania => Compania.id == id && Compania.id > 0).First();
            tabla.DeleteObject(companiaActual);


            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Compania,
                      SegmentosInsercion.Personas_Y_Pymes, companiaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {

                return ex.Message;

            }

            return "";
        }
    }
}

