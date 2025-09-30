using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class AntiguedadxNiveles
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de las Antiguedades
        /// </summary>
        /// <returns>Lista de Monedas</returns>
        public List<AntiguedadxNivel> ListarAntiguedades()
        {
            return tabla.AntiguedadxNivels.Include("Nivel").Where(AntiguedadxNivels => AntiguedadxNivels.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de las Antiguedades
        /// </summary>
        /// <param name="idRed">Id de la antiguedad a consultar</param>
        /// <returns>Lista de objectos AntiguedadxNivel</returns>
        public List<AntiguedadxNivel> ListarAntiguedadesPorId(int id)
        {
            return tabla.AntiguedadxNivels.Include("Nivel").Where(AntiguedadxNivel => AntiguedadxNivel.id == id && AntiguedadxNivel.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de antiguedad
        /// </summary>
        /// <param name="zona">Objecto AntiguedadxNivel a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarAntiguedadxNivel(AntiguedadxNivel antiguedad, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.AntiguedadxNivels.Where(a => a.numeroMeses == antiguedad.numeroMeses && a.nivel_id == antiguedad.nivel_id).ToList().Count() == 0)
            {

                tabla.AntiguedadxNivels.AddObject(antiguedad);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.AntiguedadxNivel,
    SegmentosInsercion.Personas_Y_Pymes, null, antiguedad);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de antiguedad
        /// </summary>
        /// <param name="id">Id de la antiguedad a modificar</param>
        /// <param name="zona">Objeto AntiguedadxNivel utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarAntiguedadxNivel(int id, AntiguedadxNivel antiguedad, string Username)
        {
            int resultado = 0;
            if (tabla.AntiguedadxNivels.Where(a => a.numeroMeses == antiguedad.numeroMeses && a.nivel_id == antiguedad.nivel_id).ToList().Count() == 0)
            {
                var antiguedadActual = this.tabla.AntiguedadxNivels.Where(a => a.id == id && a.id > 0).First();
                var antiguedadAntigua = antiguedadActual;
                antiguedadActual.numeroMeses = antiguedad.numeroMeses;
                antiguedadActual.nivel_id = antiguedad.nivel_id;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.AntiguedadxNivel, SegmentosInsercion.Personas_Y_Pymes, antiguedadAntigua, antiguedadActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de antiguedad
        /// </summary>
        /// <param name="id">Id de la antiguedad a eliminar</param>
        /// <param name="zona">Objecto AntiguedadxNivel utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarAntiguedadxNivel(int id, AntiguedadxNivel antiguedad, string Username)
        {
            var antiguedadActual = this.tabla.AntiguedadxNivels.Where(AntiguedadxNivel => AntiguedadxNivel.id == id && AntiguedadxNivel.id > 0).First();
            tabla.DeleteObject(antiguedadActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.AntiguedadxNivel,
    SegmentosInsercion.Personas_Y_Pymes, antiguedadActual, null);
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

