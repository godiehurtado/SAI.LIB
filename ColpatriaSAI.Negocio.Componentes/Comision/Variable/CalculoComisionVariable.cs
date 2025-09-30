using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Variable
{
    public class CalculoComisionVariable
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        internal ResultadoOperacionBD InsertarLiquidacionComision(Entidades.LiquidacionComision obj, string Username = default(string))
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                _dbcontext.LiquidacionComisions.AddObject(obj);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquidacionComision,
                    SegmentosInsercion.Personas_Y_Pymes, null, obj);
                _dbcontext.SaveChanges();
                result.RegistrosAfectados = 1;
                result.Resultado = ResultadoOperacion.Exito;
                result.IdInsercion = obj.id;
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }
    }
}
