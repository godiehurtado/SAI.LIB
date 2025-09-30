using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using Entidades = ColpatriaSAI.Negocio.Entidades;
using System.Data.Entity;
using ColpatriaSAI.Negocio.Validadores.ModeloComision;
using ColpatriaSAI.Negocio.Entidades;

using ColpatriaSAI.Datos;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Fija
{
    public class FactorComisionFija
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public ResultadoOperacionBD InsertarFactorComisionFija(Entidades.FactorComisionFija objetoDb, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == objetoDb.modelo_id).First();
               
                 if (!this.EsValidoFactorComisionFijaVsTopeComisionModelo(objetoDb))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "El porcentaje definido más el máximo factor de comisión variable no puede ser mayor que el tope del modelo.";
                }
                else
                {
                    var query = _dbcontext.FactorComisionFijas.Where(x =>
                            x.modelo_id == objetoDb.modelo_id
                            && x.compania_id == objetoDb.compania_id
                            && x.tipocontrato_id == objetoDb.tipocontrato_id
                            && x.estadoBeneficiario_id == objetoDb.estadoBeneficiario_id
                            && x.edadmaxima == objetoDb.edadmaxima
                            && x.edadminima == objetoDb.edadminima
                            && x.ramoDetalle_id == objetoDb.ramoDetalle_id);
                    if (objetoDb.planDetalle_id.HasValue)
                    {
                        query = query.Where(x => x.planDetalle_id == objetoDb.planDetalle_id);
                    }
                    if (objetoDb.productoDetalle_id.HasValue)
                    {
                        query = query.Where(x => x.productoDetalle_id == objetoDb.productoDetalle_id);
                    }
                    if (query.Count() > 0)
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "Ya se encuentra definido un factor con esta relación compañía, ramo, producto, plan, tipo contrato y rango de edades.";
                        return result;
                    }


                    //Se valida que las fechas no se traslapen
                    //Se valida que las fechas no se traslapen
                    // CAAM : 16-04.2020 _ Validaciones Removidas para dejar editar 
                    // los factores mas fluidamente x solicitud de negocio - PM11454
                    //query = _dbcontext.FactorComisionFijas.Where(x =>
                    //       x.modelo_id == objetoDb.modelo_id
                    //       && x.compania_id == objetoDb.compania_id
                    //       && x.tipocontrato_id == objetoDb.tipocontrato_id
                    //       && x.estadoBeneficiario_id == objetoDb.estadoBeneficiario_id
                    //       && x.edadmaxima >= objetoDb.edadminima
                    //       && x.edadminima <= objetoDb.edadmaxima
                    //       && x.ramoDetalle_id == objetoDb.ramoDetalle_id);

                    if (query.Count() > 0)
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "Las edades ingresadas para el factor que se desea crear se traslapan con un factor ya existente.";
                    }
                    else
                    {
                        _dbcontext.FactorComisionFijas.AddObject(objetoDb);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorComisionFija,
                    SegmentosInsercion.Personas_Y_Pymes, null, objetoDb);
                        result.RegistrosAfectados = 1;
                        result.Resultado = ResultadoOperacion.Exito;
                        _dbcontext.SaveChanges();
                    }
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

        internal ResultadoOperacionBD EliminarFactorComisionFija(Entidades.FactorComisionFija objetoDb, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == objetoDb.modelo_id).First();
               
                    objetoDb = _dbcontext.FactorComisionFijas.Where(x => x.id == objetoDb.id).First();
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorComisionFija,
                   SegmentosInsercion.Personas_Y_Pymes, objetoDb, null);
                    _dbcontext.FactorComisionFijas.DeleteObject(objetoDb);
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

        internal Entidades.FactorComisionFija ObtenerFactorComisionFijaXId(int factorId)
        {
            return _dbcontext.FactorComisionFijas
                .Where(x => x.id == factorId).FirstOrDefault();
        }

        internal ResultadoOperacionBD ActualizarFactorComisionFija(Entidades.FactorComisionFija dbobj, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == dbobj.modelo_id).First();
               
                if (!this.EsValidoFactorComisionFijaVsTopeComisionModelo(dbobj))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "El porcentaje definido más el máximo factor de comisión variable no puede ser mayor que el tope del modelo.";
                }
                else
                {
                    var validatequery = _dbcontext.FactorComisionFijas.Where(x =>
                            x.modelo_id == dbobj.modelo_id
                            && x.compania_id == dbobj.compania_id
                            && x.estadoBeneficiario_id == dbobj.estadoBeneficiario_id
                            && x.edadmaxima == dbobj.edadmaxima
                            && x.edadminima == dbobj.edadminima
                            && x.ramoDetalle_id == dbobj.ramoDetalle_id
                            && x.tipocontrato_id == dbobj.tipocontrato_id
                            && x.planDetalle_id == dbobj.planDetalle_id
                            && x.productoDetalle_id == dbobj.productoDetalle_id
                            && x.id != dbobj.id);
                   
                    if (validatequery.Count() == 0)
                    {
                        var query = _dbcontext.FactorComisionFijas.Where(x =>
                            x.id == dbobj.id);
                        if (query.Count() != 1)
                        {
                            result.RegistrosAfectados = 0;
                            result.Resultado = ResultadoOperacion.Error;
                            result.MensajeError = "No se encontró definido un factor con esta relación compañía, ramo, producto, plan, tipo contrato y rango de edades.";
                        }
                        else
                        {


                            //Se valida que las fechas no se traslapen
                            // CAAM : 16-04.2020 _ Validaciones Removidas para dejar editar 
                            // los factores mas fluidamente x solicitud de negocio - PM11454
                            //validatequery = _dbcontext.FactorComisionFijas.Where(x =>
                            //x.modelo_id == dbobj.modelo_id
                            //&& x.compania_id == dbobj.compania_id
                            //&& x.estadoBeneficiario_id == dbobj.estadoBeneficiario_id
                            //&& x.edadmaxima >= dbobj.edadminima
                            //&& x.edadminima <= dbobj.edadminima
                            //&& x.ramoDetalle_id == dbobj.ramoDetalle_id
                            //&& x.tipocontrato_id == dbobj.tipocontrato_id
                            //&& x.id != dbobj.id);



                            if (validatequery.Count() > 0)
                            {
                                result.RegistrosAfectados = 0;
                                result.Resultado = ResultadoOperacion.Error;
                                result.MensajeError = "Las fechas ingresadas para el factor que se desea modificar se traslapan con un factor ya existente.";
                            }
                            else
                            {
                                var tmpobj = query.First();
                                var pValorAntiguo = tmpobj;
                                tmpobj.estadoBeneficiario_id = dbobj.estadoBeneficiario_id;
                                tmpobj.compania_id = dbobj.compania_id;
                                tmpobj.tipocontrato_id = dbobj.tipocontrato_id;
                                tmpobj.edadmaxima = dbobj.edadmaxima;
                                tmpobj.edadminima = dbobj.edadminima;
                                tmpobj.factor = dbobj.factor;
                                tmpobj.planDetalle_id = dbobj.planDetalle_id;
                                tmpobj.productoDetalle_id = dbobj.productoDetalle_id;
                                tmpobj.ramoDetalle_id = dbobj.ramoDetalle_id;
                                result.RegistrosAfectados = 1;
                                result.Resultado = ResultadoOperacion.Exito;
                                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.FactorComisionFija,
                                SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tmpobj);
                                _dbcontext.SaveChanges();
                            }



                        }
                    }
                    else
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "Ya se encuentra definido un factor con esta relación compañía, ramo, producto, plan, tipo contrato y factor.";
                    }
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

        private bool EsValidoFactorComisionFijaVsTopeComisionModelo(Entidades.FactorComisionFija objetoDb)
        {
            var query = _dbcontext.MatrizComisionVariables.Where(x=>x.modelo_id == objetoDb.modelo_id);
            double maxFactorComisionVariable = query.Count() == 0 ? 0 : query.Max(x => x.factor);
            double topeModelo = _dbcontext.ModeloComisions.Where(x => x.id == objetoDb.modelo_id).Select(x => x.tope).FirstOrDefault();
            maxFactorComisionVariable = 0;
            return objetoDb.factor + maxFactorComisionVariable <= topeModelo;
        }

        public List<Entidades.FactorComisionFija> ListarFactorComisionFijaXModeloId(int modeloId)
        {
            return _dbcontext.FactorComisionFijas.Include("Compania")
                .Include("RamoDetalle")
                .Include("TipoContrato")
                .Include("PlanDetalle")
                .Where(x => x.modelo_id == modeloId).ToList();
        }

        /// <summary>
        /// Se valida la edad maxima ingresada como factor contra la edad maxima configurada en BD
        /// </summary>
        /// <param name="edad"></param>
        /// <returns></returns>
        internal Boolean ValidarEdad(int edad)
        {
            Boolean rtaVal = false;

            StringBuilder lSentencia = new StringBuilder();
            lSentencia.Append("select * from SAI_COMISIONES_CONFIG");
            lSentencia.Append(" where id=15 and valor>" + edad);

            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)_dbcontext.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand(lSentencia.ToString(), sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                daReport.Fill(retVal);
            }

            if (retVal.Tables.Count > 0 && retVal.Tables[0].Rows.Count > 0)
                rtaVal = true;

            return rtaVal;

        }
    }
}
