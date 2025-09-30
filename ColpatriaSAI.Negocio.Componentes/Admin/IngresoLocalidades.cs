using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class IngresoLocalidades
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de IngresoLocalidad
        /// </summary>
        /// <returns>Lista de IngresoLocalidad</returns> 
        public List<IngresoLocalidad> ListarIngresoLocalidades()
        {
            return tabla.IngresoLocalidads.Include("Localidad").Where(Ingreso => Ingreso.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de los IngresoLocalidad por id
        /// </summary>
        /// <param name="idIngreso">Id del IngresoLocalidad a consultar</param>
        /// <returns>Lista de objectos IngresoLocalidad</returns>
        public List<IngresoLocalidad> ListarIngresoLocalidadesPorId(int idIngreso)
        {
            return tabla.IngresoLocalidads.Include("Localidad").Where(Ingreso => Ingreso.id == idIngreso && Ingreso.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de IngresoLocalidad
        /// </summary>
        /// <param name="ingreso">Objecto IngresoLocalidad a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        /// 
        public int InsertarIngresoLocalidades(IngresoLocalidad ingreso, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.IngresoLocalidads.Where(Ingreso => Ingreso.año == ingreso.año && Ingreso.localidad_id == ingreso.localidad_id && Ingreso.grupo == ingreso.grupo).ToList().Count() == 0)
            {
                tabla.IngresoLocalidads.AddObject(ingreso);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.IngresoLocalidad,
                      SegmentosInsercion.Personas_Y_Pymes, null, ingreso);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de IngresoLocalidad
        /// </summary>
        /// <param name="ingreso">Objeto IngresoLocalidad utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarIngresoLocalidades(IngresoLocalidad ingreso, string Username)
        {
            int resultado = 0;
            var ingresoActual = this.tabla.IngresoLocalidads.Where(Ingreso => Ingreso.id == ingreso.id && Ingreso.id > 0).First();
            var pValorAntiguo = ingresoActual;
            ingresoActual.año = ingreso.año;
            ingresoActual.localidad_id = ingreso.localidad_id;
            ingresoActual.grupo = ingreso.grupo;
            ingresoActual.valor = ingreso.valor;

            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.IngresoLocalidad,
                      SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, ingresoActual);

            if (tabla.IngresoLocalidads.Where(Ingreso => Ingreso.año == ingreso.año && Ingreso.localidad_id == ingreso.localidad_id && Ingreso.grupo == ingreso.grupo).ToList().Count() == 0)
            {
                resultado = tabla.SaveChanges();
            }
            else if (tabla.IngresoLocalidads.Where(Ingreso => Ingreso.id == ingreso.id && Ingreso.año == ingreso.año && Ingreso.localidad_id == ingreso.localidad_id && Ingreso.grupo == ingreso.grupo).ToList().Count() == 1)
            {
                resultado = tabla.SaveChanges();
            }

            return resultado;
        }

        /// <summary>
        /// Elimina un registro de IngresoLocalidad
        /// </summary>
        /// <param name="ingreso">Objecto IngresoLocalidad utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarIngresoLocalidades(IngresoLocalidad ingreso, string Username)
        {
            String result = "";
            var ingresoActual = this.tabla.IngresoLocalidads.Where(Ingreso => Ingreso.id == ingreso.id && Ingreso.id > 0).First();
            tabla.DeleteObject(ingresoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.IngresoLocalidad,
                      SegmentosInsercion.Personas_Y_Pymes, ingresoActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}