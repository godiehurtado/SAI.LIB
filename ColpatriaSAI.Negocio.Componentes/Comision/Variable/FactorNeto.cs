using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using Entidades = ColpatriaSAI.Negocio.Entidades;
using System.Data.Entity;
using ColpatriaSAI.Negocio.Validadores.ModeloComision;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Variable
{
    public class FactorNeto
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<Entidades.FactorComisionVariableNeto> ListarFactorNetoComisionVariableXModeloId(int modeloId)
        {
            return _dbcontext.FactorComisionVariableNetoes.Include("Compania")
                .Include("RamoDetalle")
                .Include("TipoContrato")
                .Include("PlanDetalle")
                .Where(x => x.modelo_id == modeloId).ToList();
        }

        public ResultadoOperacionBD InsertarFactorNeto(Entidades.FactorComisionVariableNeto dbobj, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == dbobj.modelo_id).First();
                
                    var query = _dbcontext.FactorComisionVariableNetoes.Where(x =>
                            x.modelo_id == dbobj.modelo_id
                            && x.compania_id == dbobj.compania_id
                            && x.estadoBeneficiario_id == dbobj.estadoBeneficiario_id 
                            && x.tipocontrato_id == dbobj.tipocontrato_id
                            && x.factor == dbobj.factor
                            && x.ramoDetalle_id == dbobj.ramoDetalle_id);
                    if (dbobj.planDetalle_id.HasValue)
                    {
                        query = query.Where(x => x.planDetalle_id == dbobj.planDetalle_id);
                    }
                    if (dbobj.productoDetalle_id.HasValue)
                    {
                        query = query.Where(x => x.productoDetalle_id == dbobj.productoDetalle_id);
                    }
                    if (query.Count() > 0)
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "Ya se encuentra definido un factor con esta relación compañía, ramo, producto, plan, tipo contrato y factor.";
                    }
                    else
                    {
                        _dbcontext.FactorComisionVariableNetoes.AddObject(dbobj);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNeto,
 SegmentosInsercion.Personas_Y_Pymes, null, dbobj);
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

        internal ResultadoOperacionBD ActualizarFactorNeto(Entidades.FactorComisionVariableNeto dbobj, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == dbobj.modelo_id).First();
                
                    var validatequery = _dbcontext.FactorComisionVariableNetoes.Where(x =>
                            x.modelo_id == dbobj.modelo_id
                            && x.compania_id == dbobj.compania_id
                            && x.estadoBeneficiario_id == dbobj.estadoBeneficiario_id                           
                            && x.tipocontrato_id == dbobj.tipocontrato_id
                            && x.ramoDetalle_id == dbobj.ramoDetalle_id
                            && x.id != dbobj.id);
                    if (dbobj.planDetalle_id.HasValue)
                    {
                        validatequery = validatequery.Where(x => x.planDetalle_id == dbobj.planDetalle_id); 
                    }
                    if (dbobj.productoDetalle_id.HasValue)
                    {
                        validatequery = validatequery.Where(x => x.productoDetalle_id == dbobj.productoDetalle_id);
                    }
                    if (validatequery.Count() == 0)
                    {
                        var query = _dbcontext.FactorComisionVariableNetoes.Where(x =>
                                x.id == dbobj.id);
                        if (query.Count() != 1)
                        {
                            result.RegistrosAfectados = 0;
                            result.Resultado = ResultadoOperacion.Error;
                            result.MensajeError = "No se encontró definido el factor.";
                        }
                        else
                        {
                            var tmpobj = query.First();
                            var pValorAntiguo = tmpobj;
                            tmpobj.estadoBeneficiario_id = dbobj.estadoBeneficiario_id;
                            tmpobj.compania_id = dbobj.compania_id;
                            tmpobj.tipocontrato_id = dbobj.tipocontrato_id;
                            tmpobj.factor = dbobj.factor;
                            tmpobj.planDetalle_id = dbobj.planDetalle_id;
                            tmpobj.productoDetalle_id = dbobj.productoDetalle_id;
                            tmpobj.ramoDetalle_id = dbobj.ramoDetalle_id;
                            result.RegistrosAfectados = 1;
                            result.Resultado = ResultadoOperacion.Exito;
                            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNeto,
 SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tmpobj);
                            _dbcontext.SaveChanges();
                        }
                    }
                    else
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "Ya se encuentra definido un factor con esta relación compañía, ramo, producto, plan, tipo contrato y factor.";
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

        internal Entidades.FactorComisionVariableNeto ObtenerFactorNetoComisionVariableXId(int factorId)
        {
            return _dbcontext.FactorComisionVariableNetoes
                .Where(x => x.id == factorId).FirstOrDefault();
        }

        internal ResultadoOperacionBD EliminarFactorNeto(Entidades.FactorComisionVariableNeto dbobj, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == dbobj.modelo_id).First();
                
                    dbobj = _dbcontext.FactorComisionVariableNetoes.Where(x => x.id == dbobj.id).First();
                    _dbcontext.FactorComisionVariableNetoes.DeleteObject(dbobj);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNeto,
 SegmentosInsercion.Personas_Y_Pymes, dbobj, null);
                    result.RegistrosAfectados = 1;
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
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
