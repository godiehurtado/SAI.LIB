using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Validadores.ModeloComision;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Modelo.Excepciones
{
    public class ExcepcionFijaVariable
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public ResultadoOperacionBD InsertarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                if (objetoDB.tipoexcepcionfijavariable_id == 1 || this.EsValidoPorcentajeExcepcion(objetoDB.modelo_id.Value, objetoDB.porcentajecomision.Value))
                {
                    var query = _dbcontext
                                        .ExcepcionFijaVariables
                                        .Where(x =>
                                                x.modelo_id == objetoDB.modelo_id
                                                && x.numerocontrato == objetoDB.numerocontrato
                                                && x.participante_id == objetoDB.participante_id
                                                && (x.activo.HasValue && x.activo.Value)
                                                && (((objetoDB.fechavigencia < x.fechavigencia
                                                        && objetoDB.fechafinvigencia > x.fechavigencia)
                                                    || (objetoDB.fechavigencia > x.fechavigencia
                                                        && objetoDB.fechavigencia < x.fechafinvigencia)
                                                    || (objetoDB.fechafinvigencia > x.fechavigencia
                                                        && objetoDB.fechafinvigencia < x.fechafinvigencia)
                                                    || objetoDB.fechavigencia == x.fechavigencia
                                                    || objetoDB.fechafinvigencia == x.fechafinvigencia
                                                    ))
                                                );
                    if (query.Count() == 0)
                    {
                        var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == objetoDB.modelo_id).First();
                        if (querymodel.fechadesde > objetoDB.fechavigencia || querymodel.fechahasta < objetoDB.fechavigencia
                            || querymodel.fechadesde > objetoDB.fechafinvigencia || querymodel.fechahasta < objetoDB.fechafinvigencia)
                        {
                            result.RegistrosAfectados = 0;
                            result.Resultado = ResultadoOperacion.Error;
                            result.MensajeError = "No se puede modificar la excepción. La vigencia no se encuentra dentro la definida en el modelo";
                        }
                        else
                        {
                            _dbcontext.ExcepcionFijaVariables.AddObject(objetoDB);

                            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
 SegmentosInsercion.Personas_Y_Pymes, null, objetoDB);
                            result.RegistrosAfectados = 1;
                            result.Resultado = ResultadoOperacion.Exito;
                            _dbcontext.SaveChanges();
                        }
                    }
                    else
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "No se puede crear la excepción. Ya existe una excepción definida de esta manera dentro del modelo";
                    }
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede crear la excepción. El porcentaje de comisión debe ser menor al mínimo definido para la comisión fija";
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        public ResultadoOperacionBD ActualizarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {

                if (objetoDB.tipoexcepcionfijavariable_id == 1 || this.EsValidoPorcentajeExcepcion(objetoDB.modelo_id.Value, objetoDB.porcentajecomision.Value))
                {
                    var query = _dbcontext
                                        .ExcepcionFijaVariables
                                        .Where(x =>
                                                x.modelo_id == objetoDB.modelo_id
                                                && x.numerocontrato == objetoDB.numerocontrato
                                                && x.participante_id == objetoDB.participante_id
                                                && x.id != objetoDB.id
                                                && (x.activo.HasValue && x.activo.Value)
                                                && (((objetoDB.fechavigencia < x.fechavigencia
                                                        && objetoDB.fechafinvigencia > x.fechavigencia)
                                                    || (objetoDB.fechavigencia > x.fechavigencia
                                                        && objetoDB.fechavigencia < x.fechafinvigencia)
                                                    || (objetoDB.fechafinvigencia > x.fechavigencia
                                                        && objetoDB.fechafinvigencia < x.fechafinvigencia)
                                                    || objetoDB.fechavigencia == x.fechavigencia
                                                    || objetoDB.fechafinvigencia == x.fechafinvigencia
                                                    ))
                                                );
                    if (query.Count() == 0)
                    {

                        var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == objetoDB.modelo_id).First();

                        if (querymodel.fechadesde > objetoDB.fechavigencia || querymodel.fechahasta < objetoDB.fechavigencia
                            || querymodel.fechadesde > objetoDB.fechafinvigencia || querymodel.fechahasta < objetoDB.fechafinvigencia)
                        {
                            result.RegistrosAfectados = 0;
                            result.Resultado = ResultadoOperacion.Error;
                            result.MensajeError = "No se puede modificar la excepción. La vigencia no se encuentra dentro la definida en el modelo";
                        }
                        else
                        {
                            query = _dbcontext
                                            .ExcepcionFijaVariables
                                            .Where(x =>
                                                    x.id == objetoDB.id
                                                    && x.modelo_id == objetoDB.modelo_id
                                                    );
                            if (query.Count() != 1)
                            {
                                result.RegistrosAfectados = 0;
                                result.Resultado = ResultadoOperacion.Error;
                                result.MensajeError = "Excepción a la comisión no existente.";
                            }
                            else
                            {
                                var tmpobj = query.First();
                                var pValorAntiguo = tmpobj;
                                tmpobj.tipoexcepcionfijavariable_id = objetoDB.tipoexcepcionfijavariable_id;
                                tmpobj.numerocontrato = objetoDB.numerocontrato;
                                tmpobj.participante_id = objetoDB.participante_id;
                                tmpobj.porcentajecomision = objetoDB.porcentajecomision;
                                tmpobj.fechavigencia = objetoDB.fechavigencia;
                                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tmpobj);
                                result.RegistrosAfectados = 1;
                                result.Resultado = ResultadoOperacion.Exito;
                                _dbcontext.SaveChanges();
                            }
                        }


                    }
                    else
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "No se puede pueden aplicar los cambios a la excepción. No se pueden definir excepciones para una misma referencia con vigencias en conflicto";
                    }
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar la excepción. El porcentaje de comisión debe ser menor al mínimo definido para la comisión fija";
                }

            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        public ResultadoOperacionBD EliminarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDb, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == objetoDb.modelo_id).First();
                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(querymodel.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else
                {
                    objetoDb = _dbcontext.ExcepcionFijaVariables.FirstOrDefault(exv => exv.id == objetoDb.id);
                    _dbcontext.ExcepcionFijaVariables.DeleteObject(objetoDb);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
 SegmentosInsercion.Personas_Y_Pymes, objetoDb, null);
                    result.RegistrosAfectados = 1;
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        public bool EsValidoPorcentajeExcepcion(int modeloId, double porcentajeComision)
        {
            var query = _dbcontext.FactorComisionFijas.Where(x => x.modelo_id == modeloId).GroupBy(g => g.modelo_id).Select(y => new { MinimoComisionFija = y.Min(c => c.factor) });
            double porcentajeMinimoComisionFija = 0;
            if (query.Count() > 0)
            {
                porcentajeMinimoComisionFija = query.First().MinimoComisionFija;
            }
            return porcentajeComision <= porcentajeMinimoComisionFija;
        }

        internal ResultadoOperacionBD CambiarEstadoExcepcionFijaVariable(int excepcionId, int modeloId, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var objetoDb = _dbcontext.ExcepcionFijaVariables.FirstOrDefault(x => x.id == excepcionId && x.modelo_id == modeloId);
                objetoDb.activo = objetoDb.activo.HasValue ? !objetoDb.activo.Value : false;
                if (objetoDb.activo.Value)
                {
                    var query = _dbcontext
                                        .ExcepcionFijaVariables
                                        .Where(x =>
                                                x.modelo_id == objetoDb.modelo_id
                                                && x.numerocontrato == objetoDb.numerocontrato
                                                && x.participante_id == objetoDb.participante_id
                                                && x.id != objetoDb.id
                                                && (x.activo.HasValue && x.activo.Value)
                                                && (((objetoDb.fechavigencia < x.fechavigencia
                                                        && objetoDb.fechafinvigencia > x.fechavigencia)
                                                    || (objetoDb.fechavigencia > x.fechavigencia
                                                        && objetoDb.fechavigencia < x.fechafinvigencia)
                                                    || (objetoDb.fechafinvigencia > x.fechavigencia
                                                        && objetoDb.fechafinvigencia < x.fechafinvigencia)
                                                    || objetoDb.fechavigencia == x.fechavigencia
                                                    || objetoDb.fechafinvigencia == x.fechafinvigencia
                                                    ))
                                                );
                    if (query.Count() == 0)
                    {
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
   SegmentosInsercion.Personas_Y_Pymes, objetoDb, null);
                        result.RegistrosAfectados = 1;
                        result.Resultado = ResultadoOperacion.Exito;
                        _dbcontext.SaveChanges();
                    }
                    else
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "No se puede activar la excepción. Ya existe una excepción activa definida de esta manera dentro del modelo ";
                    }
                }
                else
                {
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
   SegmentosInsercion.Personas_Y_Pymes, objetoDb, null);
                    result.RegistrosAfectados = 1;
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
                }

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
