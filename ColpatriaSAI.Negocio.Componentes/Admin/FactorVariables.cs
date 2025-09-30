using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class FactorVariables
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de las FactorVariable
        /// </summary>
        /// <returns>Lista de FactorVariable</returns>
        public List<FactorVariable> ListarFactorVariables()
        {
            return tabla.FactorVariables.ToList();
        }

        /// <summary>
        /// Obtiene listado de FactorVariable por id
        /// </summary>
        /// <param name="id">Id del FactorVariable a consultar</param>
        /// <returns>Lista de objectos FactorVariable</returns>
        public List<FactorVariable> ListarFactorVariablesPorId(int id)
        {
            return tabla.FactorVariables.Where(FactorVariable => FactorVariable.id == id).ToList();
        }

        /// <summary>
        /// Guarda un registro del FactorVariable
        /// </summary>
        /// <param name="factorVariable">Objecto FactorVariable a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarFactorVariable(FactorVariable factorVariable, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.FactorVariables.Where(FactorVariable => FactorVariable.nombre == factorVariable.nombre).ToList().Count() == 0)
            {
                tabla.FactorVariables.AddObject(factorVariable);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorVariable,
    SegmentosInsercion.Personas_Y_Pymes, null, factorVariable);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro del FactorVariable
        /// </summary>
        /// <param name="id">Id del FactorVariable a modificar</param>
        /// <param name="factorVariable">Objeto FactorVariable utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarFactorVariable(int id, FactorVariable factorVariable, string Username)
        {
            var factorVariableActual = this.tabla.FactorVariables.Where(FactorVariable => FactorVariable.id == id).First();
            var pValorAntiguo = factorVariableActual;
            factorVariableActual.nombre = factorVariable.nombre;
            factorVariableActual.valorDirecto = factorVariable.valorDirecto;
            factorVariableActual.valorContratacion = factorVariable.valorContratacion;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.FactorVariable,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, factorVariableActual);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de FactorVariable
        /// </summary>
        /// <param name="id">Id del FactorVariable a eliminar</param>
        /// <param name="factorVariable">Objecto FactorVariable utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarFactorVariable(int id, FactorVariable factorVariable, string Username)
        {
            var factorVariableActual = this.tabla.FactorVariables.Where(FactorVariable => FactorVariable.id == id).First();
            tabla.DeleteObject(factorVariableActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorVariable,
    SegmentosInsercion.Personas_Y_Pymes, factorVariableActual, null);
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

