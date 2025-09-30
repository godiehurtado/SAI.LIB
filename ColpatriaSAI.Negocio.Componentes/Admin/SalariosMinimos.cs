using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    class SalariosMinimos
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de las SalariosMinimos
        /// </summary>
        /// <returns>Lista de SalariosMinimos</returns> 
        public List<SalarioMinimo> ListarSalariosMinimos()
        {
            return tabla.SalarioMinimoes.Where(Salario => Salario.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de los SalariosMinimos por id
        /// </summary>
        /// <param name="idSalario">Id del SalarioMinimo a consultar</param>
        /// <returns>Lista de objectos SalarioMinimo</returns>
        public List<SalarioMinimo> ListarSalariosMinimosPorId(int idSalario)
        {
            return tabla.SalarioMinimoes.Where(Salario => Salario.id == idSalario && Salario.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de SalarioMinimo
        /// </summary>
        /// <param name="salario">Objecto SalarioMinimo a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        /// 
        public int InsertarSalarioMinimo(SalarioMinimo salario, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.SalarioMinimoes.Where(Salario => Salario.anio == salario.anio).ToList().Count() == 0)
            {
                tabla.SalarioMinimoes.AddObject(salario);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.SalarioMinimo,
                      SegmentosInsercion.Personas_Y_Pymes, null, salario);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de SalarioMinimo
        /// </summary>
        /// <param name="salario">Objeto SalarioMinimo utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarSalarioMinimo(SalarioMinimo salario, string Username)
        {
            int resultado = 0;
            if (tabla.SalarioMinimoes.Where(Salario => Salario.id == salario.id && Salario.smlv == salario.smlv).ToList().Count() == 0)
            {
                var salarioActual = this.tabla.SalarioMinimoes.Where(Salario => Salario.id == salario.id && Salario.id > 0).First();
                var pValorAntiguo = salarioActual;
                salarioActual.smlv = salario.smlv;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SalarioMinimo,
                      SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, salarioActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de SalarioMinimo
        /// </summary>
        /// <param name="salario">Objecto SalarioMinimo utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarSalarioMinimo(SalarioMinimo salario, string Username)
        {
            String result = "";
            var salarioActual = this.tabla.SalarioMinimoes.Where(Salario => Salario.id == salario.id && Salario.id > 0).First();
            tabla.DeleteObject(salarioActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SalarioMinimo,
                      SegmentosInsercion.Personas_Y_Pymes, salarioActual, null);
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